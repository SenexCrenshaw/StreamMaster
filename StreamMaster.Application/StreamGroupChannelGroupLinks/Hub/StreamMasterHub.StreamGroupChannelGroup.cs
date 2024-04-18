using StreamMaster.Application.StreamGroupChannelGroupLinks;
using StreamMaster.Application.StreamGroupChannelGroupLinks.Commands;

namespace StreamMaster.Application.Hubs;

public partial class StreamMasterHub : IStreamGroupChannelGroupHub
{
    public async Task SyncStreamGroupChannelGroups(SyncStreamGroupChannelGroupsRequest request, CancellationToken cancellationToken)
    {
        await Sender.Send(request, cancellationToken).ConfigureAwait(false);
    }
}