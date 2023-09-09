using StreamMasterApplication.StreamGroupChannelGroups;
using StreamMasterApplication.StreamGroupChannelGroups.Commands;
using StreamMasterApplication.StreamGroupChannelGroups.Queries;

namespace StreamMasterApplication.Hubs;

public partial class StreamMasterHub : IStreamGroupChannelGroupHub
{
    public async Task SyncStreamGroupChannelGroups(SyncStreamGroupChannelGroupsRequest request, CancellationToken cancellationToken)
    {
        await _mediator.Send(request, cancellationToken).ConfigureAwait(false);
    }


    public async Task<List<ChannelGroupDto>> GetAllChannelGroups(GetAllChannelGroupsRequest request)
    {
        return await _mediator.Send(request).ConfigureAwait(false);
    }
}