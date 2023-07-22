﻿using AutoMapper;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

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

using System.Web;

namespace StreamMasterAPI.Controllers;

public class VideoStreamsController : ApiControllerBase, IVideoStreamController
{
    private readonly IChannelManager _channelManager;
    private readonly ILogger<VideoStreamsController> _logger;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _memoryCache;

    public VideoStreamsController(IChannelManager channelManager, IMapper mapper, IMemoryCache memoryCache, ILogger<VideoStreamsController> logger)
    {
        _mapper = mapper;
        _memoryCache = memoryCache;
        _channelManager = channelManager;
        _logger = logger;
    }

    [HttpPost]
    [Route("[action]")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(VideoStreamDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> AddVideoStream(AddVideoStreamRequest request)
    {
        return Ok(await Mediator.Send(request).ConfigureAwait(false));
    }

    [HttpPost]
    [Route("[action]")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> ChangeVideoStreamChannel(ChangeVideoStreamChannelRequest request)
    {
        await Mediator.Send(request).ConfigureAwait(false);
        return Ok();
    }

    [HttpDelete]
    [Route("[action]")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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

    [Route("[action]")]
    [ProducesResponseType(typeof(List<StreamStatisticsResult>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllStatisticsForAllUrls()
    {
        List<StreamStatisticsResult> data = await Mediator.Send(new GetAllStatisticsForAllUrls()).ConfigureAwait(false);
        return Ok(data);
    }

    [Authorize(Policy = "SGLinks")]
    [HttpGet]
    [Route("stream/{encodedIds}")]
    [Route("stream/{encodedIds}.mp4")]
    [Route("stream/{encodedIds}/{name}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetVideoStreamStream(string encodedIds, string name, CancellationToken cancellationToken)
    {
        (int? StreamGroupNumberNull, string? StreamIdNull) = encodedIds.DecodeTwoValuesAsString128(_setting.ServerKey);
        if (StreamGroupNumberNull == null || StreamIdNull == null)
        {
            return new NotFoundResult();
        }

        int streamGroupNumber = (int)StreamGroupNumberNull;
        string videoStreamId = (string)StreamIdNull;

        var videoStream = await Mediator.Send(new GetVideoStream(videoStreamId), cancellationToken).ConfigureAwait(false);
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

        var settings = FileUtil.GetSetting();

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
    [Route("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(VideoStreamDto))]
    public async Task<ActionResult<VideoStreamDto?>> GetVideoStream(string id)
    {
        return await Mediator.Send(new GetVideoStream(id)).ConfigureAwait(false);
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<VideoStreamDto>))]
    public async Task<ActionResult<List<VideoStreamDto>>> GetVideoStreams()
    {
        IEnumerable<VideoStreamDto> data = await Mediator.Send(new GetVideoStreams()).ConfigureAwait(false);
        return data.ToList();
    }

    [HttpPatch]
    [Route("[action]")]
    [ProducesResponseType(typeof(IEnumerable<ChannelNumberPair>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> SetVideoStreamChannelNumbers(SetVideoStreamChannelNumbersRequest request)
    {
        await Mediator.Send(request).ConfigureAwait(false);
        return NoContent();
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

        _channelManager.SimulateStreamFailure(HttpUtility.UrlDecode(streamUrl));
        return Ok();
    }

    [HttpPost]
    [Route("[action]")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult SimulateStreamFailureForAll()
    {
        _channelManager.SimulateStreamFailureForAll();
        return Ok();
    }

    [HttpPut]
    [Route("[action]")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> UpdateVideoStream(UpdateVideoStreamRequest request)
    {
        _ = await Mediator.Send(request).ConfigureAwait(false);
        return Ok();
    }

    [HttpPut]
    [Route("[action]")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> UpdateVideoStreams(UpdateVideoStreamsRequest request)
    {
        _ = await Mediator.Send(request).ConfigureAwait(false);
        return Ok();
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
