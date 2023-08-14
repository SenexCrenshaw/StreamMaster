using AutoMapper;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using StreamMasterApplication.StreamGroups;
using StreamMasterApplication.StreamGroups.Commands;
using StreamMasterApplication.StreamGroups.Queries;

using StreamMasterDomain.Authentication;
using StreamMasterDomain.Common;
using StreamMasterDomain.Dto;
using StreamMasterDomain.Pagination;

using System.Text;

namespace StreamMasterAPI.Controllers;

public class StreamGroupsController : ApiControllerBase, IStreamGroupController
{
    private readonly ILogger<StreamGroupsController> _logger;

    public StreamGroupsController(ILogger<StreamGroupsController> logger, IMapper mapper)
    {
        _logger = logger;
        _mapper = mapper;
    }

    private readonly IMapper _mapper;

    public static int GenerateMediaSequence()
    {
        DateTime epochStart = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        TimeSpan elapsedTime = DateTime.UtcNow - epochStart;
        int mediaSequence = (int)(elapsedTime.TotalSeconds / 10);

        return mediaSequence;
    }

    [HttpPost]
    [Route("[action]")]
    public async Task<ActionResult> AddStreamGroup(AddStreamGroupRequest request)
    {
        StreamGroupDto? entity = await Mediator.Send(request).ConfigureAwait(false);
        return entity != null ? CreatedAtAction(nameof(GetStreamGroup), new { id = entity.Id }, entity) : Ok();
    }

    [HttpDelete]
    [Route("[action]")]
    public async Task<ActionResult> DeleteStreamGroup(DeleteStreamGroupRequest request)
    {
        int? data = await Mediator.Send(request).ConfigureAwait(false);
        return data == null ? NotFound() : NoContent();
    }

    [HttpGet]
    [Route("[action]/{id}")]
    public async Task<ActionResult<StreamGroupDto>> GetStreamGroup(int id)
    {
        StreamGroupDto? data = await Mediator.Send(new GetStreamGroup(id)).ConfigureAwait(false);

        return data != null ? (ActionResult<StreamGroupDto>)data : (ActionResult<StreamGroupDto>)NotFound();
    }

    [Authorize(Policy = "SGLinks")]
    [HttpGet]
    [Route("[action]/{StreamGroupNumber}")]
    public async Task<ActionResult<StreamGroupDto>> GetStreamGroupByStreamNumber(int StreamGroupNumber)
    {
        StreamGroupDto? data = await Mediator.Send(new GetStreamGroupByStreamNumber(StreamGroupNumber)).ConfigureAwait(false);

        return data != null ? (ActionResult<StreamGroupDto>)data : (ActionResult<StreamGroupDto>)NotFound();
    }

    [HttpGet]
    [AllowAnonymous]
    [Route("{encodedId}")]
    [Route("{encodedId}/capability")]
    [Route("{encodedId}/device.xml")]
    public async Task<IActionResult> GetStreamGroupCapability(string encodedId)
    {
        int? streamGroupNumber = encodedId.DecodeValue128(_setting.ServerKey);
        if (streamGroupNumber == null)
        {
            return new NotFoundResult();
        }

        string xml = await Mediator.Send(new GetStreamGroupCapability((int)streamGroupNumber)).ConfigureAwait(false);
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
        int? streamGroupNumber = encodedId.DecodeValue128(_setting.ServerKey);
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
        int? streamGroupNumber = encodedId.DecodeValue128(_setting.ServerKey);
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
    [Route("{StreamGroupNumber}/epgGuide.json")]
    public async Task<ActionResult<EPGGuide>> GetStreamGroupEPGForGuide(int StreamGroupNumber)
    {
        EPGGuide data = await Mediator.Send(new GetStreamGroupEPGForGuide(StreamGroupNumber)).ConfigureAwait(false);
        return data != null ? (ActionResult<EPGGuide>)data : (ActionResult<EPGGuide>)NotFound();
    }

    [HttpGet]
    [Authorize(Policy = "SGLinks")]
    [Route("{encodedId}/lineup.json")]
    public async Task<IActionResult> GetStreamGroupLineUp(string encodedId)
    {
        int? streamGroupNumber = encodedId.DecodeValue128(_setting.ServerKey);
        if (streamGroupNumber == null)
        {
            return new NotFoundResult();
        }

        string json = await Mediator.Send(new GetStreamGroupLineUp((int)streamGroupNumber)).ConfigureAwait(false);
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
    public async Task<IActionResult> GetStreamGroupLineUpStatus(string encodedId)
    {
        int? streamGroupNumber = encodedId.DecodeValue128(_setting.ServerKey);
        if (streamGroupNumber == null)
        {
            return new NotFoundResult();
        }
        string json = await Mediator.Send(new GetStreamGroupLineUpStatus((int)streamGroupNumber)).ConfigureAwait(false);
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
        int? streamGroupNumber = encodedId.DecodeValue128(_setting.ServerKey);
        if (streamGroupNumber == null)
        {
            return new NotFoundResult();
        }

        string data = await Mediator.Send(new GetStreamGroupM3U((int)streamGroupNumber)).ConfigureAwait(false);

        return new FileContentResult(Encoding.UTF8.GetBytes(data), "application/x-mpegURL")
        {
            FileDownloadName = $"m3u-{streamGroupNumber}.m3u"
        };
    }

    [HttpGet]
    public async Task<ActionResult<PagedResponse<StreamGroupDto>>> GetStreamGroups([FromQuery] StreamGroupParameters parameters)
    {
        PagedResponse<StreamGroupDto> res = await Mediator.Send(new GetStreamGroups(parameters)).ConfigureAwait(false);
        return Ok(res);
    }

    //[HttpGet]
    //[Route("{streamGroupNumber}/stream/{streamId}.m3u8")]
    //[ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    //[ProducesResponseType(StatusCodes.Status404NotFound)]
    //public IActionResult GetStreamM3U8(int streamGroupNumber, int streamId, CancellationToken cancellationToken)
    //{
    //    var settings = FileUtil.GetSetting();

    // if (settings.StreamingProxyType == StreamingProxyTypes.None) {
    // _logger.LogCritical("Cannot get M3U8 Playlist for stream {streamId} as
    // proxy is set to none", streamId); return NotFound(); }

    // var clientIdString = CheckClientID(streamGroupNumber, streamId);

    // _logger.LogInformation("GetStreamM3U8 for stream {clientIdString} ", clientIdString);

    // string url = $"/api/streamgroups/{streamGroupNumber}/stream/{streamId}/{clientIdString}.ts";

    // int mediaSequence = GenerateMediaSequence();

    // StringBuilder playlistBuilder = new StringBuilder();

    // playlistBuilder.AppendLine("#EXTM3U");
    // playlistBuilder.AppendLine("#EXT-X-VERSION:3");
    // playlistBuilder.AppendLine("#EXT-X-ALLOW-CACHE:YES");
    // playlistBuilder.AppendLine("#EXT-X-TARGETDURATION:10");
    // playlistBuilder.AppendLine("#EXT-X-MEDIA-SEQUENCE:" + mediaSequence); playlistBuilder.AppendLine();

    // for (int i = 0; i < 48; i++) {
    // playlistBuilder.AppendLine("#EXTINF:10.010000,");
    // playlistBuilder.AppendLine(url); }

    // var m3u8Playlist = playlistBuilder.ToString();

    //    return new ContentResult
    //    {
    //        Content = m3u8Playlist,
    //        ContentType = "application/x-mpegURL",
    //        StatusCode = 200
    //    };
    //}

    //[HttpGet]
    //[Route("{StreamGroupNumber}/stream/{StreamId}/{clientId}.m3u8")]
    //[ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    //[ProducesResponseType(StatusCodes.Status404NotFound)]
    //public IActionResult GetStreamM3U8WithClientId(int StreamGroupNumber, int StreamId, string clientId, CancellationToken cancellationToken)
    //{
    //    var settings = FileUtil.GetSetting();

    // if (settings.StreamingProxyType == StreamingProxyTypes.None) {
    // _logger.LogCritical("Cannot get M3U8 Playlist for stream {streamId} as
    // proxy is set to none", StreamId); return NotFound(); }

    // var clientIdString = CheckClientID(StreamGroupNumber, StreamId, clientId);

    // ClientTracker? clientTracker; clientTrackers.TryGetValue(clientIdString,
    // out clientTracker); if (clientTracker == null) {
    // _logger.LogInformation("GetStreamM3U8WithClientId request. SG Number {id}
    // ChannelId {channelId} not found exiting", StreamGroupNumber, StreamId);
    // return NotFound(); }

    // _logger.LogInformation("GetStreamM3U8 for stream {clientIdString} ", clientIdString);

    // string url = $"/api/streamgroups/{StreamGroupNumber}/stream/{StreamId}/{clientIdString}.ts";

    // int mediaSequence = GenerateMediaSequence();

    // const string timeout = "10.0"; StringBuilder playlistBuilder = new StringBuilder();

    // playlistBuilder.AppendLine("#EXTM3U");
    // playlistBuilder.AppendLine("#EXT-X-VERSION:3");
    // playlistBuilder.AppendLine("#EXT-X-ALLOW-CACHE:YES");
    // playlistBuilder.AppendLine("#EXT-X-TARGETDURATION:" + timeout);
    // playlistBuilder.AppendLine("#EXT-X-MEDIA-SEQUENCE:" + mediaSequence); playlistBuilder.AppendLine();

    // ++clientTracker.M3UGet; if (clientTracker.M3UGet == 1 ||
    // clientTracker.SegmentCount < 1) {
    // playlistBuilder.AppendLine($"#EXTINF:1,\n{url}"); }

    // for (int i = 0; i < 5; i++) {
    // playlistBuilder.AppendLine($"#EXTINF:{timeout},\n{url}"); }

    // var m3u8Playlist = playlistBuilder.ToString();

    //    return new ContentResult
    //    {
    //        Content = m3u8Playlist,
    //        ContentType = "application/x-mpegURL",
    //        StatusCode = 200
    //    };
    //}

    [HttpPut]
    [Route("[action]")]
    public async Task<ActionResult> UpdateStreamGroup(UpdateStreamGroupRequest request)
    {
        StreamGroupDto? entity = await Mediator.Send(request).ConfigureAwait(false);
        return entity == null ? NotFound() : NoContent();
    }

    //private string CheckClientID(int streamGroupNumber, int streamId, string clientId = "")
    //{
    //    if (!string.IsNullOrEmpty(clientId))
    //    {
    //        if (!clientTrackers.TryGetValue(clientId, out _))
    //        {
    //            clientTrackers.TryAdd(clientId, new ClientTracker { StreamGroupNumber = streamGroupNumber, StreamId = streamId });
    //        }
    //        HttpContext.Session.SetString("ClientId", clientId);
    //        return clientId;
    //    }

    // string? clientIdString = HttpContext.Session.GetString("ClientId");

    // if (string.IsNullOrEmpty(clientIdString) ||
    // !clientTrackers.TryGetValue(clientIdString, out ClientTracker? cl) ||
    // cl.StreamGroupNumber != streamGroupNumber || cl.StreamId != streamId) {
    // clientIdString = Guid.NewGuid().ToString();
    // clientTrackers.TryAdd(clientIdString, new ClientTracker {
    // StreamGroupNumber = streamGroupNumber, StreamId = streamId }); }

    //    HttpContext.Session.SetString("ClientId", clientIdString);
    //    return clientIdString;
    //}

    private ObjectResult GetStatus(ProxyStreamErrorCode proxyStreamErrorCode)
    {
        return proxyStreamErrorCode switch
        {
            ProxyStreamErrorCode.FileNotFound => StatusCode(StatusCodes.Status500InternalServerError, "FFmpeg executable not found"),
            ProxyStreamErrorCode.IoError => StatusCode(StatusCodes.Status502BadGateway, "Error connecting to upstream server"),
            ProxyStreamErrorCode.UnknownError => StatusCode(StatusCodes.Status500InternalServerError, "An unknown error occurred"),
            ProxyStreamErrorCode.HttpRequestError => StatusCode(StatusCodes.Status500InternalServerError, "An Http Request Error occurred"),
            ProxyStreamErrorCode.ChannelManagerFinished => StatusCode(200, "Channel Manager Exited"),
            ProxyStreamErrorCode.HttpError => StatusCode(StatusCodes.Status500InternalServerError, "An Http Request Error occurred"),
            ProxyStreamErrorCode.DownloadError => StatusCode(StatusCodes.Status500InternalServerError, "Could not parse stream"),
            ProxyStreamErrorCode.MasterPlayListNotSupported => StatusCode(StatusCodes.Status500InternalServerError, "M3U8 Master playlist not supported"),
            _ => StatusCode(StatusCodes.Status500InternalServerError, "An unknown error occurred"),
        };
    }

    private string GetUrl()
    {
        HttpRequest request = HttpContext.Request;
        string scheme = request.Scheme;
        HostString host = request.Host;
        PathString path = request.Path;
        QueryString queryString = request.QueryString;

        string url = $"{scheme}://{host}{path}{queryString}";

        return url;
    }
}