using AutoMapper;

using Microsoft.AspNetCore.Mvc;

using StreamMasterApplication.Common.Interfaces;
using StreamMasterApplication.VideoStreamLinks;
using StreamMasterApplication.VideoStreamLinks.Commands;
using StreamMasterApplication.VideoStreamLinks.Queries;

using StreamMasterDomain.Dto;
using StreamMasterDomain.Pagination;

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