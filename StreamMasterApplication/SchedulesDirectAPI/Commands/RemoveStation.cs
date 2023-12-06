using StreamMaster.SchedulesDirectAPI.Domain.Models;

using StreamMasterApplication.Settings.Commands;

namespace StreamMasterApplication.SchedulesDirectAPI.Commands;


public record RemoveStation(List<StationRequest> Requests) : IRequest<bool>;

public class RemoveStationHandler( ILogger<RemoveStation> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache)
: BaseMediatorRequestHandler(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache), IRequestHandler<RemoveStation, bool>
{
    public async Task<bool> Handle(RemoveStation request, CancellationToken cancellationToken)
    {
        Setting setting = await GetSettingsAsync().ConfigureAwait(false);
        if (!setting.SDSettings.SDEnabled)
        {
            return false;
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
            var existing = updateSettingRequest.SDSettings.SDStationIds.FirstOrDefault(x => x.Lineup == station.Lineup && x.StationId == station.StationId);
            if (existing == null)
            {
                logger.LogInformation("Remove Station: Does not exists {StationIdLineup}", station.StationId);
                continue;
            }
            logger.LogInformation("Remove Station {StationIdLineup}", station.StationId);
            updateSettingRequest.SDSettings.SDStationIds.Remove(existing);
        }
       
        await Sender.Send(updateSettingRequest, cancellationToken).ConfigureAwait(false);
        return true;
    }
}