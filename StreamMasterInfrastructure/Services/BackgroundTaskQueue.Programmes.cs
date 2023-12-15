using StreamMasterApplication.Programmes;

using StreamMasterDomain.Enums;

namespace StreamMasterInfrastructure.Services;

public partial class BackgroundTaskQueue : IProgrammeChannelTasks
{
    public async ValueTask EPGSync(CancellationToken cancellationToken = default)
    {
        await QueueAsync(SMQueCommand.EPGSync, cancellationToken).ConfigureAwait(false);
    }
}
