using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace StreamMaster.API.Controllers;


[V1ApiController("v")]
public class VsController(ILogger<VsController> logger, IVideoService videoService, IStreamGroupService streamGroupService, IChannelManager channelManager) : Controller
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
        int streamGroupId = await streamGroupService.GetStreamGroupIdFromSGProfileIdAsync(streamGroupProfileId).ConfigureAwait(false);

        (Stream? stream, IClientConfiguration? clientConfiguration, string? RedirectUrl) = await videoService.GetStreamAsync(streamGroupId, streamGroupProfileId, smChannelId, cancellationToken);

        if (!string.IsNullOrEmpty(RedirectUrl))
        {
            logger.LogInformation("Channel with ChannelId {channelId} redirecting", smChannelId);
            return Redirect(RedirectUrl);
        }

        if (stream == null || clientConfiguration == null)
        {
            return StatusCode(StatusCodes.Status404NotFound);
        }

        HttpContext.Response.RegisterForDispose(new UnregisterClientOnDispose(channelManager, clientConfiguration));

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