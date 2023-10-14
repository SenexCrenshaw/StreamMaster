using Microsoft.AspNetCore.Mvc;

using StreamMasterApplication.StreamGroupVideoStreams;
using StreamMasterApplication.StreamGroupVideoStreams.Commands;
using StreamMasterApplication.StreamGroupVideoStreams.Queries;

using StreamMasterDomain.Dto;
using StreamMasterDomain.Pagination;

namespace StreamMasterAPI.Controllers;

public class StreamGroupVideoStreamsController() : ApiControllerBase, IStreamGroupVideoStreamController
{
    [HttpGet]
    [Route("[action]")]
    public async Task<ActionResult<List<VideoStreamIsReadOnly>>> GetStreamGroupVideoStreamIds([FromQuery] GetStreamGroupVideoStreamIdsRequest request, CancellationToken cancellationToken = default)
    {
        List<VideoStreamIsReadOnly> res = await Mediator.Send(request, cancellationToken).ConfigureAwait(false);
        return Ok(res);
    }

    [HttpGet]
    [Route("[action]")]
    public async Task<ActionResult<PagedResponse<VideoStreamDto>>> GetPagedStreamGroupVideoStreams([FromQuery] StreamGroupVideoStreamParameters Parameters, CancellationToken cancellationToken = default)
    {
        PagedResponse<VideoStreamDto> res = await Mediator.Send(new GetPagedStreamGroupVideoStreams(Parameters), cancellationToken).ConfigureAwait(false);
        return Ok(res);
    }

    //[HttpPost]
    //[Route("[action]")]
    //public async Task<IActionResult> AddVideoStreamToStreamGroup(AddVideoStreamToStreamGroupRequest request, CancellationToken cancellationToken)
    //{
    //    await Mediator.Send(request, cancellationToken).ConfigureAwait(false);
    //    return NoContent();
    //}

    //[HttpDelete]
    //[Route("[action]")]
    //public async Task<IActionResult> RemoveVideoStreamFromStreamGroup(RemoveVideoStreamFromStreamGroupRequest request, CancellationToken cancellationToken)
    //{
    //    await Mediator.Send(request, cancellationToken).ConfigureAwait(false);
    //    return NoContent();
    //}

    [HttpPatch]
    [Route("[action]")]
    public async Task<IActionResult> SetVideoStreamRanks(SetVideoStreamRanksRequest request, CancellationToken cancellationToken)
    {
        await Mediator.Send(request, cancellationToken).ConfigureAwait(false);
        return NoContent();
    }

    [HttpPost]
    [HttpDelete]
    [Route("[action]")]
    public async Task<IActionResult> SyncVideoStreamToStreamGroup(SyncVideoStreamToStreamGroupRequest request, CancellationToken cancellationToken)
    {
        await Mediator.Send(request, cancellationToken).ConfigureAwait(false);
        return NoContent();
    }

    [HttpPatch]
    [Route("[action]")]
    public async Task<IActionResult> SetStreamGroupVideoStreamChannelNumbers(SetStreamGroupVideoStreamChannelNumbersRequest request, CancellationToken cancellationToken)
    {
        await Mediator.Send(request, cancellationToken).ConfigureAwait(false);
        return NoContent();
    }
}