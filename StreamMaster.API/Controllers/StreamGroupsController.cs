using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using StreamMaster.Application.Common.Extensions;
using StreamMaster.Application.StreamGroups.CommandsOld;
using StreamMaster.Application.StreamGroups.Queries;
using StreamMaster.Domain.Authentication;
using StreamMaster.Domain.Helpers;
using StreamMaster.Domain.Repository;
using StreamMaster.Domain.Requests;
using StreamMaster.SchedulesDirect.Domain.Interfaces;

using System.Text;
using System.Web;

namespace StreamMaster.API.Controllers;

public class StreamGroupsController(IRepositoryWrapper Repository, IHttpContextAccessor httpContextAccessor, ISchedulesDirectDataService schedulesDirectDataService) : ApiControllerBase
{

    //private static int GenerateMediaSequence()
    //{
    //    DateTime epochStart = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    //    TimeSpan elapsedTime = SMDT.UtcNow - epochStart;
    //    int mediaSequence = (int)(elapsedTime.TotalSeconds / 10);

    //    return mediaSequence;
    //}

    [HttpPost]
    [Route("[action]")]
    public async Task<ActionResult> CreateStreamGroup(CreateStreamGroupRequest request)
    {

        await Mediator.Send(request).ConfigureAwait(false);
        return Ok();
    }

    [HttpDelete]
    [Route("[action]")]
    public async Task<ActionResult> DeleteStreamGroup(DeleteStreamGroupRequest request)
    {
        int? data = await Mediator.Send(request).ConfigureAwait(false);
        return data == null ? NotFound() : NoContent();
    }

    [Authorize(Policy = "SGLinks")]
    [HttpGet]
    [HttpHead]
    [Route("{encodedId}/auto/v{channelId}")]
    public async Task<ActionResult> GetVideoStreamStreamFromAuto(string encodedId, string channelId, CancellationToken cancellationToken)
    {
        int? streamGroupId = encodedId.DecodeValue128(Settings.ServerKey);
        if (streamGroupId == null)
        {
            return new NotFoundResult();
        }

        List<VideoStreamDto> videoStreams = await Repository.StreamGroupVideoStream.GetStreamGroupVideoStreams((int)streamGroupId);

        if (videoStreams.Count == 0)
        {
            return NotFound();
        }


        int epgNumber = EPGHelper.DummyId;

        foreach (VideoStreamDto videoStream in videoStreams)
        {
            string stationId;

            MxfService? service = null;

            if (string.IsNullOrEmpty(videoStream.User_Tvg_ID))
            {
                stationId = videoStream.User_Tvg_group;
            }
            else
            {
                if (EPGHelper.IsValidEPGId(videoStream.User_Tvg_ID))
                {
                    (epgNumber, stationId) = videoStream.User_Tvg_ID.ExtractEPGNumberAndStationId();
                    service = schedulesDirectDataService.GetService(stationId);
                }
                else
                {
                    stationId = videoStream.User_Tvg_ID;
                    string toTest = $"{stationId}-";
                    service = schedulesDirectDataService.AllServices.FirstOrDefault(a => a.StationId.StartsWith(toTest));
                }
            }

            string graceNote = service?.CallSign ?? stationId;

            string id = graceNote;
            if (Settings.M3UUseChnoForId)
            {
                id = videoStream.User_Tvg_chno.ToString();
            }
            if (id.Equals(channelId))
            {
                string url = httpContextAccessor.GetUrl();
                string videoUrl;
                if (HLSSettings.HLSM3U8Enable)
                {
                    videoUrl = $"{url}/api/stream/{videoStream.Id}.m3u8";
                    return Redirect(videoUrl);
                }

                string encodedName = HttpUtility.HtmlEncode(videoStream.User_Tvg_name).Trim()
                    .Replace("/", "")
                    .Replace(" ", "_");

                string encodedNumbers = ((int)streamGroupId).EncodeValues128(videoStream.Id, Settings.ServerKey);
                videoUrl = $"{url}/api/videostreams/stream/{encodedNumbers}/{encodedName}";

                return Redirect(videoUrl);
            }
        }

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

        int? streamGroupNumber = encodedId.DecodeValue128(Settings.ServerKey);
        if (streamGroupNumber == null)
        {
            return new NotFoundResult();
        }

        string data = await Mediator.Send(new GetStreamGroupM3U((int)streamGroupNumber, false)).ConfigureAwait(false);

        return new FileContentResult(Encoding.UTF8.GetBytes(data), "application/x-mpegURL")
        {
            FileDownloadName = $"m3u-{streamGroupNumber}.m3u"
        };
    }


    [HttpPatch]
    [Route("[action]")]
    public async Task<ActionResult> UpdateStreamGroup(UpdateStreamGroupRequest request)
    {
        StreamGroupDto? entity = await Mediator.Send(request).ConfigureAwait(false);
        return entity == null ? NotFound() : NoContent();
    }

    //private ObjectResult Status(ProxyStreamErrorCode proxyStreamErrorCode)
    //{
    //    return proxyStreamErrorCode switch
    //    {
    //        ProxyStreamErrorCode.FileNotFound => StatusCode(StatusCodes.Status500InternalServerError, "FFmpeg executable not found"),
    //        ProxyStreamErrorCode.IoError => StatusCode(StatusCodes.Status502BadGateway, "Error connecting to upstream server"),
    //        ProxyStreamErrorCode.UnknownError => StatusCode(StatusCodes.Status500InternalServerError, "An unknown error occurred"),
    //        ProxyStreamErrorCode.HttpRequestError => StatusCode(StatusCodes.Status500InternalServerError, "An Http Request Error occurred"),
    //        ProxyStreamErrorCode.ChannelManagerFinished => StatusCode(200, "Channel Manager Exited"),
    //        ProxyStreamErrorCode.HttpError => StatusCode(StatusCodes.Status500InternalServerError, "An Http Request Error occurred"),
    //        ProxyStreamErrorCode.DownloadError => StatusCode(StatusCodes.Status500InternalServerError, "Could not parse stream"),
    //        ProxyStreamErrorCode.MasterPlayListNotSupported => StatusCode(StatusCodes.Status500InternalServerError, "M3U8 Master playlist not supported"),
    //        _ => StatusCode(StatusCodes.Status500InternalServerError, "An unknown error occurred"),
    //    };
    //}

    //private string GetUrl()
    //{
    //    HttpRequest request = HttpContext.Request;
    //    string scheme = request.Scheme;
    //    HostString host = request.Host;
    //    PathString path = request.Path;
    //    QueryString queryString = request.QueryString;

    //    string url = $"{scheme}://{host}{path}{queryString}";

    //    return url;
    //}

    [HttpGet]
    [Route("[action]")]
    public async Task<ActionResult<string?>> GetStreamGroupVideoStreamUrl(string VideoStreamId)
    {
        string? res = await Mediator.Send(new GetStreamGroupVideoStreamUrl(VideoStreamId)).ConfigureAwait(false);
        return Ok(res);
    }
}