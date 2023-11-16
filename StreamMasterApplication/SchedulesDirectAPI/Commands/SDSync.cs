using StreamMaster.SchedulesDirectAPI.Domain.Interfaces;

namespace StreamMasterApplication.SchedulesDirectAPI.Commands;

public record SDSync() : IRequest<bool>;

public class SDSyncHandler(ISDService sdService, ILogger<SDSync> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache)
: BaseMediatorRequestHandler(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache), IRequestHandler<SDSync, bool>
{
    public async Task<bool> Handle(SDSync request, CancellationToken cancellationToken)
    {
        Setting setting = await GetSettingsAsync().ConfigureAwait(false);
        if (!setting.SDEnabled)
        {
            return false;
        }

        if (await sdService.SDSync(cancellationToken).ConfigureAwait(false))
        {
            logger.LogInformation("Synced Schedules Direct");
            await HubContext.Clients.All.SchedulesDirectsRefresh();
            return true;
        }
        return false;
    }
}