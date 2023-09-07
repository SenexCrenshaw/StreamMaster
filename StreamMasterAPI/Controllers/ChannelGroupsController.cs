using Microsoft.AspNetCore.Mvc;

using StreamMasterApplication.ChannelGroups;
using StreamMasterApplication.ChannelGroups.Commands;
using StreamMasterApplication.ChannelGroups.Queries;

using StreamMasterDomain.Dto;
using StreamMasterDomain.Pagination;

namespace StreamMasterAPI.Controllers;

public class ChannelGroupsController : ApiControllerBase, IChannelGroupController
{

    [HttpPost]
    public async Task<ActionResult> CreateChannelGroup(CreateChannelGroupRequest request)
    {
        await Mediator.Send(request).ConfigureAwait(false);
        return Ok();
    }

    [HttpDelete]
    [Route("[action]")]
    public async Task<ActionResult> DeleteAllChannelGroupsFromParameters(DeleteAllChannelGroupsFromParametersRequest request)
    {
        await Mediator.Send(request).ConfigureAwait(false);
        return Ok();
    }

    [HttpDelete]
    [Route("[action]")]
    public async Task<ActionResult> DeleteChannelGroup(DeleteChannelGroupRequest request)
    {
        bool ret = await Mediator.Send(request).ConfigureAwait(false);
        return ret ? NoContent() : NotFound();
    }
    [HttpGet]
    [Route("[action]")]
    public async Task<ActionResult<List<ChannelGroupDto>>> GetAllChannelGroups([FromQuery] GetAllChannelGroupsRequest request)
    {
        List<ChannelGroupDto> res = await Mediator.Send(request).ConfigureAwait(false);
        return Ok(res);
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
    public async Task<ActionResult<List<string>>> GetChannelGroupNames()
    {
        List<string> res = await Mediator.Send(new GetChannelGroupNames()).ConfigureAwait(false);
        return Ok(res);
    }

    [HttpGet]
    public async Task<ActionResult<PagedResponse<ChannelGroupDto>>> GetChannelGroups([FromQuery] ChannelGroupParameters Parameters)
    {
        PagedResponse<ChannelGroupDto> res = await Mediator.Send(new GetChannelGroupsQuery(Parameters)).ConfigureAwait(false);
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
}