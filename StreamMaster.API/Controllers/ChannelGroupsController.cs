using Microsoft.AspNetCore.Mvc;

using StreamMaster.Application.ChannelGroups.Commands;
using StreamMaster.Application.ChannelGroups.CommandsOld;
using StreamMaster.Application.ChannelGroups.Queries;
using StreamMaster.Application.Common;


namespace StreamMaster.API.Controllers;

[V1ApiController("api/[controller]")]
public class ChannelGroupsController : ApiControllerBase
{

    [HttpPost]
    public async Task<ActionResult> CreateChannelGroup(CreateChannelGroupRequest request)
    {
        await Mediator.Send(request).ConfigureAwait(false);
        return Ok();
    }

    [HttpDelete("[action]")]
    public async Task<ActionResult> DeleteAllChannelGroupsFromParameters(DeleteAllChannelGroupsFromParametersRequest request)
    {
        await Mediator.Send(request).ConfigureAwait(false);
        return Ok();
    }

    [HttpDelete("[action]")]
    public async Task<ActionResult> DeleteChannelGroup(DeleteChannelGroupRequest request)
    {
        bool ret = await Mediator.Send(request).ConfigureAwait(false);
        return ret ? NoContent() : NotFound();
    }


    [HttpGet]
    [Route("{id}")]
    public async Task<ActionResult<ChannelGroupDto>> GetChannelGroup(int id)
    {
        ChannelGroupDto? data = await Mediator.Send(new GetChannelGroup(id)).ConfigureAwait(false);

        return data != null ? (ActionResult<ChannelGroupDto>)data : (ActionResult<ChannelGroupDto>)NotFound();
    }

    [HttpGet]
    [Route("[action]")]
    public async Task<ActionResult<IEnumerable<ChannelGroupIdName>>> GetChannelGroupIdNames()
    {
        IEnumerable<ChannelGroupIdName> res = await Mediator.Send(new GetChannelGroupIdNames()).ConfigureAwait(false);
        return Ok(res);
    }


    [HttpPatch]
    [Route("[action]")]
    public async Task<ActionResult> UpdateChannelGroup(UpdateChannelGroupRequest request)
    {
        await Mediator.Send(request).ConfigureAwait(false);
        return NoContent();
    }

    [HttpPatch]
    [Route("[action]")]
    public async Task<ActionResult> UpdateChannelGroups(UpdateChannelGroupsRequest request)
    {
        await Mediator.Send(request).ConfigureAwait(false);
        return Ok();
    }

    [HttpGet]
    [Route("[action]")]
    public async Task<ActionResult<IEnumerable<string>>> GetChannelGroupNames()
    {
        IEnumerable<string> res = await Mediator.Send(new GetChannelGroupNames()).ConfigureAwait(false);
        return Ok(res);
    }

    [HttpGet]
    [Route("[action]")]
    public async Task<ActionResult<List<ChannelGroupDto>>> GetChannelGroupsForStreamGroup(GetChannelGroupsForStreamGroupRequest request)
    {
        List<ChannelGroupDto> ret = await Mediator.Send(request).ConfigureAwait(false);
        return ret;
    }
}