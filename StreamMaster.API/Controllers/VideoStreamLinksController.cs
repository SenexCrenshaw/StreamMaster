using AutoMapper;

using Microsoft.AspNetCore.Mvc;

using StreamMaster.Domain.Dto;
using StreamMaster.Domain.Pagination;

using StreamMaster.Application.Common.Interfaces;
using StreamMaster.Application.VideoStreamLinks;
using StreamMaster.Application.VideoStreamLinks.Commands;
using StreamMaster.Application.VideoStreamLinks.Queries;

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

    [HttpPatch]
    [Route("[action]")]
    public async Task<ActionResult> AddVideoStreamToVideoStream(AddVideoStreamToVideoStreamRequest request)
    {
        await Mediator.Send(request).ConfigureAwait(false);
        return Ok();
    }

    [HttpGet]
    [Route("[action]")]
    public async Task<ActionResult<List<string>>> GetVideoStreamVideoStreamIds([FromQuery] GetVideoStreamVideoStreamIdsRequest request, CancellationToken cancellationToken)
    {
        List<string> data = await Mediator.Send(request, cancellationToken).ConfigureAwait(false);
        return Ok(data);
    }

    [HttpGet]
    [Route("[action]")]
    public async Task<ActionResult<PagedResponse<VideoStreamDto>>> GetPagedVideoStreamVideoStreams([FromQuery] VideoStreamLinkParameters Parameters, CancellationToken cancellationToken)
    {
        PagedResponse<VideoStreamDto> data = await Mediator.Send(new GetPagedVideoStreamVideoStreams(Parameters), cancellationToken).ConfigureAwait(false);
        return Ok(data);
    }

    [HttpPatch]
    [Route("[action]")]
    public async Task<ActionResult> RemoveVideoStreamFromVideoStream(RemoveVideoStreamFromVideoStreamRequest request)
    {
        await Mediator.Send(request).ConfigureAwait(false);
        return Ok();
    }
}