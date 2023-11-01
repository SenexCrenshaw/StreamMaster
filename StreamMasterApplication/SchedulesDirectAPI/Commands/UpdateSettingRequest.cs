using StreamMaster.SchedulesDirectAPI.Domain.Models;

namespace StreamMasterApplication.SchedulesDirectAPI.Commands;

public record UpdateSDStationIds(List<StationIdLineUp> SDStationIds) : IRequest;

public class UpdateSDStationIdsHandler(ILogger<UpdateSDStationIds> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache)
: BaseMediatorRequestHandler(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache), IRequestHandler<UpdateSDStationIds>
{
    public async Task Handle(UpdateSDStationIds request, CancellationToken cancellationToken)
    {
        Setting currentSetting = await GetSettingsAsync();

        if (request.SDStationIds != null)
        {
            bool haveSameElements = new HashSet<StationIdLineUp>(currentSetting.SDStationIds).SetEquals(request.SDStationIds);
            if (!haveSameElements)
            {
                currentSetting.SDStationIds = request.SDStationIds;

                await HubContext.Clients.All.SchedulesDirectsRefresh().ConfigureAwait(false);
            }
        }

        FileUtil.UpdateSetting(currentSetting);
    }
}
