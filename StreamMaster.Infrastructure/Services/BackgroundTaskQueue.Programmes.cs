using StreamMaster.Application.Programmes;
using StreamMaster.Domain.Enums;

namespace StreamMaster.Infrastructure.Services;

public partial class BackgroundTaskQueue : IProgrammeChannelTasks
{
    public async ValueTask EPGSync(CancellationToken cancellationToken = default)
    {
        await QueueAsync(SMQueCommand.EPGSync, cancellationToken).ConfigureAwait(false);
    }
}
