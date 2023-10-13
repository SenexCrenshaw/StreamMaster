using StreamMasterApplication.Programmes;

using StreamMasterDomain.Enums;

namespace StreamMasterInfrastructure.Services;

public partial class BackgroundTaskQueue : IProgrammeChannelTasks
{
    public async ValueTask SetSDProgramme(CancellationToken cancellationToken = default)
    {
        await QueueAsync(SMQueCommand.SetSDProgramme, cancellationToken).ConfigureAwait(false);
    }
}
