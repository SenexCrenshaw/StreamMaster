using StreamMasterApplication.StreamGroupChannelGroups;
using StreamMasterApplication.StreamGroupChannelGroups.Commands;

namespace StreamMasterApplication.Hubs;

public partial class StreamMasterHub : IStreamGroupChannelGroupHub
{
    public async Task SyncStreamGroupChannelGroups(SyncStreamGroupChannelGroupsRequest request, CancellationToken cancellationToken)
    {
        await mediator.Send(request, cancellationToken).ConfigureAwait(false);
    }
}