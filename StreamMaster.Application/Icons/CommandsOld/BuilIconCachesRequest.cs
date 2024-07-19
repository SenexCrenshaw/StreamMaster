using StreamMaster.Application.Icons.Commands;

namespace StreamMaster.Application.Icons.CommandsOld;

public record BuildIconCachesRequest : IRequest { }

public class BuildIconCachesRequestHandler(ISender Sender, IOptionsMonitor<Setting> intSettings) : IRequestHandler<BuildIconCachesRequest>
{
    private readonly Setting settings = intSettings.CurrentValue;

    public async Task Handle(BuildIconCachesRequest request, CancellationToken cancellationToken)
    {

        if (!settings.CacheIcons)
        {
            return;
        }
        _ = await Sender.Send(new BuildIconsCacheFromVideoStreamRequest(), cancellationToken).ConfigureAwait(false);
    }
}
