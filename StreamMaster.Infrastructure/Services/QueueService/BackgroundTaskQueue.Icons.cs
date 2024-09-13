using StreamMaster.Application.Icons;
using StreamMaster.Domain.Enums;

namespace StreamMaster.Infrastructure.Services.QueueService;


public partial class BackgroundTaskQueue : IIconTasks
{
    public async ValueTask BuildIconCaches(CancellationToken cancellationToken = default)
    {
        await QueueAsync(SMQueCommand.BuildIconCaches, cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask BuildLogosCacheFromStreams(CancellationToken cancellationToken = default)
    {
        await QueueAsync(SMQueCommand.BuildLogosCacheFromStreams, cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask BuildIconsCacheFromVideoStreams(CancellationToken cancellationToken = default)
    {
        await QueueAsync(SMQueCommand.BuildIconsCacheFromVideoStreams, cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask BuildProgIconsCacheFromEPGs(CancellationToken cancellationToken = default)
    {
        await QueueAsync(SMQueCommand.BuildProgIconsCacheFromEPGs, cancellationToken).ConfigureAwait(false);
    }

    public async Task ReadDirectoryLogos(CancellationToken cancellationToken = default)
    {
        await QueueAsync(SMQueCommand.ReadDirectoryLogosRequest, cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask ScanDirectoryForIconFiles(CancellationToken cancellationToken = default)
    {
        await QueueAsync(SMQueCommand.ScanDirectoryForIconFiles, cancellationToken).ConfigureAwait(false);
    }
}
