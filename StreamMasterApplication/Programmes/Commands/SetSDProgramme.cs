using StreamMasterApplication.Services;

using StreamMasterDomain.EPG;

namespace StreamMasterApplication.Programmes.Commands;

public record SetSDProgramme() : IRequest;

public class SetSDProgrammeHandler(ISDService sDService, ILogger<SetSDProgramme> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache)
: BaseMediatorRequestHandler(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache), IRequestHandler<SetSDProgramme>
{
    public async Task Handle(SetSDProgramme request, CancellationToken cancellationToken)
    {
        Setting setting = await GetSettingsAsync();
        if (!setting.SDEnabled)
        {
            return;
        }

        List<Programme> res = await sDService.GetProgrammes(cancellationToken).ConfigureAwait(false);
        MemoryCache.SetSDProgreammesCache(res, TimeSpan.FromHours(4));
    }
}
