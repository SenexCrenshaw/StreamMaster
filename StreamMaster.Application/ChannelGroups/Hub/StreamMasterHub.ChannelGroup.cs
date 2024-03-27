using StreamMaster.Application.ChannelGroups.Queries;

namespace StreamMaster.Application.Hubs;

public partial class StreamMasterHub
{

    public async Task DeleteAllChannelGroupsFromParameters(DeleteAllChannelGroupsFromParametersRequest request)
    {
        await Sender.Send(request).ConfigureAwait(false);
    }

    public async Task DeleteChannelGroup(DeleteChannelGroupRequest request)
    {
        await Sender.Send(request).ConfigureAwait(false);
    }


    public async Task<ChannelGroupDto?> GetChannelGroup(int id)
    {
        return await Sender.Send(new GetChannelGroup(id)).ConfigureAwait(false);
    }

    public async Task<IEnumerable<ChannelGroupIdName>> GetChannelGroupIdNames()
    {
        IEnumerable<ChannelGroupIdName> ret = await Sender.Send(new GetChannelGroupIdNames()).ConfigureAwait(false);
        return ret;
    }

    public async Task<IEnumerable<string>> GetChannelGroupNames()
    {
        IEnumerable<string> ret = await Sender.Send(new GetChannelGroupNames()).ConfigureAwait(false);
        return ret;
    }

    public async Task<PagedResponse<ChannelGroupDto>> GetPagedChannelGroups(ChannelGroupParameters channelGroupParameters)
    {
        PagedResponse<ChannelGroupDto> ret = await Sender.Send(new GetPagedChannelGroups(channelGroupParameters)).ConfigureAwait(false);
        return ret;
    }

    public async Task<List<ChannelGroupDto>> GetChannelGroupsForStreamGroup(GetChannelGroupsForStreamGroupRequest request)
    {
        List<ChannelGroupDto> ret = await Sender.Send(request).ConfigureAwait(false);
        return ret;
    }

    public async Task UpdateChannelGroup(UpdateChannelGroupRequest request)
    {
        await Sender.Send(request).ConfigureAwait(false);
    }

    public async Task UpdateChannelGroups(UpdateChannelGroupsRequest request)
    {
        await Sender.Send(request).ConfigureAwait(false);
    }


}