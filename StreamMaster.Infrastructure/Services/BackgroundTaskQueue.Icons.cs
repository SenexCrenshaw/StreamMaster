using StreamMaster.Domain.Enums;

using StreamMaster.Application.Icons;

namespace StreamMaster.Infrastructure.Services;

public partial class BackgroundTaskQueue : IIconTasks
{
    public async ValueTask BuildIconCaches(CancellationToken cancellationToken = default)
    {
        await QueueAsync(SMQueCommand.BuildIconCaches, cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask BuildIconsCacheFromVideoStreams(CancellationToken cancellationToken = default)
    {
        await QueueAsync(SMQueCommand.BuildIconsCacheFromVideoStreams, cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask BuildProgIconsCacheFromEPGs(CancellationToken cancellationToken = default)
    {
        await QueueAsync(SMQueCommand.BuildProgIconsCacheFromEPGs, cancellationToken).ConfigureAwait(false);
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
