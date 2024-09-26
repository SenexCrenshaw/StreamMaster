using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


using StreamMaster.Application.StreamGroups.Queries;
using StreamMaster.Domain.Crypto;

using System.Text;

namespace StreamMaster.Application.StreamGroups.Controllers;

public partial class StreamGroupsController
{

    [HttpGet]
    [AllowAnonymous]
    [Route("{encodedId}")]
    [Route("{encodedId}/capability")]
    [Route("{encodedId}/device.xml")]
    public async Task<IActionResult> GetStreamGroupCapability(string encodedId)
    {
        int? streamGroupProfileId = CryptoService.DecodeInt(encodedId);
        if (!streamGroupProfileId.HasValue)
        {
            return new NotFoundResult();
        }

        if (HttpContext.Request == null)
        {
            return NotFound();
        }

        string xml = await StreamGroupService.GetStreamGroupCapability(streamGroupProfileId.Value, HttpContext.Request).ConfigureAwait(false);

        return new ContentResult
        {
            Content = xml,
            ContentType = "application/xml",
            StatusCode = 200
        };
    }

    [HttpGet]
    [Authorize(Policy = "SGLinks")]
    [Route("{encodedId}/discover.json")]
    public async Task<IActionResult> GetStreamGroupDiscover(string encodedId)
    {
        HttpRequest a = HttpContext.Request;
        int? streamGroupProfileId = CryptoService.DecodeInt(encodedId);
        if (!streamGroupProfileId.HasValue)
        {
            return new NotFoundResult();
        }

        if (HttpContext.Request == null)
        {
            return NotFound();
        }

        string json = await StreamGroupService.GetStreamGroupDiscover(streamGroupProfileId.Value, HttpContext.Request).ConfigureAwait(false);

        return new ContentResult
        {
            Content = json,
            ContentType = "text/json",
            StatusCode = 200
        };
    }

    [Authorize(Policy = "SGLinks")]
    [HttpGet]
    [Route("{encodedId}/epg.xml")]
    public async Task<IActionResult> GetStreamGroupEPG(string encodedId)
    {
        int? streamGroupProfileId = CryptoService.DecodeInt(encodedId);
        if (!streamGroupProfileId.HasValue)
        {
            return new NotFoundResult();
        }

        StreamGroup? sg = await StreamGroupService.GetStreamGroupFromSGProfileIdAsync(streamGroupProfileId.Value).ConfigureAwait(false);
        if (sg == null)
        {
            return new NotFoundResult();
        }

        string xml = await Sender.Send(new GetStreamGroupEPG(streamGroupProfileId.Value)).ConfigureAwait(false);
        return new FileContentResult(Encoding.UTF8.GetBytes(xml), "application/xml")
        {
            FileDownloadName = $"epg-{sg.Name.ToCleanFileString()}.xml"
        };
    }


    [HttpGet]
    [Authorize(Policy = "SGLinks")]
    [Route("{encodedId}/lineup.json")]
    public async Task<IActionResult> GetStreamGroupLineup(string encodedId)
    {
        int? streamGroupProfileId = CryptoService.DecodeInt(encodedId);
        if (!streamGroupProfileId.HasValue)
        {
            return new NotFoundResult();
        }

        if (HttpContext.Request == null)
        {
            return NotFound();
        }

        string json = await StreamGroupService.GetStreamGroupLineup(streamGroupProfileId.Value, HttpContext.Request, true).ConfigureAwait(false);
        return new ContentResult
        {
            Content = json,
            ContentType = "application/json",
            StatusCode = 200
        };
    }

    [HttpGet]
    [Authorize(Policy = "SGLinks")]
    [Route("{encodedId}/lineup_status.json")]
    public IActionResult GetStreamGroupLineupStatus(string encodedId)
    {
        int? streamGroupProfileId = CryptoService.DecodeInt(encodedId);
        if (!streamGroupProfileId.HasValue)
        {
            return new NotFoundResult();
        }

        string json = StreamGroupService.GetStreamGroupLineupStatus();
        return new ContentResult
        {
            Content = json,
            ContentType = "text/json",
            StatusCode = 200
        };
    }

    [Authorize(Policy = "SGLinks")]
    [HttpGet]
    [Route("{encodedId}/m3u.m3u")]
    public async Task<IActionResult> GetStreamGroupM3U(string encodedId)
    {
        int? streamGroupProfileId = CryptoService.DecodeInt(encodedId);
        if (!streamGroupProfileId.HasValue)
        {
            return new NotFoundResult();
        }

        StreamGroup? sg = await StreamGroupService.GetStreamGroupFromSGProfileIdAsync(streamGroupProfileId.Value).ConfigureAwait(false);
        if (sg == null)
        {
            return new NotFoundResult();
        }

        string data = await Sender.Send(new GetStreamGroupM3U(streamGroupProfileId.Value, false)).ConfigureAwait(false);

        return new FileContentResult(Encoding.UTF8.GetBytes(data), "application/x-mpegURL")
        {
            FileDownloadName = $"m3u-{streamGroupProfileId.Value}.m3u"
        };
    }

    [Authorize(Policy = "SGLinks")]
    [HttpGet]
    [Route("{encodedId}/auto/v{channelNumber}")]
    public async Task<IActionResult> GetAutoStream(string encodedId, string channelNumber)
    {
        int? streamGroupProfileId = CryptoService.DecodeInt(encodedId);
        if (!streamGroupProfileId.HasValue)
        {
            return new NotFoundResult();
        }

        StreamGroup? streamGroup = await StreamGroupService.GetStreamGroupFromSGProfileIdAsync(streamGroupProfileId.Value).ConfigureAwait(false);
        if (streamGroup == null)
        {
            return new NotFoundResult();
        }

        (List<VideoStreamConfig> videoStreamConfigs, StreamGroupProfile streamGroupProfile) = await StreamGroupService.GetStreamGroupVideoConfigs(streamGroupProfileId.Value);

        if (videoStreamConfigs is null || streamGroupProfile is null)
        {
            return new NotFoundResult();
        }
        VideoStreamConfig? videoStreamConfig = videoStreamConfigs.FirstOrDefault(a => a.ChannelNumber.ToString() == channelNumber);

        if (videoStreamConfig == null)
        {
            return new NotFoundResult();
        }

        string url = HttpContext.Request.GetUrl();
        string? encodedString = StreamGroupService.EncodeStreamGroupIdProfileIdChannelId(streamGroup, streamGroupProfileId.Value, videoStreamConfig.Id);
        string cleanName = videoStreamConfig.Name.ToCleanFileString();
        string videoUrl = $"{url}/api/videostreams/stream/{encodedString}/{cleanName}";

        return Redirect(videoUrl);
    }


}