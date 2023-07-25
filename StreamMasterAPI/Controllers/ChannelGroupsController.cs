using Microsoft.AspNetCore.Mvc;

using StreamMasterApplication.ChannelGroups;
using StreamMasterApplication.ChannelGroups.Commands;
using StreamMasterApplication.ChannelGroups.Queries;

using StreamMasterDomain.Dto;

namespace StreamMasterAPI.Controllers;

public class ChannelGroupsController : ApiControllerBase, IChannelGroupController
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ChannelGroupDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> AddChannelGroup(AddChannelGroupRequest request)
    {
        ChannelGroupDto? entity = await Mediator.Send(request).ConfigureAwait(false);
        return Ok();
    }

    [HttpDelete]
    [Route("[action]")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> DeleteChannelGroup(DeleteChannelGroupRequest request)
    {
        int? data = await Mediator.Send(request).ConfigureAwait(false);
        return data == null ? NotFound() : NoContent();
    }

    [HttpGet]
    [Route("{id}")]
    [ProducesResponseType(typeof(ChannelGroupDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ChannelGroupDto>> GetChannelGroup(int id)
    {
        ChannelGroupDto? data = await Mediator.Send(new GetChannelGroup(id)).ConfigureAwait(false);

        return data != null ? (ActionResult<ChannelGroupDto>)data : (ActionResult<ChannelGroupDto>)NotFound();
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ChannelGroupDto>))]
    public async Task<ActionResult<IEnumerable<ChannelGroupDto>>> GetChannelGroups()
    {
        IEnumerable<ChannelGroupDto> data = await Mediator.Send(new GetChannelGroups()).ConfigureAwait(false);
        return data.ToList();
    }

    [HttpPut]
    [Route("[action]")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> SetChannelGroupsVisible(SetChannelGroupsVisibleRequest request)
    {
        _ = await Mediator.Send(request).ConfigureAwait(false);
        return Ok();
    }

    [HttpPut]
    [Route("[action]")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> UpdateChannelGroup(UpdateChannelGroupRequest request)
    {
        ChannelGroupDto? entity = await Mediator.Send(request).ConfigureAwait(false);
        return entity == null ? NotFound() : NoContent();
    }

    [HttpPut]
    [Route("[action]")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> UpdateChannelGroupOrder(UpdateChannelGroupOrderRequest request)
    {
        IEnumerable<ChannelGroupDto>? entity = await Mediator.Send(request).ConfigureAwait(false);
        return entity == null ? NotFound() : NoContent();
    }

    [HttpPut]
    [Route("[action]")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> UpdateChannelGroups(UpdateChannelGroupsRequest request)
    {
        _ = await Mediator.Send(request).ConfigureAwait(false);
        return Ok();
    }
}
