using StreamMaster.Application.Logos;
using StreamMaster.Domain.Enums;

namespace StreamMaster.Infrastructure.Services.QueueService;

public partial class BackgroundTaskQueue : ILogoTasks
{
    public async ValueTask BuildLogoCaches(CancellationToken cancellationToken = default)
    {
        await QueueAsync(SMQueCommand.BuildLogoCaches, cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask BuildLogosCacheFromStreams(CancellationToken cancellationToken = default)
    {
        await QueueAsync(SMQueCommand.BuildLogosCacheFromStreams, cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask BuildLogosCacheFromVideoStreams(CancellationToken cancellationToken = default)
    {
        await QueueAsync(SMQueCommand.BuildLogosCacheFromVideoStreams, cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask BuildProgLogosCacheFromEPGs(CancellationToken cancellationToken = default)
    {
        await QueueAsync(SMQueCommand.BuildProgLogosCacheFromEPGs, cancellationToken).ConfigureAwait(false);
    }

    public async Task ReadDirectoryLogos(CancellationToken cancellationToken = default)
    {
        await QueueAsync(SMQueCommand.ReadDirectoryLogosRequest, cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask ScanDirectoryForLogoFiles(CancellationToken cancellationToken = default)
    {
        await QueueAsync(SMQueCommand.ScanDirectoryForLogoFiles, cancellationToken).ConfigureAwait(false);
    }
}
