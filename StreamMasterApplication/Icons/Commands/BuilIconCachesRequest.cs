namespace StreamMasterApplication.Icons.Commands;

public record BuildIconCachesRequest : IRequest { }

public class BuildIconCachesRequestHandler(ILogger<BuildIconCachesRequest> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext)
: BaseMediatorRequestHandler(logger, repository, mapper, settingsService, publisher, sender, hubContext), IRequestHandler<BuildIconCachesRequest>
{
    public async Task Handle(BuildIconCachesRequest request, CancellationToken cancellationToken)
    {
        Setting setting = await GetSettingsAsync();
        if (!setting.CacheIcons)
        {
            return;
        }
        _ = await Sender.Send(new BuildIconsCacheFromVideoStreamRequest(), cancellationToken).ConfigureAwait(false);
        _ = await Sender.Send(new BuildProgIconsCacheFromEPGsRequest(), cancellationToken).ConfigureAwait(false);
    }
}
