using StreamMasterApplication.ChannelGroups;
using StreamMasterApplication.ChannelGroups.Commands;
using StreamMasterApplication.ChannelGroups.Queries;

using StreamMasterDomain.Dto;

namespace StreamMasterApplication.Hubs;

public partial class StreamMasterHub : IChannelGroupHub
{
    public async Task<ChannelGroupDto?> AddChannelGroup(AddChannelGroupRequest request)
    {
        return await _mediator.Send(request).ConfigureAwait(false);
    }

    public async Task<int?> DeleteChannelGroup(DeleteChannelGroupRequest request)
    {
        return await _mediator.Send(request).ConfigureAwait(false);
    }

    public async Task<ChannelGroupDto?> GetChannelGroup(int id)
    {
        return await _mediator.Send(new GetChannelGroup(id)).ConfigureAwait(false);
    }

    public async Task<IEnumerable<ChannelGroupDto>?> GetChannelGroups()
    {
        return await _mediator.Send(new GetChannelGroups()).ConfigureAwait(false);
    }

    public async Task<IEnumerable<SetChannelGroupsVisibleArg>> SetChannelGroupsVisible(SetChannelGroupsVisibleRequest request)
    {
        return await _mediator.Send(request).ConfigureAwait(false);
    }

    public async Task<ChannelGroupDto?> UpdateChannelGroup(UpdateChannelGroupRequest request)
    {
        return await _mediator.Send(request).ConfigureAwait(false);
    }

    public async Task<IEnumerable<ChannelGroupDto>?> UpdateChannelGroupOrder(UpdateChannelGroupOrderRequest request)
    {
        return await _mediator.Send(request).ConfigureAwait(false);
    }

    public async Task<IEnumerable<ChannelGroupDto>?> UpdateChannelGroups(UpdateChannelGroupsRequest request)
    {
        return await _mediator.Send(request).ConfigureAwait(false);
    }
}
