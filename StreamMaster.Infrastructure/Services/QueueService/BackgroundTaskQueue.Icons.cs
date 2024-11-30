using StreamMaster.Application.Logos;
using StreamMaster.Domain.Enums;

namespace StreamMaster.Infrastructure.Services.QueueService;

public partial class BackgroundTaskQueue : ILogoTasks
{
    public async ValueTask CacheChannelLogos(CancellationToken cancellationToken = default)
    {
        await QueueAsync(SMQueCommand.CacheChannelLogos, cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask CacheStreamLogos(CancellationToken cancellationToken = default)
    {
        await QueueAsync(SMQueCommand.CacheStreamLogos, cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask ScanForTvLogos(CancellationToken cancellationToken = default)
    {
        await QueueAsync(SMQueCommand.ScanForTvLogos, cancellationToken).ConfigureAwait(false);
    }
}
