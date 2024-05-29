using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using StreamMaster.Application.StreamGroups.CommandsOld;
using StreamMaster.Domain.Authentication;
using StreamMaster.Domain.Enums;
using StreamMaster.Domain.Repository;
using StreamMaster.Infrastructure.Clients;
using StreamMaster.Streams.Domain.Interfaces;

namespace StreamMaster.API.Controllers;

public class VideoStreamsController(IChannelManager channelManager, IRepositoryWrapper repositoryWrapper, ILogger<VideoStreamsController> logger) : ApiControllerBase
{
    [HttpPost]
    [Route("[action]")]
    public IActionResult FailClient(FailClientRequest request)
    {
        channelManager.FailClient(request.clientId);
        return Ok();
    }


    [Authorize(Policy = "SGLinks")]
    [HttpGet]
    [HttpHead]
    [Route("stream/{encodedIds}")]
    [Route("stream/{encodedIds}.mp4")]
    [Route("stream/{encodedIds}.ts")]
    [Route("stream/{encodedIds}/{name}")]
    public async Task<ActionResult> GetVideoStreamStream(string encodedIds, string name, CancellationToken cancellationToken)
    {

        (int? StreamGroupNumberNull, string? SMChannelId) = encodedIds.DecodeTwoValuesAsString128(Settings.ServerKey);
        if (StreamGroupNumberNull == null || string.IsNullOrEmpty(SMChannelId))
        {
            return new NotFoundResult();
        }

        int streamGroupNumber = (int)StreamGroupNumberNull;

        if (!int.TryParse(SMChannelId, out int smChannelId))
        {
            return new NotFoundResult();
        }


        SMChannel? smChannel = repositoryWrapper.SMChannel.GetSMChannel(smChannelId);

        if (smChannel == null)
        {
            logger.LogInformation("GetStreamGroupVideoStream request. SG Number {id} ChannelId {channelId} not found exiting", streamGroupNumber, smChannelId);
            return NotFound();
        }
        logger.LogInformation("GetStreamGroupVideoStream request. SG Number {id} ChannelId {channelId}", streamGroupNumber, smChannelId);


        if (smChannel.SMStreams.Count == 0 || string.IsNullOrEmpty(smChannel.SMStreams.First().SMStream.Url))
        {
            logger.LogInformation("GetStreamGroupVideoStream request. SG Number {id} ChannelId {channelId} missing url or additional streams", streamGroupNumber, smChannelId);
            return new NotFoundResult();
        }

        HttpContext.Session.Remove("ClientId");

        StreamingProxyTypes proxyType = GetStreamingProxyType(smChannel);
        bool redirect = proxyType == StreamingProxyTypes.None;

        if (redirect)
        {
            logger.LogInformation("GetStreamGroupVideoStream request SG Number {id} ChannelId {channelId} proxy is none, sending redirect", streamGroupNumber, smChannelId);

            return Redirect(smChannel.SMStreams.First().SMStream.Url);
        }

        string? ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

        ClientStreamerConfiguration config = new(smChannel, Request.Headers["User-Agent"].ToString(), ipAddress ?? "unkown", cancellationToken, HttpContext.Response);

        Stream? stream = await channelManager.GetChannel(config);


        HttpContext.Response.RegisterForDispose(new UnregisterClientOnDispose(channelManager, config));
        return stream != null ? new FileStreamResult(stream, "video/mp4") : StatusCode(StatusCodes.Status404NotFound);
    }

    private StreamingProxyTypes GetStreamingProxyType(SMChannel smChannel)
    {

        return smChannel.StreamingProxyType == StreamingProxyTypes.SystemDefault
            ? Settings.StreamingProxyType
            : smChannel.StreamingProxyType;
    }
    public async Task ReadAndWriteAsync(Stream sourceStream, string filePath, CancellationToken cancellationToken = default)
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
            using FileStream fileStream = new(filePath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize, true);

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



    [HttpPost]
    [Route("[action]")]
    public ActionResult SimulateStreamFailureForAll()
    {
        channelManager.SimulateStreamFailureForAll();
        return Ok();
    }


    [HttpPost]
    [Route("[action]")]
    public async Task<IActionResult> SimulateStreamFailure(SimulateStreamFailureRequest request)
    {
        await Mediator.Send(request).ConfigureAwait(false);
        return Ok();
    }


    private class UnregisterClientOnDispose(IChannelManager channelManager, ClientStreamerConfiguration config) : IDisposable
    {
        private readonly IChannelManager _channelManager = channelManager;
        private readonly ClientStreamerConfiguration _config = config;

        public void Dispose()
        {
            _channelManager.RemoveClient(_config);
        }
    }
}