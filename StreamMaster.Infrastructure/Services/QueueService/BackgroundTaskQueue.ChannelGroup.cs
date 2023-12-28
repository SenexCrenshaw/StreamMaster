using StreamMaster.Application.ChannelGroups;
using StreamMaster.Domain.Enums;

namespace StreamMaster.Infrastructure.Services.QueueService;

public partial class BackgroundTaskQueue : IChannelGroupTasks
{
    public async ValueTask UpdateChannelGroupCounts(CancellationToken cancellationToken = default)
    {
        await QueueAsync(SMQueCommand.UpdateChannelGroupCounts, cancellationToken).ConfigureAwait(false);
    }
}
