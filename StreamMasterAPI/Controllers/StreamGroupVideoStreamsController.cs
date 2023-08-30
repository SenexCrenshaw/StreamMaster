using AutoMapper;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using StreamMasterApplication.StreamGroups;
using StreamMasterApplication.StreamGroups.Commands;
using StreamMasterApplication.StreamGroups.Queries;
using StreamMasterApplication.StreamGroupVideoStreams;
using StreamMasterApplication.StreamGroupVideoStreams.Commands;
using StreamMasterApplication.StreamGroupVideoStreams.Queries;
using StreamMasterApplication.VideoStreamLinks.Commands;
using StreamMasterApplication.VideoStreamLinks.Queries;

using StreamMasterDomain.Authentication;
using StreamMasterDomain.Common;
using StreamMasterDomain.Dto;
using StreamMasterDomain.Pagination;

using System.Text;

namespace StreamMasterAPI.Controllers;

public class StreamGroupVideoStreamsController : ApiControllerBase, IStreamGroupVideoStreamController
{
    private readonly ILogger<StreamGroupVideoStreamsController> _logger;

    public StreamGroupVideoStreamsController(ILogger<StreamGroupVideoStreamsController> logger, IMapper mapper)
    {
        _logger = logger;
        _mapper = mapper;
    }

    private readonly IMapper _mapper;

    [HttpGet]
    [Route("[action]")]
    public async Task<ActionResult<List<VideoStreamIsReadOnly>>> GetStreamGroupVideoStreamIds([FromQuery] GetStreamGroupVideoStreamIdsRequest request, CancellationToken cancellationToken = default)
    {
        List<VideoStreamIsReadOnly> res = await Mediator.Send(request, cancellationToken).ConfigureAwait(false);
        return Ok(res);
    }

    [HttpGet]
    [Route("[action]")]
    public async Task<ActionResult<PagedResponse<VideoStreamDto>>> GetStreamGroupVideoStreams([FromQuery] StreamGroupVideoStreamParameters Parameters, CancellationToken cancellationToken = default)
    {
        var res = await Mediator.Send(new GetStreamGroupVideoStreamsRequest(Parameters), cancellationToken).ConfigureAwait(false);
        return Ok(res);
    }

    [HttpPost]
    [Route("[action]")]
    public async Task<ActionResult> AddVideoStreamToStreamGroup(AddVideoStreamToStreamGroupRequest request, CancellationToken cancellationToken)
    {
        await Mediator.Send(request, cancellationToken).ConfigureAwait(false);
        return Ok();
    }

    [HttpPost]
    [Route("[action]")]
    public async Task<ActionResult> RemoveVideoStreamFromStreamGroup(RemoveVideoStreamFromStreamGroupRequest request, CancellationToken cancellationToken)
    {
        await Mediator.Send(request, cancellationToken).ConfigureAwait(false);
        return Ok();
    }
}