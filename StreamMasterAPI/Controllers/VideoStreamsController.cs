using AutoMapper;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using StreamMasterAPI.Extensions;

using StreamMasterApplication.ChannelGroups.Queries;
using StreamMasterApplication.Common.Interfaces;
using StreamMasterApplication.Common.Models;
using StreamMasterApplication.StreamGroups.Commands;
using StreamMasterApplication.StreamGroups.Queries;
using StreamMasterApplication.VideoStreams;
using StreamMasterApplication.VideoStreams.Commands;
using StreamMasterApplication.VideoStreams.Queries;

using StreamMasterDomain.Authentication;
using StreamMasterDomain.Common;
using StreamMasterDomain.Dto;
using StreamMasterDomain.Enums;
using StreamMasterDomain.Pagination;
using StreamMasterDomain.Repository;

using System.Web;

namespace StreamMasterAPI.Controllers;

public class VideoStreamsController : ApiControllerBase, IVideoStreamController
{
    private readonly IChannelManager _channelManager;
    private readonly ILogger<VideoStreamsController> _logger;

    private readonly IMapper _mapper;

    public VideoStreamsController(IChannelManager channelManager, ILogger<VideoStreamsController> logger, IMapper mapper)
    {
        _channelManager = channelManager;
        _logger = logger;
        _mapper = mapper;
    }

    [HttpPost]
    [Route("[action]")]
    public async Task<ActionResult> CreateVideoStream(CreateVideoStreamRequest request)
    {
        return Ok(await Mediator.Send(request).ConfigureAwait(false));
    }

    [HttpPost]
    [Route("[action]")]
    public async Task<ActionResult> ChangeVideoStreamChannel(ChangeVideoStreamChannelRequest request)
    {
        await Mediator.Send(request).ConfigureAwait(false);
        return Ok();
    }

    [HttpDelete]
    [Route("[action]")]
    public async Task<ActionResult> DeleteVideoStream(DeleteVideoStreamRequest request)
    {
        string? data = await Mediator.Send(request).ConfigureAwait(false);
        return data == null ? NotFound() : NoContent();
    }

    [HttpPost]
    [Route("[action]")]
    public async Task<ActionResult> FailClient(FailClientRequest request)
    {
        await _channelManager.FailClient(request.clientId);
        return Ok();
    }

    [HttpGet]
    [Route("[action]")]
    public async Task<ActionResult<List<StreamStatisticsResult>>> GetAllStatisticsForAllUrls()
    {
        List<StreamStatisticsResult> data = await Mediator.Send(new GetAllStatisticsForAllUrls()).ConfigureAwait(false);
        return Ok(data);
    }

    [HttpGet]
    [Route("[action]")]
    public async Task<ActionResult<List<ChannelLogoDto>>> GetChannelLogoDtos()
    {
        List<ChannelLogoDto> data = await Mediator.Send(new GetChannelLogoDtos()).ConfigureAwait(false);
        return Ok(data);
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<ActionResult<VideoStreamDto?>> GetVideoStream(string id)
    {
        VideoStreamDto? data = await Mediator.Send(new GetVideoStream(id)).ConfigureAwait(false);
        return data;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResponse<VideoStreamDto>>> GetVideoStreams([FromQuery] VideoStreamParameters videoStreamParameters)
    {
        var res = await Mediator.Send(new GetVideoStreams(videoStreamParameters)).ConfigureAwait(false);
        return Ok(res);
    }

    [Authorize(Policy = "SGLinks")]
    [HttpGet]
    [Route("stream/{encodedIds}")]
    [Route("stream/{encodedIds}.mp4")]
    [Route("stream/{encodedIds}/{name}")]
    public async Task<ActionResult> GetVideoStreamStream(string encodedIds, string name, CancellationToken cancellationToken)
    {
        (int? StreamGroupNumberNull, string? StreamIdNull) = encodedIds.DecodeTwoValuesAsString128(_setting.ServerKey);
        if (StreamGroupNumberNull == null || StreamIdNull == null)
        {
            return new NotFoundResult();
        }

        int streamGroupNumber = (int)StreamGroupNumberNull;
        string videoStreamId = StreamIdNull;

        VideoStreamDto? videoStream = await Mediator.Send(new GetVideoStream(videoStreamId), cancellationToken).ConfigureAwait(false);
        _logger.LogInformation("GetStreamGroupVideoStream request. SG Number {id} ChannelId {channelId}", streamGroupNumber, videoStreamId);

        if (videoStream == null)
        {
            _logger.LogInformation("GetStreamGroupVideoStream request. SG Number {id} ChannelId {channelId} not found exiting", streamGroupNumber, videoStreamId);
            return NotFound();
        }

        if (string.IsNullOrEmpty(videoStream.User_Url) && !videoStream.ChildVideoStreams.Any() && string.IsNullOrEmpty(videoStream.ChildVideoStreams.First().User_Url))
        {
            _logger.LogInformation("GetStreamGroupVideoStream request. SG Number {id} ChannelId {channelId} missing url or additional streams", streamGroupNumber, videoStreamId);
            return new NotFoundResult();
        }

        HttpContext.Session.Remove("ClientId");

        Setting settings = FileUtil.GetSetting();

        if (settings.StreamingProxyType == StreamingProxyTypes.None)
        {
            _logger.LogInformation("GetStreamGroupVideoStream request SG Number {id} ChannelId {channelId} proxy is none, sending redirect", streamGroupNumber, videoStreamId);

            return Redirect(videoStream.User_Url);
        }

        ClientStreamerConfiguration config = new(videoStream.Id, Request.Headers["User-Agent"].ToString(), cancellationToken);

        // Get the read stream for the client
        Stream? stream = await _channelManager.GetStream(config);

        HttpContext.Response.RegisterForDispose(new UnregisterClientOnDispose(_channelManager, config));
        if (stream != null)
        {
            return new FileStreamResult(stream, "video/mp4");
        }
        //else if (error != null)
        //{
        //    // Log the error using the built-in logging framework
        //    _logger.LogError("Error getting FFmpeg stream: {error.Message}", error.Message);

        //    return GetStatus(error.ErrorCode);
        //    // Return an appropriate error response to the client
        //}
        else
        {
            // Unknown error occurred
            return StatusCode(StatusCodes.Status500InternalServerError, "An unknown error occurred");
        }
    }

    [HttpGet]
    [Route("[action]")]
    public async Task<ActionResult> ReSetVideoStreamsLogo(ReSetVideoStreamsLogoRequest request)
    {
        await Mediator.Send(request).ConfigureAwait(false);
        return Ok();
    }

    [HttpPatch]
    [Route("[action]")]
    public async Task<ActionResult> SetVideoStreamChannelNumbers(SetVideoStreamChannelNumbersRequest request)
    {
        await Mediator.Send(request).ConfigureAwait(false);
        return NoContent();
    }

    [HttpPatch]
    [Route("[action]")]
    public async Task<ActionResult> SetVideoStreamSetEPGsFromName(SetVideoStreamSetEPGsFromNameRequest request)
    {
        await Mediator.Send(request).ConfigureAwait(false);
        return NoContent();
    }

    [HttpGet]
    [Route("[action]")]
    public async Task<ActionResult> SetVideoStreamsLogoToEPG(SetVideoStreamsLogoToEPGRequest request)
    {
        await Mediator.Send(request).ConfigureAwait(false);
        return Ok();
    }

    [HttpPost]
    [Route("[action]/{streamUrl}")]
    public ActionResult SimulateStreamFailure(string streamUrl)
    {
        if (string.IsNullOrEmpty(streamUrl))
        {
            return BadRequest("streamUrl is required.");
        }

        _channelManager.SimulateStreamFailure(HttpUtility.UrlDecode(streamUrl));
        return Ok();
    }

    [HttpPost]
    [Route("[action]")]
    public ActionResult SimulateStreamFailureForAll()
    {
        _channelManager.SimulateStreamFailureForAll();
        return Ok();
    }

    [HttpPut]
    [Route("[action]")]
    public async Task<ActionResult> UpdateVideoStream(UpdateVideoStreamRequest request)
    {
        _ = await Mediator.Send(request).ConfigureAwait(false);
        return Ok();
    }

    [HttpPut]
    [Route("[action]")]
    public async Task<ActionResult> UpdateVideoStreams(UpdateVideoStreamsRequest request)
    {
        _ = await Mediator.Send(request).ConfigureAwait(false);
        return Ok();
    }

    [HttpGet]
    [Route("[action]")]
    public async Task<ActionResult<IEnumerable<VideoStreamDto>>> GetVideoStreamsByNamePattern([FromQuery] GetVideoStreamsByNamePatternQuery request)
    {
        var result = await Mediator.Send(request).ConfigureAwait(false);
        return Ok(result);
    }

    private class UnregisterClientOnDispose : IDisposable
    {
        private readonly IChannelManager _channelManager;
        private readonly ClientStreamerConfiguration _config;

        public UnregisterClientOnDispose(IChannelManager channelManager, ClientStreamerConfiguration config)
        {
            _channelManager = channelManager;
            _config = config;
        }

        public void Dispose()
        {
            _channelManager.RemoveClient(_config);
        }
    }
}