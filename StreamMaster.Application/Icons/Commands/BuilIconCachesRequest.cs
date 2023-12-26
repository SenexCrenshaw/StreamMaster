using StreamMaster.Domain.Common;
using StreamMaster.Domain.Repository;
using StreamMaster.Domain.Services;

namespace StreamMaster.Application.Icons.Commands;

public record BuildIconCachesRequest : IRequest { }

public class BuildIconCachesRequestHandler(ILogger<BuildIconCachesRequest> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache)
: BaseMediatorRequestHandler(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache), IRequestHandler<BuildIconCachesRequest>
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
