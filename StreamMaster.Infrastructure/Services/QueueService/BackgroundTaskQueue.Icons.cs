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

    //public async ValueTask BuildLogosCacheFromVideoStreams(CancellationToken cancellationToken = default)
    //{
    //    await QueueAsync(SMQueCommand.BuildLogosCacheFromVideoStreams, cancellationToken).ConfigureAwait(false);
    //}

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
