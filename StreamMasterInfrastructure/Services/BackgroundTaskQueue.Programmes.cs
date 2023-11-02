using StreamMasterApplication.Programmes;

using StreamMasterDomain.Enums;

namespace StreamMasterInfrastructure.Services;

public partial class BackgroundTaskQueue : IProgrammeChannelTasks
{
    public async ValueTask SDSync(CancellationToken cancellationToken = default)
    {
        await QueueAsync(SMQueCommand.SDSync, cancellationToken).ConfigureAwait(false);
    }
}
