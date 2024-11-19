namespace StreamMaster.Application.Logos.CommandsOld;

public record BuildLogoCachesRequest : IRequest { }

public class BuildLogoCachesRequestHandler(ILogoService logoService, IOptionsMonitor<Setting> intSettings) : IRequestHandler<BuildLogoCachesRequest>
{
    private readonly Setting settings = intSettings.CurrentValue;

    public Task Handle(BuildLogoCachesRequest request, CancellationToken cancellationToken)
    {
        if (!settings.LogoCache.EqualsIgnoreCase("cache"))
        {
            return Task.CompletedTask;
        }

        logoService.CacheSMChannelLogos();

        return Task.CompletedTask;
    }
}
