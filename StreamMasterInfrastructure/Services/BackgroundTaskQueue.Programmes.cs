using StreamMasterApplication.Programmes;

using StreamMasterDomain.Enums;

namespace StreamMasterInfrastructure.Services;

public partial class BackgroundTaskQueue : IProgrammeChannelTasks
{
    public async ValueTask RebuildProgrammeChannelNames(CancellationToken cancellationToken = default)
    {
        await QueueAsync(SMQueCommand.RebuildProgrammeChannelNames, cancellationToken).ConfigureAwait(false);
    }
}
