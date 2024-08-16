using AutoMapper;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace StreamMaster.API.Controllers;

[V1ApiController("v")]
public class VsController(ILogger<VsController> logger, IRepositoryWrapper repositoryWrapper, IStreamGroupService streamGroupService, IChannelManager channelManager, IClientConfigurationService clientConfigurationService, IMapper mapper) : Controller
{
    [Authorize(Policy = "SGLinks")]
    [HttpGet]
    [HttpHead]
    [Route("{smChannelId}")]
    [Route("{smChannelId}.ts")]
    [Route("{streamGroupProfileId}/{smChannelId}")]
    [Route("{streamGroupProfileId}/{smChannelId}.ts")]
    public async Task<ActionResult> GetVideoStreamStream(int smChannelId, int? streamGroupProfileId, CancellationToken cancellationToken)
    {
        int sgId = await streamGroupService.GetStreamGroupIdFromSGProfileIdAsync(streamGroupProfileId).ConfigureAwait(false);
        int defaultSGId = await streamGroupService.GetDefaultSGIdAsync();

        SMChannel? smChannel = sgId == defaultSGId
            ? repositoryWrapper.SMChannel.GetSMChannel(smChannelId)
            : repositoryWrapper.SMChannel.GetSMChannelFromStreamGroup(smChannelId, sgId);

        if (smChannel == null)
        {
            logger.LogInformation("Channel with ChannelId {smChannelId} not found, exiting", smChannelId);
            return NotFound();
        }

        if (smChannel.SMStreams.Count == 0 || string.IsNullOrEmpty(smChannel.SMStreams.First().SMStream.Url))
        {
            logger.LogInformation("Channel with ChannelId {smChannelId} has no streams, exiting", smChannelId);
            return new NotFoundResult();
        }

        StreamGroupProfile streamGroupProfile = await streamGroupService.GetStreamGroupProfileAsync(null, streamGroupProfileId);
        CommandProfileDto commandProfileDto = await streamGroupService.GetProfileFromSGIdsCommandProfileNameAsync(null, streamGroupProfile.Id, smChannel.CommandProfileName);

        if (commandProfileDto.ProfileName.Equals("Redirect", StringComparison.InvariantCultureIgnoreCase))
        {
            logger.LogInformation("Channel with ChannelId {channelId} redirecting", smChannelId);
            return Redirect(smChannel.SMStreams.First().SMStream.Url);
        }

        string? ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

        SMChannelDto smChannelDto = mapper.Map<SMChannelDto>(smChannel);
        foreach (SMStreamDto sm in smChannelDto.SMStreams.OrderBy(a => a.Rank))
        {
            sm.Rank = smChannel.SMStreams.First(a => a.SMStreamId == sm.Id).Rank;
        }

        HttpRequest request = HttpContext.Request;
        string originalUrl = $"{request.Scheme}://{request.Host}{request.PathBase}{request.Path}{request.QueryString}";
        smChannelDto.StreamUrl = originalUrl;

        string uniqueRequestId = request.HttpContext.TraceIdentifier;
        IClientConfiguration config = clientConfigurationService.NewClientConfiguration(uniqueRequestId, smChannelDto, Request.Headers.UserAgent.ToString(), ipAddress ?? "unknown", HttpContext.Response, cancellationToken);

        logger.LogInformation("Requesting channel with ChannelId {channelId}", smChannelId);
        Stream? stream = await channelManager.GetChannelStreamAsync(config, streamGroupProfile.Id, cancellationToken);
        logger.LogInformation("Streaming channel with ChannelId {channelId} to client {id}", smChannelId, uniqueRequestId);

        HttpContext.Response.RegisterForDispose(new UnregisterClientOnDispose(channelManager, config));
        return stream != null ? new FileStreamResult(stream, "video/mp4") { EnableRangeProcessing = false } : StatusCode(StatusCodes.Status404NotFound);
    }

    private class UnregisterClientOnDispose(IChannelManager channelManager, IClientConfiguration config) : IDisposable
    {
        private readonly IChannelManager _channelManager = channelManager;
        private readonly IClientConfiguration _config = config;

        public void Dispose()
        {
            _ = _channelManager.RemoveClientAsync(_config);
        }
    }
}