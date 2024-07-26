using AutoMapper;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using System.Diagnostics;

namespace StreamMaster.API.Controllers;
[V1ApiController("m")]
public class MController(ILogger<MController> logger, ICryptoService cryptoService, IOptionsMonitor<HLSSettings> intHLSSettings, IAccessTracker accessTracker, IHLSManager hlsManager, IRepositoryWrapper repositoryWrapper, IMapper mapper) : Controller
{
    [Authorize(Policy = "SGLinks")]
    [HttpGet]
    [HttpHead]
    [Route("{encodedString}.m3u8")]
    public async Task<ActionResult> GetM3U8(string encodedString, CancellationToken cancellationToken)
    {
        //await HttpContext.LogRequestDetailsAsync(logger);

        (int? streamGroupId, int? SMChannelId) = cryptoService.DecodeSMChannelIdFromEncoded(encodedString);

        SMChannel? smChannel = await repositoryWrapper.SMChannel.FirstOrDefaultAsync(a => a.Id == SMChannelId, cancellationToken: cancellationToken);
        if (smChannel is null)
        {
            return NotFound();
        }

        SMChannelDto smChannelDto = mapper.Map<SMChannelDto>(smChannel);

        IM3U8ChannelStatus? channelStatus = await hlsManager.TryAddAsync(smChannelDto, CancellationToken.None);
        if (channelStatus == null || channelStatus.SMStreamInfo == null || string.IsNullOrEmpty(channelStatus.SMStreamInfo.Url))
        {
            return NotFound();
        }

        HLSSettings hlsSettings = intHLSSettings.CurrentValue;
        StreamAccessInfo streamAccessInfo = accessTracker.UpdateAccessTime(channelStatus.SMStreamInfo.Id + ".m3u8", channelStatus.SMStreamInfo.Id, TimeSpan.FromSeconds(hlsSettings.HLSM3U8ReadTimeOutInSeconds));
        if (streamAccessInfo.MillisecondsSinceLastUpdate > 0)
        {
            logger.LogInformation("M3U8 last update {key} {Milliseconds}ms {HLSM3U8ReadTimeOutInSeconds}", streamAccessInfo.Key, streamAccessInfo.MillisecondsSinceLastUpdate, hlsSettings.HLSM3U8ReadTimeOutInSeconds);
        }

        HttpContext.Response.Headers.Connection = "close";
        HttpContext.Response.Headers.AccessControlAllowOrigin = "*";
        HttpContext.Response.Headers.AccessControlExposeHeaders = "Content-Length";
        HttpContext.Response.Headers.CacheControl = "no-cache";

        string m3u8Content = await System.IO.File.ReadAllTextAsync(channelStatus.M3U8File, cancellationToken);
        return Content(m3u8Content, "application/vnd.apple.mpegurl");
    }

    [Authorize(Policy = "SGLinks")]
    [HttpGet]
    [HttpHead]
    [Route("{encodedString}/{num}.ts")]
    public async Task<IActionResult> GetVideoStream(string encodedString, int num, CancellationToken cancellationToken)
    {
        string? smStreamId = cryptoService.DecodeString(encodedString);
        if (smStreamId == null)
        {
            logger.LogError("Encode error");
            return NotFound();
        }

        string M3U8Directory = M3U8ChannelStatus.GetDirectory(smStreamId);

        HLSSettings hlsSettings = intHLSSettings.CurrentValue;
        int segment = hlsSettings.HLSSegmentCount / 2;
        int timeout = hlsSettings.HLSSegmentCount / 2 * hlsSettings.HLSSegmentDurationInSeconds;
        string tsFile = Path.Combine(M3U8Directory, $"{num}.ts");
        if (!await FileUtil.WaitForFileAsync(tsFile, timeout, 50, cancellationToken).ConfigureAwait(false))
        {
            Debug.WriteLine("File not found: {0}", tsFile);
            return NotFound();
        }

        try
        {
            StreamAccessInfo streamAccessInfo = accessTracker.UpdateAccessTime(smStreamId, smStreamId, TimeSpan.FromSeconds(hlsSettings.HLSTSReadTimeOutInSeconds));
            if (streamAccessInfo.MillisecondsSinceLastUpdate > 0)
            {
                logger.LogInformation("TS last update took {key} {Milliseconds}ms {HLSM3U8ReadTimeOutInSeconds}", streamAccessInfo.Key, streamAccessInfo.MillisecondsSinceLastUpdate, hlsSettings.HLSTSReadTimeOutInSeconds);
            }

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
