using StreamMaster.Application.StreamGroupChannelGroups;
using StreamMaster.Application.StreamGroupChannelGroups.Commands;

namespace StreamMaster.Application.Hubs;

public partial class StreamMasterHub : IStreamGroupChannelGroupHub
{
    public async Task SyncStreamGroupChannelGroups(SyncStreamGroupChannelGroupsRequest request, CancellationToken cancellationToken)
    {
        await mediator.Send(request, cancellationToken).ConfigureAwait(false);
    }
}