﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using StreamMaster.Application.SMChannels.Commands;
using StreamMaster.Application.StreamGroups.CommandsOld;
using StreamMaster.Application.VideoStreams;
using StreamMaster.Application.VideoStreams.Commands;
using StreamMaster.Application.VideoStreams.Queries;
using StreamMaster.Domain.API;
using StreamMaster.Domain.Authentication;
using StreamMaster.Domain.Enums;
using StreamMaster.Domain.Pagination;
using StreamMaster.Domain.Requests;
using StreamMaster.Infrastructure.Clients;
using StreamMaster.Streams.Domain.Interfaces;

namespace StreamMaster.API.Controllers;

public class VideoStreamsController : ApiControllerBase, IVideoStreamController
{
    private readonly IChannelManager _channelManager;
    private readonly ILogger<VideoStreamsController> _logger;

    public VideoStreamsController(IChannelManager channelManager, ILogger<VideoStreamsController> logger)
    {
        _channelManager = channelManager;
        _logger = logger;
    }

    [HttpPost]
    [Route("[action]")]
    public async Task<ActionResult> CreateVideoStream(CreateVideoStreamRequest request)
    {
        await Mediator.Send(request).ConfigureAwait(false);
        return Ok();
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
        bool data = await Mediator.Send(request).ConfigureAwait(false);
        return data ? NoContent() : NotFound();
    }

    [HttpPost]
    [Route("[action]")]
    public IActionResult FailClient(FailClientRequest request)
    {
        _channelManager.FailClient(request.clientId);
        return Ok();
    }


    [HttpGet]
    [Route("{id}")]
    public async Task<ActionResult<VideoStreamDto?>> GetVideoStream(string id)
    {
        VideoStreamDto? data = await Mediator.Send(new GetVideoStream(id)).ConfigureAwait(false);
        return data;
    }


    [HttpGet]
    [Route("[action]")]
    public async Task<ActionResult<List<IdName>>> GetVideoStreamNames()
    {
        List<IdName> res = await Mediator.Send(new GetVideoStreamNamesRequest()).ConfigureAwait(false);
        return Ok(res);
    }

    [HttpGet]
    public async Task<ActionResult<PagedResponse<VideoStreamDto>>> GetPagedVideoStreams([FromQuery] VideoStreamParameters videoStreamParameters)
    {
        PagedResponse<VideoStreamDto> res = await Mediator.Send(new GetPagedVideoStreams(videoStreamParameters)).ConfigureAwait(false);
        return Ok(res);
    }

    private StreamingProxyTypes GetStreamingProxyType(VideoStreamDto videoStream)
    {

        return videoStream.StreamingProxyType == StreamingProxyTypes.SystemDefault
            ? Settings.StreamingProxyType
            : videoStream.StreamingProxyType;
    }

    [Authorize(Policy = "SGLinks")]
    [HttpGet]
    [HttpHead]
    [Route("stream/{encodedIds}")]
    [Route("stream/{encodedIds}.mp4")]
    [Route("stream/{encodedIds}.ts")]
    [Route("stream/{encodedIds}/{name}")]
    public async Task<ActionResult> GetVideoStreamStream(string encodedIds, string name, CancellationToken cancellationToken)
    {

        (int? StreamGroupNumberNull, string? StreamIdNull) = encodedIds.DecodeTwoValuesAsString128(Settings.ServerKey);
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

        if (
            string.IsNullOrEmpty(videoStream.User_Url) &&
            (videoStream.ChildVideoStreams.Count == 0 || string.IsNullOrEmpty(videoStream.ChildVideoStreams.First().User_Url))
        )
        {
            _logger.LogInformation("GetStreamGroupVideoStream request. SG Number {id} ChannelId {channelId} missing url or additional streams", streamGroupNumber, videoStreamId);
            return new NotFoundResult();
        }

        HttpContext.Session.Remove("ClientId");

        StreamingProxyTypes proxyType = GetStreamingProxyType(videoStream);
        bool redirect = proxyType == StreamingProxyTypes.None;

        if (redirect)
        {
            _logger.LogInformation("GetStreamGroupVideoStream request SG Number {id} ChannelId {channelId} proxy is none, sending redirect", streamGroupNumber, videoStreamId);

            return Redirect(videoStream.User_Url);
        }


        string? ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

        ClientStreamerConfiguration config = new(videoStream.Id, videoStream.User_Tvg_name, Request.Headers["User-Agent"].ToString(), ipAddress ?? "unkown", cancellationToken, HttpContext.Response);

        Stream? stream = await _channelManager.GetChannel(config);


        HttpContext.Response.RegisterForDispose(new UnregisterClientOnDispose(_channelManager, config));
        return stream != null ? new FileStreamResult(stream, "video/mp4") : StatusCode(StatusCodes.Status404NotFound);
    }

    public async Task ReadAndWriteAsync(Stream sourceStream, string filePath, CancellationToken cancellationToken = default)
    {
        const int bufferSize = 1024; // Read in chunks of 1024 bytes
        const int totalSize = 1 * 1024 * 1024;
        Memory<byte> buffer = new(new byte[bufferSize]);
        int totalBytesRead = 0;

        // Ensure the source stream supports reading
        if (!sourceStream.CanRead)
        {
            throw new InvalidOperationException("Source stream does not support reading.");
        }

        try
        {
            using FileStream fileStream = new(filePath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize, true);

            while (totalBytesRead < totalSize)
            {
                int maxReadLength = Math.Min(buffer.Length, totalSize - totalBytesRead);
                int bytesRead = await sourceStream.ReadAsync(buffer[..maxReadLength], cancellationToken).ConfigureAwait(false);

                if (bytesRead == 0)
                {
                    break; // End of the source stream
                }

                await fileStream.WriteAsync(buffer[..bytesRead], cancellationToken).ConfigureAwait(false);
                totalBytesRead += bytesRead;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error writing stream to file");
            throw;
        }
    }

    [HttpPatch]
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
    public async Task<ActionResult> SetVideoStreamsLogoFromEPG(SetVideoStreamsLogoFromEPGRequest request)
    {
        await Mediator.Send(request).ConfigureAwait(false);
        return Ok();
    }

    [HttpPatch]
    [Route("[action]")]
    public async Task<ActionResult> UpdateVideoStream(UpdateVideoStreamRequest request)
    {
        _ = await Mediator.Send(request).ConfigureAwait(false);
        return Ok();
    }

    [HttpPatch]
    [Route("[action]")]
    public async Task<ActionResult> UpdateVideoStreams(UpdateVideoStreamsRequest request)
    {
        await Mediator.Send(request).ConfigureAwait(false);
        return Ok();
    }

    [HttpPatch]
    [Route("[action]")]
    public async Task<ActionResult> UpdateAllVideoStreamsFromParameters(UpdateAllVideoStreamsFromParametersRequest request)
    {
        await Mediator.Send(request).ConfigureAwait(false);
        return Ok();
    }

    [HttpDelete]
    [Route("[action]")]
    public async Task<ActionResult> DeleteAllVideoStreamsFromParameters(DeleteAllVideoStreamsFromParametersRequest request)
    {
        await Mediator.Send(request).ConfigureAwait(false);
        return Ok();
    }

    [HttpPatch]
    [Route("[action]")]
    public async Task<ActionResult> SetVideoStreamChannelNumbersFromParameters(SetVideoStreamChannelNumbersFromParametersRequest request)
    {
        await Mediator.Send(request).ConfigureAwait(false);
        return Ok();
    }

    [HttpPatch]
    [Route("[action]")]
    public async Task<ActionResult> SetVideoStreamsLogoFromEPGFromParameters(SetVideoStreamsLogoFromEPGFromParametersRequest request)
    {
        await Mediator.Send(request).ConfigureAwait(false);
        return Ok();
    }

    [HttpPatch]
    [Route("[action]")]
    public async Task<ActionResult> ReSetVideoStreamsLogoFromParameters(ReSetVideoStreamsLogoFromParametersRequest request)
    {
        await Mediator.Send(request).ConfigureAwait(false);
        return Ok();
    }

    [HttpPost]
    [Route("[action]")]
    public ActionResult SimulateStreamFailureForAll()
    {
        _channelManager.SimulateStreamFailureForAll();
        return Ok();
    }


    [HttpPost]
    [Route("[action]")]
    public async Task<IActionResult> SimulateStreamFailure(SimulateStreamFailureRequest request)
    {
        await Mediator.Send(request).ConfigureAwait(false);
        return Ok();
    }

    [HttpPatch]
    [Route("[action]")]
    public async Task<IActionResult> AutoSetEPG(AutoSetEPGRequest request)
    {
        await Mediator.Send(request).ConfigureAwait(false);
        return Ok();
    }

    [HttpPatch]
    [Route("[action]")]
    public async Task<IActionResult> AutoSetEPGFromParameters(AutoSetEPGFromParametersRequest request)
    {
        await Mediator.Send(request).ConfigureAwait(false);
        return Ok();
    }
    [HttpPatch]
    [Route("[action]")]
    public async Task<ActionResult> SetVideoStreamTimeShifts(SetVideoStreamTimeShiftsRequest request)
    {
        await Mediator.Send(request).ConfigureAwait(false);
        return Ok();
    }
    [HttpPatch]
    [Route("[action]")]
    public async Task<ActionResult> SetVideoStreamTimeShiftFromParameters(SetVideoStreamTimeShiftFromParametersRequest request)
    {
        await Mediator.Send(request).ConfigureAwait(false);
        return Ok();
    }


    [HttpGet]
    [Route("[action]")]
    public async Task<ActionResult<VideoInfo>> GetVideoStreamInfoFromId([FromQuery] GetVideoStreamInfoFromIdRequest request)
    {
        VideoInfo res = await Mediator.Send(request).ConfigureAwait(false);
        return Ok(res);
    }


    [HttpGet]
    [Route("[action]")]
    public async Task<ActionResult<VideoInfo>> GetVideoStreamInfoFromUrl([FromQuery] GetVideoStreamInfoFromUrlRequest request)
    {
        VideoInfo res = await Mediator.Send(request).ConfigureAwait(false);
        return Ok(res);
    }

    [HttpGet]
    [Route("[action]")]

    public async Task<ActionResult<List<IdNameUrl>>> GetVideoStreamNamesAndUrls()
    {
        List<IdNameUrl> res = await Mediator.Send(new GetVideoStreamNamesAndUrlsRequest()).ConfigureAwait(false);
        return Ok(res);
    }

    private class UnregisterClientOnDispose(IChannelManager channelManager, ClientStreamerConfiguration config) : IDisposable
    {
        private readonly IChannelManager _channelManager = channelManager;
        private readonly ClientStreamerConfiguration _config = config;

        public void Dispose()
        {
            _channelManager.RemoveClient(_config);
        }
    }
}