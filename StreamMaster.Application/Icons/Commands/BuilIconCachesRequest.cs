using StreamMaster.Domain.Configuration;

namespace StreamMaster.Application.Icons.Commands;

public record BuildIconCachesRequest : IRequest { }

public class BuildIconCachesRequestHandler(ISender Sender, IOptionsMonitor<Setting> intsettings) : IRequestHandler<BuildIconCachesRequest>
{
    private readonly Setting settings = intsettings.CurrentValue;

    public async Task Handle(BuildIconCachesRequest request, CancellationToken cancellationToken)
    {

        if (!settings.CacheIcons)
        {
            return;
        }
        _ = await Sender.Send(new BuildIconsCacheFromVideoStreamRequest(), cancellationToken).ConfigureAwait(false);
        //_ = await Sender.Send(new BuildProgIconsCacheFromEPGsRequest(), cancellationToken).ConfigureAwait(false);
    }
}
