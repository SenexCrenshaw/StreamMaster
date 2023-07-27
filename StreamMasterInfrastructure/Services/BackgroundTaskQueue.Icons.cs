using StreamMasterApplication.Icons;

using StreamMasterDomain.Enums;

namespace StreamMasterInfrastructure.Services;

public partial class BackgroundTaskQueue : IIconTasks
{
    public async ValueTask BuildIconsCacheFromVideoStreams(CancellationToken cancellationToken = default)
    {
        await QueueAsync(SMQueCommand.BuildIconsCacheFromVideoStreams, cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask BuildProgIconsCacheFromEPGs(CancellationToken cancellationToken = default)
    {
        await QueueAsync(SMQueCommand.BuildProgIconsCacheFromEPGs, cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask BuilIconCaches(CancellationToken cancellationToken = default)
    {
        await QueueAsync(SMQueCommand.BuilIconCaches, cancellationToken).ConfigureAwait(false);
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
