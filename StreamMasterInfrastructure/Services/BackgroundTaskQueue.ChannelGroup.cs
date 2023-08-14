using StreamMasterApplication.ChannelGroups;

using StreamMasterDomain.Enums;

namespace StreamMasterInfrastructure.Services;

public partial class BackgroundTaskQueue : IChannelGroupTasks
{
    public async ValueTask UpdateChannelGroupCounts(CancellationToken cancellationToken = default)
    {
        await QueueAsync(SMQueCommand.UpdateChannelGroupCounts, cancellationToken).ConfigureAwait(false);
    }
}
