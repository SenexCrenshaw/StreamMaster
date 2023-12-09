using StreamMaster.SchedulesDirectAPI.Domain.Models;

using StreamMasterApplication.Settings.Commands;

namespace StreamMasterApplication.SchedulesDirectAPI.Commands;

public record StationRequest(string StationId, string LineUp);

public record AddStation(List<StationRequest> Requests) : IRequest<bool>;

public class AddStationHandler(ILogger<AddStation> logger, ISchedulesDirect schedulesDirect, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache)
: BaseMediatorRequestHandler(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache), IRequestHandler<AddStation, bool>
{
    public async Task<bool> Handle(AddStation request, CancellationToken cancellationToken)
    {
        if (!request.Requests.Any())
        {
            return true;
        }

        Setting setting = await GetSettingsAsync().ConfigureAwait(false);
        if (!setting.SDSettings.SDEnabled)
        {
            return true;
        }

        var updateSettingRequest = new UpdateSettingRequest
        {
            SDSettings = new SDSettingsRequest
            {
                SDStationIds = setting.SDSettings.SDStationIds
            }
        };

        foreach (var stationRequest in request.Requests)
        {
            StationIdLineup station = new(stationRequest.StationId, stationRequest.LineUp);
            if (updateSettingRequest.SDSettings.SDStationIds.Any(x => x.Lineup == station.Lineup && x.StationId == station.StationId))
            {
                logger.LogInformation("Add Station: Already exists {StationIdLineup}", station.StationId);
                continue;
            }
            logger.LogInformation("Added Station {StationIdLineup}", station.StationId);
            updateSettingRequest.SDSettings.SDStationIds.Add(station);
        }

        if (await schedulesDirect.SDSync(cancellationToken))
        {
            await HubContext.Clients.All.SchedulesDirectsRefresh();
            return true;
        }

        _ = await Sender.Send(updateSettingRequest, cancellationToken).ConfigureAwait(false);

        return true;
    }
}