using StreamMaster.Application.Icons.Commands;

namespace StreamMaster.Application.Icons.CommandsOld;

public record BuildIconCachesRequest : IRequest { }

public class BuildIconCachesRequestHandler(ISender Sender, ILogoService logoService, IOptionsMonitor<Setting> intSettings) : IRequestHandler<BuildIconCachesRequest>
{
    private readonly Setting settings = intSettings.CurrentValue;

    public async Task Handle(BuildIconCachesRequest request, CancellationToken cancellationToken)
    {
        _ = await Sender.Send(new BuildIconsCacheFromVideoStreamRequest(), cancellationToken).ConfigureAwait(false);

        if (!string.Equals(settings.LogoCache, "cache", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }
        await logoService.CacheSMChannelLogos();
    }
}
