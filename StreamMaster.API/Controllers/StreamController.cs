using AutoMapper;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using StreamMaster.Domain.Repository;
using StreamMaster.Streams.Domain.Interfaces;
using StreamMaster.Streams.Domain.Models;
using StreamMaster.Streams.Streams;


namespace StreamMaster.API.Controllers
{
    public class StreamController(ILogger<StreamController> logger, IMapper mapper, IStreamTracker streamTracker, ILogger<FFMPEGRunner> FFMPEGRunnerlogger, IChannelService channelService, IAccessTracker accessTracker, IHLSManager hLsManager, IRepositoryWrapper repositoryWrapper) : ApiControllerBase
    {

        [Authorize(Policy = "SGLinks")]
        [HttpGet]
        [HttpHead]
        [Route("{SMChannelId}.m3u8")]
        public async Task<ActionResult> GetM3U8(int SMChannelId, CancellationToken cancellationToken)
        {
            SMChannel? smChannel = await repositoryWrapper.SMChannel.FirstOrDefaultAsync(a => a.Id == SMChannelId);
            if (smChannel is null)
            {
                return NotFound();
            }

            IChannelStatus? channelStatus = await channelService.SetupChannel(smChannel: mapper.Map<SMChannelDto>(smChannel));

            if (channelStatus == null || channelStatus.SMStream == null)
            {
                return NotFound();
            }

            await hLsManager.GetOrAdd(channelStatus.SMStream);

            int timeOut = HLSSettings.HLSM3U8CreationTimeOutInSeconds;
            string m3u8File = Path.Combine(BuildInfo.HLSOutputFolder, channelStatus.SMStream.Id, $"index.m3u8");

            if (!streamTracker.HasStream(channelStatus.SMStream.Id))
            {
                if (!await FileUtil.WaitForFileAsync(m3u8File, timeOut, 100, cancellationToken))
                {
                    logger.LogWarning("HLS segment timeout {FileName}, exiting", m3u8File);
                    hLsManager.Stop(channelStatus.SMStream.Id);
                    return NotFound();
                }

                string tsFile = Path.Combine(BuildInfo.HLSOutputFolder, channelStatus.SMStream.Id, $"2.ts");

                if (!await FileUtil.WaitForFileAsync(tsFile, timeOut, 100, cancellationToken))
                {
                    logger.LogWarning("TS segment timeout {FileName}, exiting", tsFile);
                    hLsManager.Stop(channelStatus.SMStream.Id);
                    return NotFound();
                }

                if (streamTracker.AddStream(channelStatus.SMStream.Id))
                {
                    //timeOut = HLSSettings.HLSM3U8CreationTimeOutInSeconds;
                }
            }

            accessTracker.UpdateAccessTime(channelStatus.SMStream.Id, TimeSpan.FromSeconds(HLSSettings.HLSM3U8ReadTimeOutInSeconds));

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
        public IActionResult GetVideoStream(string videoStreamId, int num, CancellationToken cancellationToken)
        {
            string tsFile = Path.Combine(BuildInfo.HLSOutputFolder, videoStreamId, $"{num}.ts");
            if (!System.IO.File.Exists(tsFile))
            {
                return NotFound();
            }
            try
            {

                accessTracker.UpdateAccessTime(videoStreamId, TimeSpan.FromSeconds(HLSSettings.HLSTSReadTimeOutInSeconds));

                HttpContext.Response.Headers.Connection = "close";
                HttpContext.Response.Headers.AccessControlAllowOrigin = "*";
                //HttpContext.Response.Headers.CacheControl = "no-cache";


                FileStream stream = new(tsFile, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true);
                return new FileStreamResult(stream, "video/mp2t");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error streaming video file {FileName}", tsFile);
                return StatusCode(500, "Error streaming video");
            }
        }

        [Authorize(Policy = "SGLinks")]
        [HttpGet]
        [HttpHead]
        [Route("{videoStreamId}.mp4")]
        public async Task<ActionResult> GetVideoStreamMP4(string videoStreamId, CancellationToken cancellationToken)
        {
            SMStreamDto? smStream = repositoryWrapper.SMStream.GetSMStream(videoStreamId);
            if (smStream is null)
            {
                return NotFound();
            }

            try
            {

                HttpRequest request = HttpContext.Request;
                //string url = GetUrl(request);
                string url = "http://127.0.0.1:7095/api/stream/" + videoStreamId + ".m3u8";

                logger.LogInformation("Adding MP4Handler for {name}", smStream.Name);
                FFMPEGRunner ffmpegRunner = new(FFMPEGRunnerlogger, intsettings, inthlssettings);
                ffmpegRunner.ProcessExited += (sender, args) =>
                {
                    logger.LogInformation("MP4Handler Process Exited for {Name} with exit code {ExitCode}", smStream.Name, args.ExitCode);
                };
                (Stream? stream, int processId, ProxyStreamError? error) = await ffmpegRunner.CreateFFMpegStream(url, smStream.Name);

                return stream != null ? new FileStreamResult(stream, "video/mp4") : StatusCode(StatusCodes.Status404NotFound);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error streaming video file {videoStreamId}", videoStreamId);
                return StatusCode(500, "Error streaming video");
            }
        }

        private class UnregisterClientOnDispose(IChannelService _channelService, IChannelStatus channelStatus, int smChannelId) : IDisposable
        {
            private readonly IChannelService _channelService = _channelService;
            private readonly int smChannelId = smChannelId;
            private readonly IChannelStatus _channelStatus = channelStatus;

            public void Dispose()
            {
                IChannelStatus? status = _channelService.GetChannelStatusFromSMChannelId(smChannelId);

                //int count = _channelService.GetChannelStatusesFromSMChannelId(smChannelId);
                if (status == null || status.ClientCount == 0)
                {
                    _channelService.UnRegisterChannel(smChannelId);
                }
            }
        }
        private string GetUrl(HttpRequest request)
        {

            return $"{request.Scheme}://{request.Host}";
        }
    }


}
