using StreamMaster.Application.Custom;
using StreamMaster.Domain.Enums;

namespace StreamMaster.Infrastructure.Services.QueueService;


public partial class BackgroundTaskQueue : ICustomPlayListsTasks
{
    public async ValueTask ScanForCustomPlayLists(CancellationToken cancellationToken = default)
    {
        await QueueAsync(SMQueCommand.ScanForCustomPlayLists, cancellationToken).ConfigureAwait(false);
    }

}
