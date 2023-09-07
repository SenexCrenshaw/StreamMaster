using StreamMasterApplication.StreamGroupChannelGroups;
using StreamMasterApplication.StreamGroupChannelGroups.Commands;
using StreamMasterApplication.StreamGroupChannelGroups.Queries;

namespace StreamMasterApplication.Hubs;

public partial class StreamMasterHub : IStreamGroupChannelGroupHub
{
    public async Task<int> SyncStreamGroupChannelGroups(SyncStreamGroupChannelGroupsRequest request, CancellationToken cancellationToken)
    {
        return await _mediator.Send(request, cancellationToken).ConfigureAwait(false);
    }

    public async Task<IEnumerable<ChannelGroupDto>> GetChannelGroupsFromStreamGroup(GetChannelGroupsFromStreamGroupRequest request, CancellationToken cancellationToken)
    {
        return await _mediator.Send(request, cancellationToken).ConfigureAwait(false);
    }

    public async Task<IEnumerable<string>> RemoveStreamGroupChannelGroups(RemoveStreamGroupChannelGroupsRequest request, CancellationToken cancellationToken)
    {
        return await _mediator.Send(request, cancellationToken).ConfigureAwait(false);
    }
}