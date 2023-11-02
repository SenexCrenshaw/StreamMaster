using StreamMaster.SchedulesDirectAPI.Domain.EPG;
using StreamMaster.SchedulesDirectAPI.Domain.Interfaces;

namespace StreamMasterApplication.Programmes.Commands;

public record SetSDProgramme() : IRequest;

public class SetSDProgrammeHandler(ISDService sdService, ILogger<SetSDProgramme> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache)
: BaseMediatorRequestHandler(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache), IRequestHandler<SetSDProgramme>
{
    public async Task Handle(SetSDProgramme request, CancellationToken cancellationToken)
    {
        Setting setting = await GetSettingsAsync();
        if (!setting.SDEnabled)
        {
            return;
        }

        List<Programme> res = await sdService.GetProgrammes(setting.SDEPGDays, setting.SDMaxRatings, setting.SDUseLineUpInName, cancellationToken).ConfigureAwait(false);

        MemoryCache.SetSDProgreammesCache(res);
    }
}
