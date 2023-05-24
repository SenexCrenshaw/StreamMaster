using StreamMasterApplication.Icons;

using StreamMasterDomain.Enums;

namespace StreamMasterInfrastructure.Services;

public partial class BackgroundTaskQueue : IIconTasks
{
    public async ValueTask CacheAllIcons(CancellationToken cancellationToken = default)
    {
        await QueueAsync(SMQueCommand.CacheAllIcons, cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask CacheIconsFromProgrammes(CancellationToken cancellationToken = default)
    {
        await QueueAsync(SMQueCommand.CacheIconsFromProgrammes, cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask CacheIconsFromVideoStreams(CancellationToken cancellationToken = default)
    {
        await QueueAsync(SMQueCommand.CacheIconsFromVideoStreams, cancellationToken).ConfigureAwait(false);
    }

    public async Task ReadDirectoryLogosRequest(CancellationToken cancellationToken = default)
    {
        await QueueAsync(SMQueCommand.ReadDirectoryLogosRequest, cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask ScanDirectoryForIconFiles(CancellationToken cancellationToken = default)
    {
        await QueueAsync(SMQueCommand.ScanDirectoryForIconFiles, cancellationToken).ConfigureAwait(false);
    }
}
