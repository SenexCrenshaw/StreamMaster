using Microsoft.AspNetCore.Mvc;

using StreamMasterApplication.StreamGroupChannelGroups;
using StreamMasterApplication.StreamGroupChannelGroups.Commands;
using StreamMasterApplication.StreamGroupChannelGroups.Queries;

using StreamMasterDomain.Dto;

namespace StreamMasterAPI.Controllers;

public class StreamGroupChannelGroupController : ApiControllerBase, IStreamGroupChannelGroupController
{
    [HttpPatch]
    [Route("[action]")]
    public async Task<ActionResult<int>> SyncStreamGroupChannelGroups(SyncStreamGroupChannelGroupsRequest request, CancellationToken cancellationToken)
    {
        int res = await Mediator.Send(request, cancellationToken).ConfigureAwait(false);
        return Ok(res);
    }

    [HttpGet]
    [Route("[action]")]
    public async Task<ActionResult<IEnumerable<ChannelGroupDto>>> GetChannelGroupsFromStreamGroup([FromQuery] GetChannelGroupsFromStreamGroupRequest request, CancellationToken cancellationToken)
    {
        IEnumerable<ChannelGroupDto> res = await Mediator.Send(request, cancellationToken).ConfigureAwait(false);
        return Ok(res);
    }

    [HttpDelete]
    [Route("[action]")]
    public async Task<ActionResult<IEnumerable<string>>> RemoveStreamGroupChannelGroups(RemoveStreamGroupChannelGroupsRequest request, CancellationToken cancellationToken)
    {
        IEnumerable<string> res = await Mediator.Send(request, cancellationToken).ConfigureAwait(false);
        return Ok(res);
    }
}