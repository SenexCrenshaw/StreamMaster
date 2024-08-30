using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace StreamMaster.API.Controllers;

[V1ApiController("api/[controller]")]
public class VideoStreamsController(ILogger<VideoStreamsController> logger, IVideoService videoService, IChannelManager channelManager, IStreamGroupService streamGroupService)
     : ControllerBase
{
    [Authorize(Policy = "SGLinks")]
    [HttpGet]
    [HttpHead]
    [Route("stream/{encodedIds}")]
    [Route("stream/{encodedIds}.mp4")]
    [Route("stream/{encodedIds}.ts")]
    [Route("stream/{encodedIds}/{name?}")]
#pragma warning disable RCS1163 // Unused parameter
    public async Task<ActionResult> GetVideoStreamStream(string encodedIds, string? name, CancellationToken cancellationToken)
#pragma warning restore RCS1163 // Unused parameter
    {
        (int? streamGroupId, int? streamGroupProfileId, int? smChannelId) = await streamGroupService.DecodeProfileIdSMChannelIdFromEncodedAsync(encodedIds);

        (Stream? stream, IClientConfiguration? clientConfiguration, string? RedirectUrl) = await videoService.GetStreamAsync(streamGroupId, streamGroupProfileId, smChannelId, cancellationToken);

        if (!string.IsNullOrEmpty(RedirectUrl))
        {
            logger.LogInformation("Channel with ChannelId {channelId} {name} redirecting", smChannelId, clientConfiguration?.SMChannel.Name ?? "");
            return Redirect(RedirectUrl);
        }

        if (stream == null || clientConfiguration == null)
        {
            logger.LogInformation("Channel with ChannelId {channelId} {name} failed", smChannelId, clientConfiguration?.SMChannel.Name ?? "");
            return StatusCode(StatusCodes.Status404NotFound);
        }

        HttpContext.Response.RegisterForDispose(new UnregisterClientOnDispose(channelManager, clientConfiguration, logger));

        return stream != null ? new FileStreamResult(stream, "video/mp4") { EnableRangeProcessing = false } : StatusCode(StatusCodes.Status404NotFound);
    }
    private class UnregisterClientOnDispose(IChannelManager channelManager, IClientConfiguration config, ILogger logger) : IDisposable
    {
        private readonly IChannelManager _channelManager = channelManager;
        private readonly IClientConfiguration _config = config;

        public void Dispose()
        {
            logger.LogInformation("Client {UniqueRequestId} {name} disposing", _config.UniqueRequestId, _config.SMChannel.Name);
            _config.Response.CompleteAsync().Wait();

            logger.LogInformation("Client {UniqueRequestId} {name} disposing next", _config.UniqueRequestId, _config.SMChannel.Name);
            _ = _channelManager.RemoveClientAsync(_config);
        }
    }
}