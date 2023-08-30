using AutoMapper;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using StreamMasterApplication.Common.Interfaces;
using StreamMasterApplication.Common.Models;
using StreamMasterApplication.StreamGroups.Queries;
using StreamMasterApplication.VideoStreamLinks;
using StreamMasterApplication.VideoStreamLinks.Commands;
using StreamMasterApplication.VideoStreamLinks.Queries;
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

public class VideoStreamLinksController : ApiControllerBase, IVideoStreamLinkController
{
    private readonly IChannelManager _channelManager;
    private readonly ILogger<VideoStreamLinksController> _logger;

    private readonly IMapper _mapper;

    public VideoStreamLinksController(IChannelManager channelManager, ILogger<VideoStreamLinksController> logger, IMapper mapper)
    {
        _channelManager = channelManager;
        _logger = logger;
        _mapper = mapper;
    }

    [HttpPut]
    [Route("[action]")]
    public async Task<ActionResult> AddVideoStreamToVideoStream(AddVideoStreamToVideoStreamRequest request, CancellationToken cancellationToken)
    {
        await Mediator.Send(request, cancellationToken).ConfigureAwait(false);
        return Ok();
    }

    [HttpGet]
    [Route("[action]")]
    public async Task<ActionResult<List<string>>> GetVideoStreamVideoStreamIds(GetVideoStreamVideoStreamIdsRequest request, CancellationToken cancellationToken)
    {
        var data = await Mediator.Send(request, cancellationToken).ConfigureAwait(false);
        return Ok(data);
    }

    [HttpGet]
    [Route("[action]")]
    public async Task<ActionResult<List<ChildVideoStreamDto>>> GetVideoStreamVideoStreams(GetVideoStreamVideoStreamsRequest request, CancellationToken cancellationToken)
    {
        var data = await Mediator.Send(request, cancellationToken).ConfigureAwait(false);
        return Ok(data);
    }

    [HttpPut]
    [Route("[action]")]
    public async Task<ActionResult> RemoveVideoStreamFromVideoStream(RemoveVideoStreamFromVideoStreamRequest request, CancellationToken cancellationToken)
    {
        await Mediator.Send(request, cancellationToken).ConfigureAwait(false);
        return Ok();
    }
}