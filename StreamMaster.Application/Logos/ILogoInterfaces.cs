
namespace StreamMaster.Application.Logos;

public interface ILogoTasks
{
    ValueTask CacheChannelLogos(CancellationToken cancellationToken = default);
    ValueTask CacheStreamLogos(CancellationToken cancellationToken = default);
    ValueTask ScanForTvLogos(CancellationToken cancellationToken = default);
}