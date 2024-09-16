namespace StreamMaster.Application.Logos.CommandsOld;

public record BuildLogoCachesRequest : IRequest { }

public class BuildLogoCachesRequestHandler(ILogoService logoService, IOptionsMonitor<Setting> intSettings) : IRequestHandler<BuildLogoCachesRequest>
{
    private readonly Setting settings = intSettings.CurrentValue;

    public async Task Handle(BuildLogoCachesRequest request, CancellationToken cancellationToken)
    {

        if (!string.Equals(settings.LogoCache, "cache", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        //_ = await Sender.Send(new BuildLogosCacheFromStreamsRequest(), cancellationToken).ConfigureAwait(false);

        logoService.CacheSMChannelLogos();
    }
}
