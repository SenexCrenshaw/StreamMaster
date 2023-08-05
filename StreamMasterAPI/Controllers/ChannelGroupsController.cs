using AutoMapper;

using Microsoft.AspNetCore.Mvc;

using StreamMasterApplication.ChannelGroups;
using StreamMasterApplication.ChannelGroups.Commands;
using StreamMasterApplication.ChannelGroups.Queries;

using StreamMasterDomain.Dto;
using StreamMasterDomain.Pagination;

namespace StreamMasterAPI.Controllers;

public class ChannelGroupsController : ApiControllerBase, IChannelGroupController
{
    private readonly IMapper _mapper;

    public ChannelGroupsController(IMapper mapper)
    {
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<ActionResult> CreateChannelGroup(CreateChannelGroupRequest request)
    {
        ChannelGroupDto? result = await Mediator.Send(request).ConfigureAwait(false);
        return result != null ? Ok() : BadRequest();
    }

    [HttpDelete]
    [Route("[action]")]
    public async Task<ActionResult> DeleteChannelGroup(DeleteChannelGroupRequest request)
    {
        int? data = await Mediator.Send(request).ConfigureAwait(false);
        return data == null ? NotFound() : NoContent();
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<ActionResult<ChannelGroupDto>> GetChannelGroup(int id)
    {
        ChannelGroupDto? data = await Mediator.Send(new GetChannelGroup(id)).ConfigureAwait(false);

        return data != null ? (ActionResult<ChannelGroupDto>)data : (ActionResult<ChannelGroupDto>)NotFound();
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ChannelGroupDto>>> GetChannelGroups(ChannelGroupParameters channelGroupParameters)
    {
        PagedList<StreamMasterDomain.Repository.ChannelGroup> channelGroups = await Mediator.Send(new GetChannelGroupsQuery(channelGroupParameters)).ConfigureAwait(false);
        Response.Headers.Add("X-Pagination", channelGroups.GetMetadata());
        IEnumerable<M3UFileDto> channelGroupsResult = _mapper.Map<IEnumerable<M3UFileDto>>(channelGroups);
        return Ok(channelGroupsResult);
    }

    [HttpPut]
    [Route("[action]")]
    public async Task<ActionResult> SetChannelGroupsVisible(SetChannelGroupsVisibleRequest request)
    {
        _ = await Mediator.Send(request).ConfigureAwait(false);
        return Ok();
    }

    [HttpPut]
    [Route("[action]")]
    public async Task<ActionResult> UpdateChannelGroup(UpdateChannelGroupRequest request)
    {
        ChannelGroupDto? entity = await Mediator.Send(request).ConfigureAwait(false);
        return entity == null ? NotFound() : NoContent();
    }



    [HttpPut]
    [Route("[action]")]
    public async Task<ActionResult> UpdateChannelGroups(UpdateChannelGroupsRequest request)
    {
        _ = await Mediator.Send(request).ConfigureAwait(false);
        return Ok();
    }
}
