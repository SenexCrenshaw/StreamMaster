using StreamMasterApplication.ChannelGroups;
using StreamMasterApplication.ChannelGroups.Commands;
using StreamMasterApplication.ChannelGroups.Queries;

using StreamMasterDomain.Pagination;

namespace StreamMasterApplication.Hubs;

public partial class StreamMasterHub : IChannelGroupHub
{
    public async Task CreateChannelGroup(CreateChannelGroupRequest request)
    {
        await mediator.Send(request).ConfigureAwait(false);
    }

    public async Task DeleteAllChannelGroupsFromParameters(DeleteAllChannelGroupsFromParametersRequest request)
    {
        await mediator.Send(request).ConfigureAwait(false);
    }

    public async Task DeleteChannelGroup(DeleteChannelGroupRequest request)
    {
        await mediator.Send(request).ConfigureAwait(false);
    }


    public async Task<ChannelGroupDto?> GetChannelGroup(int id)
    {
        return await mediator.Send(new GetChannelGroup(id)).ConfigureAwait(false);
    }

    public async Task<IEnumerable<ChannelGroupIdName>> GetChannelGroupIdNames()
    {
        IEnumerable<ChannelGroupIdName> ret = await mediator.Send(new GetChannelGroupIdNames()).ConfigureAwait(false);
        return ret;
    }

    public async Task<IEnumerable<string>> GetChannelGroupNames()
    {
        IEnumerable<string> ret = await mediator.Send(new GetChannelGroupNames()).ConfigureAwait(false);
        return ret;
    }

    public async Task<PagedResponse<ChannelGroupDto>> GetChannelGroups(ChannelGroupParameters channelGroupParameters)
    {
        PagedResponse<ChannelGroupDto> ret = await mediator.Send(new GetChannelGroupsQuery(channelGroupParameters)).ConfigureAwait(false);
        return ret;
    }

    public async Task UpdateChannelGroup(UpdateChannelGroupRequest request)
    {
        await mediator.Send(request).ConfigureAwait(false);
    }

    public async Task UpdateChannelGroups(UpdateChannelGroupsRequest request)
    {
        await mediator.Send(request).ConfigureAwait(false);
    }


}