using StreamMaster.SchedulesDirectAPI.Domain.Interfaces;

namespace StreamMasterApplication.SchedulesDirectAPI.Commands;

public record SDSync() : IRequest;

public class SDSyncHandler(ISDService sdService, ILogger<SDSync> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache)
: BaseMediatorRequestHandler(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache), IRequestHandler<SDSync>
{
    public async Task Handle(SDSync request, CancellationToken cancellationToken)
    {
        Setting setting = await GetSettingsAsync().ConfigureAwait(false);
        if (!setting.SDEnabled)
        {
            return;
        }
        logger.LogInformation("Syncing Schedules Direct");
        await sdService.SDSync(cancellationToken).ConfigureAwait(false);
    }
}
