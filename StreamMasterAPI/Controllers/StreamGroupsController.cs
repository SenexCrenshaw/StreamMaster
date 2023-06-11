using AutoMapper;

using Microsoft.AspNetCore.Mvc;

using StreamMasterApplication.Common.Interfaces;
using StreamMasterApplication.Common.Models;
using StreamMasterApplication.Settings.Queries;
using StreamMasterApplication.StreamGroups;
using StreamMasterApplication.StreamGroups.Commands;
using StreamMasterApplication.StreamGroups.Queries;
using StreamMasterApplication.VideoStreams.Queries;

using StreamMasterDomain.Common;
using StreamMasterDomain.Dto;
using StreamMasterDomain.Enums;

using System.Web;

namespace StreamMasterAPI.Controllers;

public class StreamGroupsController : ApiControllerBase, IStreamGroupController
{
    private readonly ILogger<StreamGroupsController> _logger;
    private readonly IMapper _mapper;
    private readonly IRingBufferManager _ringBufferManager;

    public StreamGroupsController(IRingBufferManager ringBufferManager, IMapper mapper, ILogger<StreamGroupsController> logger)
    {
        _mapper = mapper;
        _ringBufferManager = ringBufferManager;
        _logger = logger;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(StreamGroupDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> AddStreamGroup(AddStreamGroupRequest request)
    {
        StreamGroupDto? entity = await Mediator.Send(request).ConfigureAwait(false);
        return entity != null ? CreatedAtAction(nameof(GetStreamGroup), new { id = entity.Id }, entity) : Ok();
    }

    [HttpDelete]
    [Route("[action]")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> DeleteStreamGroup(DeleteStreamGroupRequest request)
    {
        int? data = await Mediator.Send(request).ConfigureAwait(false);
        return data == null ? NotFound() : NoContent();
    }

    [Route("[action]")]
    [ProducesResponseType(typeof(List<StreamStatisticsResult>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllStatisticsForAllUrls()
    {
        List<StreamStatisticsResult> data = await Mediator.Send(new GetAllStatisticsForAllUrls()).ConfigureAwait(false);
        return Ok(data);
    }

    [HttpGet]
    [Route("[action]/{id}")]
    [ProducesResponseType(typeof(StreamGroupDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<StreamGroupDto>> GetStreamGroup(int id)
    {
        StreamGroupDto? data = await Mediator.Send(new GetStreamGroup(id)).ConfigureAwait(false);

        return data != null ? (ActionResult<StreamGroupDto>)data : (ActionResult<StreamGroupDto>)NotFound();
    }

    [HttpGet]
    [Route("[action]/{StreamGroupNumber}")]
    [ProducesResponseType(typeof(StreamGroupDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<StreamGroupDto>> GetStreamGroupByStreamNumber(int StreamGroupNumber)
    {
        StreamGroupDto? data = await Mediator.Send(new GetStreamGroupByStreamNumber(StreamGroupNumber)).ConfigureAwait(false);

        return data != null ? (ActionResult<StreamGroupDto>)data : (ActionResult<StreamGroupDto>)NotFound();
    }

    [HttpGet]
    [Route("{StreamGroupNumber}/capability")]
    [Route("{StreamGroupNumber}")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ContentResult> GetStreamGroupCapability(int StreamGroupNumber)
    {
        string xml = await Mediator.Send(new GetStreamGroupCapability(StreamGroupNumber)).ConfigureAwait(false);
        return new ContentResult
        {
            Content = xml,
            ContentType = "application/xml",
            StatusCode = 200
        };
    }

    [HttpGet]
    [Route("{StreamGroupNumber}/device.xml")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ContentResult> GetStreamGroupDeviceXML(int StreamGroupNumber)
    {
        string xml = await Mediator.Send(new GetStreamGroupCapability(StreamGroupNumber)).ConfigureAwait(false);
        return new ContentResult
        {
            Content = xml,
            ContentType = "application/xml",
            StatusCode = 200
        };
    }

    [HttpGet]
    [Route("{StreamGroupNumber}/discover.json")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ContentResult> GetStreamGroupDiscover(int StreamGroupNumber)
    {
        string json = await Mediator.Send(new GetStreamGroupDiscover(StreamGroupNumber)).ConfigureAwait(false);
        return new ContentResult
        {
            Content = json,
            ContentType = "text/json",
            StatusCode = 200
        };
    }

    [HttpGet]
    [Route("{StreamGroupNumber}/epg.xml")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ContentResult> GetStreamGroupEPG(int StreamGroupNumber)
    {
        string xml = await Mediator.Send(new GetStreamGroupEPG(StreamGroupNumber)).ConfigureAwait(false);
        return new ContentResult
        {
            Content = xml,
            ContentType = "application/xml",
            StatusCode = 200
        };
    }

    [HttpGet]
    [Route("{StreamGroupNumber}/epgGuide.json")]
    [ProducesResponseType(typeof(EPGGuide), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<EPGGuide>> GetStreamGroupEPGForGuide(int StreamGroupNumber)
    {
        var data = await Mediator.Send(new GetStreamGroupEPGForGuide(StreamGroupNumber)).ConfigureAwait(false);
        return data != null ? (ActionResult<EPGGuide>)data : (ActionResult<EPGGuide>)NotFound();
    }

    [HttpGet]
    [Route("{StreamGroupNumber}/lineup.json")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ContentResult> GetStreamGroupLineUp(int StreamGroupNumber)
    {
        string json = await Mediator.Send(new GetStreamGroupLineUp(StreamGroupNumber)).ConfigureAwait(false);
        return new ContentResult
        {
            Content = json,
            ContentType = "application/json",
            StatusCode = 200
        };
    }

    [HttpGet]
    [Route("{StreamGroupNumber}/lineup_status.json")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ContentResult> GetStreamGroupLineUpStatus(int StreamGroupNumber)
    {
        string json = await Mediator.Send(new GetStreamGroupLineUpStatus(StreamGroupNumber)).ConfigureAwait(false);
        return new ContentResult
        {
            Content = json,
            ContentType = "text/json",
            StatusCode = 200
        };
    }

    [HttpGet]
    [Route("{StreamGroupNumber}/m3u")]
    [Route("{StreamGroupNumber}/m3u/m3u.m3u")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ContentResult> GetStreamGroupM3U(int StreamGroupNumber)
    {
        string data = await Mediator.Send(new GetStreamGroupM3U(StreamGroupNumber)).ConfigureAwait(false);

        return new ContentResult
        {
            Content = data,
            ContentType = "text/plain",
            StatusCode = 200
        };
    }

    [HttpGet]
    [Route("{StreamGroupNumber}/m3u2")]
    [Route("{StreamGroupNumber}/m3u2/m3u.m3u")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ContentResult> GetStreamGroupM3U2(int StreamGroupNumber)
    {
        string data = await Mediator.Send(new GetStreamGroupM3U2(StreamGroupNumber)).ConfigureAwait(false);

        return new ContentResult
        {
            Content = data,
            ContentType = "text/plain",
            StatusCode = 200
        };
    }

    [HttpGet]
    [Route("[action]")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<StreamGroupDto>))]
    public async Task<ActionResult<IEnumerable<StreamGroupDto>>> GetStreamGroups()
    {
        IEnumerable<StreamGroupDto> data = await Mediator.Send(new GetStreamGroups()).ConfigureAwait(false);
        return data.ToList();
    }

    [HttpGet]
    [Route("{id}/stream/{streamId}")]
    [Route("{id}/stream/{streamId}.mp4")]
    [Route("{id}/stream/{streamId}.m3u8")]
    [Route("{id}/stream/{streamId}.ts")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStreamGroupVideoStream(int id, int streamId, CancellationToken cancellationToken)
    {
        var videoStream = await Mediator.Send(new GetVideoStream(streamId), cancellationToken).ConfigureAwait(false);
        _logger.LogInformation("GetStreamGroupVideoStream request. SG Number {id} ChannelId {channelId}", id, streamId);

        if (videoStream == null)
        {
            _logger.LogInformation("GetStreamGroupVideoStream request. SG Number {id} ChannelId {channelId} not found exiting", id, streamId);
            return NotFound();
        }

        SettingDto settings = await Mediator.Send(new GetSettings(), cancellationToken).ConfigureAwait(false);

        if (settings.StreamingProxyType == StreamingProxyTypes.None)
        {
            _logger.LogInformation("GetStreamGroupVideoStream request SG Number {id} ChannelId {channelId} proxy is none, sending redirect", id, streamId);

            return Redirect(videoStream.User_Url);
        }

        //List<IconFileDto> icons = await Mediator.Send(new GetIcons(), cancellationToken).ConfigureAwait(false);

        //IconFileDto? icon = icons.SingleOrDefault(a => a.OriginalSource == videoStream.User_Tvg_logo);
        //string logourl = icon != null ? icon.Source : settings.BaseHostURL + settings.DefaultIcon;

        //videoStream.User_Tvg_logo = logourl;

        List<VideoStreamDto> videoStreams = new List<VideoStreamDto> { videoStream };
        StreamerConfiguration config = new()
        {
            VideoStreams = videoStreams,
            BufferSize = settings.RingBufferSizeMB * 1024 * 1000,
            CancellationToken = cancellationToken,
            MaxConnectRetry = settings.MaxConnectRetry,
            MaxConnectRetryTimeMS = settings.MaxConnectRetryTimeMS,
        };

        // Get the read stream for the client
        (Stream? stream, Guid clientId, ProxyStreamError? error) = await _ringBufferManager.GetStream(config);

        HttpContext.Response.RegisterForDispose(new UnregisterClientOnDispose(_ringBufferManager, config));
        if (stream != null)
        {
            return new FileStreamResult(stream, "video/mp4");
        }
        else if (error != null)
        {
            // Log the error using the built-in logging framework
            _logger.LogError("Error getting FFmpeg stream: {error.Message}", error.Message);

            // Return an appropriate error response to the client
            return error.ErrorCode switch
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
        else
        {
            // Unknown error occurred
            return StatusCode(StatusCodes.Status500InternalServerError, "An unknown error occurred");
        }
    }

    [HttpPost]
    [Route("[action]/{streamUrl}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult SimulateStreamFailure(string streamUrl)
    {
        if (string.IsNullOrEmpty(streamUrl))
        {
            return BadRequest("streamUrl is required.");
        }

        _ringBufferManager.SimulateStreamFailure(HttpUtility.UrlDecode(streamUrl));
        return Ok();
    }

    [HttpPut]
    [Route("[action]")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> UpdateStreamGroup(UpdateStreamGroupRequest request)
    {
        StreamGroupDto? entity = await Mediator.Send(request).ConfigureAwait(false);
        return entity == null ? NotFound() : NoContent();
    }

    private class UnregisterClientOnDispose : IDisposable
    {
        private readonly StreamerConfiguration _config;
        private readonly IRingBufferManager _ringBufferManager;

        public UnregisterClientOnDispose(IRingBufferManager ringBufferManager, StreamerConfiguration config)
        {
            _ringBufferManager = ringBufferManager;
            _config = config;
        }

        public void Dispose()
        {
            _ringBufferManager.RemoveClient(_config);
        }
    }
}
