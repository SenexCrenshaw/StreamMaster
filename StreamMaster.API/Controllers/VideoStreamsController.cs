using AutoMapper;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using StreamMaster.Application.Crypto.Commands;
using StreamMaster.Domain.Repository;
using StreamMaster.Streams.Domain.Interfaces;
using StreamMaster.Streams.Domain.Models;

namespace StreamMaster.API.Controllers;

public class VideoStreamsController(IChannelManager channelManager, ISender sender, IMapper mapper, IRepositoryWrapper repositoryWrapper, ILogger<VideoStreamsController> logger)
    : ApiControllerBase
{
    [Authorize(Policy = "SGLinks")]
    [HttpGet]
    [HttpHead]
    [Route("stream/{encodedIds}")]
    [Route("stream/{encodedIds}.mp4")]
    [Route("stream/{encodedIds}.ts")]
    [Route("stream/{encodedIds}/{name}")]
    public async Task<ActionResult> GetVideoStreamStream(string encodedIds, string name, CancellationToken cancellationToken)
    {
        (int? streamGroupId, int? streamGroupProfileId, int? smChannelId) = await sender.Send(new DecodeProfileIdSMChannelIdFromEncoded(encodedIds), cancellationToken);

        if (!streamGroupId.HasValue || !streamGroupProfileId.HasValue || !smChannelId.HasValue)
        {
            return new NotFoundResult();
        }

        SMChannel? smChannel = streamGroupId < 2
            ? repositoryWrapper.SMChannel.GetSMChannel(smChannelId.Value)
            : repositoryWrapper.SMChannel.GetSMChannelFromStreamGroup(smChannelId.Value, streamGroupId.Value);

        if (smChannel == null)
        {
            logger.LogInformation("GetVideoStreamStream request. SG Number {id} ChannelId {channelId} not found exiting", streamGroupId.Value, smChannelId);
            return NotFound();
        }
        logger.LogInformation("GetVideoStreamStream request. SG Number {id} ChannelId {channelId}", streamGroupId.Value, smChannelId);

        if (smChannel.SMStreams.Count == 0 || string.IsNullOrEmpty(smChannel.SMStreams.First().SMStream.Url))
        {
            logger.LogInformation("GetVideoStreamStream request. SG Number {id} ChannelId {channelId} no streams", streamGroupId.Value, smChannelId);
            return new NotFoundResult();
        }

        StreamGroupProfile? streamGroupProfile = await repositoryWrapper.StreamGroupProfile.GetStreamGroupProfileAsync(streamGroupId.Value, streamGroupProfileId.Value);

        if (streamGroupProfile == null || streamGroupProfile?.CommandProfileName == "None")
        {
            logger.LogInformation("GetVideoStreamStream request SG Number {id} ChannelId {channelId} proxy is none, sending redirect", streamGroupId.Value, smChannelId);

            return Redirect(smChannel.SMStreams.First().SMStream.Url);
        }

        string? ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

        SMChannelDto smChannelDto = mapper.Map<SMChannelDto>(smChannel);

        HttpRequest request = HttpContext.Request;
        string originalUrl = $"{request.Scheme}://{request.Host}{request.PathBase}{request.Path}{request.QueryString}";
        smChannelDto.StreamUrl = originalUrl;
        string uniqueRequestId = request.HttpContext.TraceIdentifier;
        ClientStreamerConfiguration config = new(uniqueRequestId, smChannelDto, streamGroupId.Value, streamGroupProfile!.Id, Request.Headers.UserAgent.ToString(), ipAddress ?? "unknown", HttpContext.Response, cancellationToken);
        Stream? stream = await channelManager.GetChannelStreamAsync(config, cancellationToken);

        HttpContext.Response.RegisterForDispose(new UnregisterClientOnDispose(channelManager, config));
        return stream != null ? new FileStreamResult(stream, "video/mp4") : StatusCode(StatusCodes.Status404NotFound);
    }
    private async Task ReadAndWriteAsync(Stream sourceStream, string filePath, CancellationToken cancellationToken = default)
    {
        const int bufferSize = 1024; // Read in chunks of 1024 bytes
        const int totalSize = 1 * 1024 * 1024;
        Memory<byte> buffer = new(new byte[bufferSize]);
        int totalBytesRead = 0;

        // Ensure the source stream supports reading
        if (!sourceStream.CanRead)
        {
            throw new InvalidOperationException("Source stream does not support reading.");
        }

        try
        {
            await using FileStream fileStream = new(filePath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize, true);

            while (totalBytesRead < totalSize)
            {
                int maxReadLength = Math.Min(buffer.Length, totalSize - totalBytesRead);
                int bytesRead = await sourceStream.ReadAsync(buffer[..maxReadLength], cancellationToken).ConfigureAwait(false);

                if (bytesRead == 0)
                {
                    break; // End of the source stream
                }

                await fileStream.WriteAsync(buffer[..bytesRead], cancellationToken).ConfigureAwait(false);
                totalBytesRead += bytesRead;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error writing stream to file");
            throw;
        }
    }

    private class UnregisterClientOnDispose(IChannelManager channelManager, ClientStreamerConfiguration config) : IDisposable
    {
        private readonly IChannelManager _channelManager = channelManager;
        private readonly ClientStreamerConfiguration _config = config;

        public void Dispose()
        {
            _ = _channelManager.RemoveClientAsync(_config);
        }
    }
}