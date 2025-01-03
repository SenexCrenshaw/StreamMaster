using StreamMaster.Application.StreamGroups;
using StreamMaster.Domain.Enums;

namespace StreamMaster.Infrastructure.Services.QueueService;

public partial class BackgroundTaskQueue : IStreamGroupTasks
{
    public async ValueTask CreateSTRMFiles(CancellationToken cancellationToken = default)
    {
        await QueueAsync(SMQueCommand.CreateSTRMFiles, cancellationToken).ConfigureAwait(false);
    }
}