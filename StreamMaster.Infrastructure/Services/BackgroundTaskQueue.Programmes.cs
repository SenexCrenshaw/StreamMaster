using StreamMaster.Domain.Enums;

using StreamMaster.Application.Programmes;

namespace StreamMaster.Infrastructure.Services;

public partial class BackgroundTaskQueue : IProgrammeChannelTasks
{
    public async ValueTask EPGSync(CancellationToken cancellationToken = default)
    {
        await QueueAsync(SMQueCommand.EPGSync, cancellationToken).ConfigureAwait(false);
    }
}
