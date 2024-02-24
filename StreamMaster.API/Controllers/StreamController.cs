using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;



namespace StreamMaster.API.Controllers
{
    public class StreamController(ILogger<StreamController> logger, IVideoStreamService videoStreamService, IAccessTracker accessTracker, IHLSManager hLsManager, IMemoryCache memoryCache) : ApiControllerBase
    {
        [Authorize(Policy = "SGLinks")]
        [HttpGet]
        [HttpHead]
        [Route("{videoStreamId}.m3u8")]
        public async Task<ActionResult> GetM3U8(string videoStreamId, CancellationToken cancellationToken)
        {
            VideoStreamDto? videoStreamDto = await videoStreamService.GetVideoStreamDtoAsync(videoStreamId, cancellationToken);
            if (videoStreamDto is null)
            {
                return NotFound();
            }

            hLsManager.GetOrAdd(videoStreamDto);

            string m3u8File = Path.Combine(BuildInfo.HLSOutputFolder, videoStreamId, $"index.m3u8");
            Setting setting = memoryCache.GetSetting();

            if (!await FileUtil.WaitForFileAsync(m3u8File, setting.HLS.HLSM3U8CreationTimeOutInSeconds, 100, cancellationToken))
            {
                logger.LogWarning("HLS file not found {FileName}, exiting", m3u8File);
                hLsManager.Stop(videoStreamId);
                return NotFound();
            }

            accessTracker.UpdateAccessTime(videoStreamId, TimeSpan.FromSeconds(setting.HLS.HLSM3U8ReadTimeOutInSeconds));

            HttpContext.Response.Headers.Connection = "close";
            HttpContext.Response.Headers.AccessControlAllowOrigin = "*";
            HttpContext.Response.Headers.AccessControlExposeHeaders = "Content-Length";
            HttpContext.Response.Headers.CacheControl = "no-cache";

            string m3u8Content = await System.IO.File.ReadAllTextAsync(m3u8File, cancellationToken);
            return Content(m3u8Content, "application/vnd.apple.mpegurl");

        }


        [Authorize(Policy = "SGLinks")]
        [HttpGet]
        [HttpHead]
        [Route("{videoStreamId}/{num}.ts")]
        public ActionResult GetVideoStream(string videoStreamId, int num, CancellationToken cancellationToken)
        {
            string tsFile = Path.Combine(BuildInfo.HLSOutputFolder, videoStreamId, $"{num}.ts");
            if (!System.IO.File.Exists(tsFile))
            {
                return NotFound();
            }
            try
            {
                Setting setting = memoryCache.GetSetting();
                accessTracker.UpdateAccessTime(videoStreamId, TimeSpan.FromSeconds(setting.HLS.HLSTSReadTimeOutInSeconds));

                HttpContext.Response.Headers.Connection = "close";
                HttpContext.Response.Headers.AccessControlAllowOrigin = "*";
                HttpContext.Response.Headers.CacheControl = "no-cache";


                FileStream stream = new(tsFile, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true);
                return new FileStreamResult(stream, "video/mp2t");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error streaming video file {FileName}", tsFile);
                return StatusCode(500, "Error streaming video");
            }
        }
    }
}
