namespace StreamMaster.Application.Icons.CommandsOld;

public record BuildIconCachesRequest : IRequest { }

public class BuildIconCachesRequestHandler(ISender Sender, ILogoService logoService, IOptionsMonitor<Setting> intSettings) : IRequestHandler<BuildIconCachesRequest>
{
    private readonly Setting settings = intSettings.CurrentValue;

    public async Task Handle(BuildIconCachesRequest request, CancellationToken cancellationToken)
    {

        if (!string.Equals(settings.LogoCache, "cache", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        //_ = await Sender.Send(new BuildLogosCacheFromStreamsRequest(), cancellationToken).ConfigureAwait(false);

        await logoService.CacheSMChannelLogos();
    }
}
