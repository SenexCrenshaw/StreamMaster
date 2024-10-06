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
    public async Task<ActionResult> GetVideoStreamStream(string encodedIds, string? name, CancellationToken cancellationToken)
    {
        (int? streamGroupId, int? streamGroupProfileId, int? smChannelId) = await streamGroupService.DecodeProfileIdSMChannelIdFromEncodedAsync(encodedIds);

        StreamResult streamResult = await videoService.GetStreamAsync(streamGroupId, streamGroupProfileId, smChannelId, cancellationToken);

        if (!string.IsNullOrEmpty(streamResult.RedirectUrl))
        {
            logger.LogInformation("Channel with ChannelId {channelId} {name} redirecting", smChannelId, streamResult.ClientConfiguration?.SMChannel.Name ?? name);
            return Redirect(streamResult.RedirectUrl);
        }

        if (streamResult.Stream == null || streamResult.ClientConfiguration == null)
        {
            logger.LogInformation("Channel with ChannelId {channelId} {name} failed", smChannelId, streamResult.ClientConfiguration?.SMChannel.Name ?? name);
            return StatusCode(StatusCodes.Status404NotFound);
        }

        streamResult.ClientConfiguration.ClientStopped += (sender, args) =>
        {
            //logger.LogInformation("Client {UniqueRequestId} {name} disposing", streamResult.ClientConfiguration.UniqueRequestId, streamResult.ClientConfiguration.SMChannel.Name);
            streamResult.ClientConfiguration.Response.CompleteAsync().Wait();
            //logger.LogInformation("Client {UniqueRequestId} {name} disposing next", streamResult.ClientConfiguration.UniqueRequestId, streamResult.ClientConfiguration.SMChannel.Name);
            _ = channelManager.RemoveClientAsync(streamResult.ClientConfiguration);
        };


        HttpContext.Response.RegisterForDispose(new UnregisterClientOnDispose(channelManager, streamResult.ClientConfiguration, logger));

        return streamResult.Stream != null ? new FileStreamResult(streamResult.Stream, "video/mp2t") { EnableRangeProcessing = false, FileDownloadName = $"{encodedIds}.ts" } : StatusCode(StatusCodes.Status404NotFound);
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