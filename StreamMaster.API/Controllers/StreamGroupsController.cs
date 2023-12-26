using AutoMapper;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using StreamMaster.Domain.Authentication;
using StreamMaster.Domain.Common;
using StreamMaster.Domain.Dto;
using StreamMaster.Domain.Pagination;
using StreamMaster.Domain.Requests;

using StreamMaster.Application.StreamGroups;
using StreamMaster.Application.StreamGroups.Commands;
using StreamMaster.Application.StreamGroups.Queries;

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

    private static int GenerateMediaSequence()
    {
        DateTime epochStart = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        TimeSpan elapsedTime = DateTime.UtcNow - epochStart;
        int mediaSequence = (int)(elapsedTime.TotalSeconds / 10);

        return mediaSequence;
    }

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

    [HttpGet]
    [Route("[action]/{id}")]
    public async Task<ActionResult<StreamGroupDto>> GetStreamGroup(int id)
    {
        StreamGroupDto? data = await Mediator.Send(new GetStreamGroup(id)).ConfigureAwait(false);

        return data != null ? (ActionResult<StreamGroupDto>)data : (ActionResult<StreamGroupDto>)NotFound();
    }

    [HttpGet]
    [AllowAnonymous]
    [Route("{encodedId}")]
    [Route("{encodedId}/capability")]
    [Route("{encodedId}/device.xml")]
    public async Task<IActionResult> GetStreamGroupCapability(string encodedId)
    {
        Setting setting = await SettingsService.GetSettingsAsync();
        int? streamGroupId = encodedId.DecodeValue128(setting.ServerKey);
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
        Setting setting = await SettingsService.GetSettingsAsync();
        int? streamGroupNumber = encodedId.DecodeValue128(setting.ServerKey);
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
        Setting setting = await SettingsService.GetSettingsAsync();
        int? streamGroupNumber = encodedId.DecodeValue128(setting.ServerKey);
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
        Setting setting = await SettingsService.GetSettingsAsync();
        int? streamGroupNumber = encodedId.DecodeValue128(setting.ServerKey);
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
        Setting setting = await SettingsService.GetSettingsAsync();
        int? streamGroupNumber = encodedId.DecodeValue128(setting.ServerKey);
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
        Setting setting = await SettingsService.GetSettingsAsync();
        int? streamGroupNumber = encodedId.DecodeValue128(setting.ServerKey);
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
    public async Task<ActionResult<PagedResponse<StreamGroupDto>>> GetPagedStreamGroups([FromQuery] StreamGroupParameters parameters)
    {
        PagedResponse<StreamGroupDto> res = await Mediator.Send(new GetPagedStreamGroups(parameters)).ConfigureAwait(false);
        return Ok(res);
    }

    [HttpPatch]
    [Route("[action]")]
    public async Task<ActionResult> UpdateStreamGroup(UpdateStreamGroupRequest request)
    {
        StreamGroupDto? entity = await Mediator.Send(request).ConfigureAwait(false);
        return entity == null ? NotFound() : NoContent();
    }

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

    [HttpGet]
    [Route("[action]")]
    public async Task<ActionResult<string?>> GetStreamGroupVideoStreamUrl(string VideoStreamId)
    {
        string? res = await Mediator.Send(new GetStreamGroupVideoStreamUrl(VideoStreamId)).ConfigureAwait(false);
        return Ok(res);
    }
}