using AutoMapper;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace StreamMaster.API.Controllers;

[V1ApiController("v")]
public class VsController(ILogger<VsController> logger, IRepositoryWrapper repositoryWrapper, IClientConfigurationService clientConfigurationService, IMapper mapper) : Controller

{

    [Authorize(Policy = "SGLinks")]
    [HttpGet]
    [HttpHead]
    [Route("{smChannelId}")]
    [Route("{smChannelId}.ts")]
    [Route("{streamGroupProfileId}/{smChannelId}")]
    [Route("{streamGroupProfileId}/{smChannelId}.ts")]
    public async Task<ActionResult> GetVideoStreamStream(int smChannelId, IStreamGroupService streamGroupService, IChannelManager channelManager, int? streamGroupProfileId, CancellationToken cancellationToken)
    {
        SMChannel? smChannel = repositoryWrapper.SMChannel.GetSMChannel(smChannelId);
        if (smChannel == null)
        {
            logger.LogInformation("GetVideoStreamStream request.ChannelId {smChannelId} not found exiting", smChannelId);
            return NotFound();
        }

        int? sgId = await streamGroupService.GetDefaultSGIdAsync().ConfigureAwait(false);
        if (sgId == null)
        {
            logger.LogInformation("GetVideoStreamStream sgId not found for ChannelId {smChannelId} exiting", smChannelId);
            return NotFound();
        }

        logger.LogInformation("GetVideoStreamStream request. SG Number {id} ChannelId {channelId}", sgId, smChannelId);

        if (smChannel.SMStreams.Count == 0 || string.IsNullOrEmpty(smChannel.SMStreams.First().SMStream.Url))
        {
            logger.LogInformation("GetVideoStreamStream request. SG Number {id} ChannelId {channelId} no streams", sgId, smChannelId);
            return new NotFoundResult();
        }

        //if (streamGroupProfile == null || streamGroupProfile?.CommandProfileName == "None")
        //{
        //    logger.LogInformation("GetVideoStreamStream request streamGroupProfileId {streamGroupProfileId} ChannelId {channelId} proxy is none, sending redirect", streamGroupProfileId, smChannelId);

        //    return Redirect(smChannel.SMStreams.First().SMStream.Url);
        //}

        string? ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

        SMChannelDto smChannelDto = mapper.Map<SMChannelDto>(smChannel);

        HttpRequest request = HttpContext.Request;
        string originalUrl = $"{request.Scheme}://{request.Host}{request.PathBase}{request.Path}{request.QueryString}";
        smChannelDto.StreamUrl = originalUrl;
        string uniqueRequestId = request.HttpContext.TraceIdentifier;
        IClientConfiguration config = clientConfigurationService.NewClientConfiguration(uniqueRequestId, smChannelDto, sgId.Value, 1, Request.Headers.UserAgent.ToString(), ipAddress ?? "unknown", HttpContext.Response, cancellationToken);
        Stream? stream = await channelManager.GetChannelStreamAsync(config, cancellationToken);

        HttpContext.Response.RegisterForDispose(new UnregisterClientOnDispose(channelManager, config));
        return stream != null ? new FileStreamResult(stream, "video/mp4") : StatusCode(StatusCodes.Status404NotFound);
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