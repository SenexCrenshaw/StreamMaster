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

        StreamResult streamResult = await videoService.GetStreamAsync(streamGroupId, streamGroupProfileId, smChannelId, cancellationToken);

        if (!string.IsNullOrEmpty(streamResult.RedirectUrl))
        {
            logger.LogInformation("Channel with ChannelId {channelId} redirecting", smChannelId);
            return Redirect(streamResult.RedirectUrl);
        }

        if (streamResult.Stream == null || streamResult.ClientConfiguration == null)
        {
            logger.LogInformation("Channel with ChannelId {channelId} failed", smChannelId);
            return StatusCode(StatusCodes.Status404NotFound);
        }

        HttpContext.Response.RegisterForDispose(new UnregisterClientOnDispose(channelManager, streamResult.ClientConfiguration));

        streamResult.ClientConfiguration.ClientStopped += (sender, args) =>
        {
            _ = channelManager.RemoveClientAsync(streamResult.ClientConfiguration);
        };

        return streamResult.Stream != null ? new FileStreamResult(streamResult.Stream, "video/mp2t") { EnableRangeProcessing = false, FileDownloadName = $"{smChannelId}.ts" } : StatusCode(StatusCodes.Status404NotFound);
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