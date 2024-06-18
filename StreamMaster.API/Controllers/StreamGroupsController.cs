using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using StreamMaster.Application.StreamGroups.Queries;
using StreamMaster.Application.StreamGroups.QueriesOld;
using StreamMaster.Domain.Authentication;
using StreamMaster.Domain.Requests;

using System.Text;

namespace StreamMaster.API.Controllers;

public class StreamGroupsController()
    : ApiControllerBase
{

    //private static int GenerateMediaSequence()
    //{
    //    DateTime epochStart = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    //    TimeSpan elapsedTime = SMDT.UtcNow - epochStart;
    //    int mediaSequence = (int)(elapsedTime.TotalSeconds / 10);

    //    return mediaSequence;
    //}

    //[HttpPost]
    //[Route("[action]")]
    //public async Task<ActionResult> CreateStreamGroup(CreateStreamGroupRequest request)
    //{

    //    await Mediator.Send(request).ConfigureAwait(false);
    //    return Ok();
    //}


    [Authorize(Policy = "SGLinks")]
    [HttpGet]
    [HttpHead]
    [Route("{encodedId}/auto/v{channelId}")]
    public ActionResult GetVideoStreamStreamFromAuto(string encodedId, string channelId, CancellationToken cancellationToken)
    {
        int? streamGroupId = encodedId.DecodeValue128(Settings.ServerKey);
        if (streamGroupId == null)
        {
            return new NotFoundResult();
        }

        //List<VideoStreamDto> videoStreams = await Repository.StreamGroupVideoStream.GetStreamGroupVideoStreams((int)streamGroupId);

        //if (videoStreams.Count == 0)
        //{
        //    return NotFound();
        //}


        //int epgNumber = EPGHelper.DummyId;

        //foreach (VideoStreamDto videoStream in videoStreams)
        //{
        //    string stationId;

        //    MxfService? service = null;

        //    if (string.IsNullOrEmpty(smChannelDto.EPGId))
        //    {
        //        stationId = videoStream.User_Tvg_group;
        //    }
        //    else
        //    {
        //        if (EPGHelper.IsValidEPGId(smChannelDto.EPGId))
        //        {
        //            (epgNumber, stationId) = smChannelDto.EPGId.ExtractEPGNumberAndStationId();
        //            service = schedulesDirectDataService.GetService(stationId);
        //        }
        //        else
        //        {
        //            stationId = smChannelDto.EPGId;
        //            string toTest = $"{stationId}-";
        //            service = schedulesDirectDataService.AllServices.FirstOrDefault(a => a.StationId.StartsWith(toTest));
        //        }
        //    }

        //    string graceNote = service?.CallSign ?? stationId;

        //    string id = graceNote;
        //    if (Settings.M3UUseChnoForId)
        //    {
        //        id = smChannelDto.ChannelNumber.ToString();
        //    }
        //    if (id.Equals(channelId))
        //    {
        //        string url = httpContextAccessor.GetUrl();
        //        string videoUrl;
        //        if (HLSSettings.HLSM3U8Enable)
        //        {
        //            videoUrl = $"{url}/api/stream/{videoStream.Id}.m3u8";
        //            return Redirect(videoUrl);
        //        }

        //        string encodedName = HttpUtility.HtmlEncode(smChannelDto.Name).Trim()
        //            .Replace("/", "")
        //            .Replace(" ", "_");

        //        string encodedNumbers = ((int)streamGroupId).EncodeValues128(videoStream.Id, Settings.ServerKey);
        //        videoUrl = $"{url}/api/videostreams/stream/{encodedNumbers}/{encodedName}";

        //        return Redirect(videoUrl);
        //    }
        //}

        return NotFound();
    }

    [HttpGet]
    [AllowAnonymous]
    [Route("{encodedId}")]
    [Route("{encodedId}/capability")]
    [Route("{encodedId}/device.xml")]
    public async Task<IActionResult> GetStreamGroupCapability(string encodedId)
    {

        int? streamGroupId = encodedId.DecodeValue128(Settings.ServerKey);
        if (streamGroupId == null)
        {
            return new NotFoundResult();
        }

        string xml = await Mediator.Send(new GetStreamGroupCapability((int)streamGroupId)).ConfigureAwait(false);
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

        int? streamGroupNumber = encodedId.DecodeValue128(Settings.ServerKey);
        if (streamGroupNumber == null)
        {
            return new NotFoundResult();
        }

        string json = await Mediator.Send(new GetStreamGroupDiscover((int)streamGroupNumber)).ConfigureAwait(false);
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

        int? streamGroupNumber = encodedId.DecodeValue128(Settings.ServerKey);
        if (streamGroupNumber == null)
        {
            return new NotFoundResult();
        }

        string xml = await Mediator.Send(new GetStreamGroupEPG((int)streamGroupNumber)).ConfigureAwait(false);
        return new FileContentResult(Encoding.UTF8.GetBytes(xml), "application/xml")
        {
            FileDownloadName = $"epg-{streamGroupNumber}.xml"
        };
    }


    [HttpGet]
    [Authorize(Policy = "SGLinks")]
    [Route("{encodedId}/lineup.json")]
    public async Task<IActionResult> GetStreamGroupLineup(string encodedId)
    {

        int? streamGroupNumber = encodedId.DecodeValue128(Settings.ServerKey);
        if (streamGroupNumber == null)
        {
            return new NotFoundResult();
        }

        string json = await Mediator.Send(new GetStreamGroupLineup((int)streamGroupNumber)).ConfigureAwait(false);
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
    public async Task<IActionResult> GetStreamGroupLineupStatus(string encodedId)
    {

        int? streamGroupNumber = encodedId.DecodeValue128(Settings.ServerKey);
        if (streamGroupNumber == null)
        {
            return new NotFoundResult();
        }
        string json = await Mediator.Send(new GetStreamGroupLineupStatus((int)streamGroupNumber)).ConfigureAwait(false);
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

        (int? streamGroupId, int? streamGroupProfileId) = encodedId.DecodeValues128(Settings.ServerKey);
        if (!streamGroupId.HasValue || !streamGroupProfileId.HasValue)
        {
            return new NotFoundResult();
        }

        string data = await Mediator.Send(new GetStreamGroupM3U(streamGroupId.Value, streamGroupProfileId.Value)).ConfigureAwait(false);

        return new FileContentResult(Encoding.UTF8.GetBytes(data), "application/x-mpegURL")
        {
            FileDownloadName = $"m3u-{streamGroupId.Value}.m3u"
        };
    }


    [HttpGet]
    [Route("[action]")]
    public ActionResult<string?> GetStreamGroupVideoStreamUrl(string VideoStreamId)
    {
        //string? res = await Mediator.Send(new GetStreamGroupVideoStreamUrl(VideoStreamId)).ConfigureAwait(false);
        //return Ok(res);
        return Ok("hello");
    }
}