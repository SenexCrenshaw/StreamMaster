using StreamMaster.Application.Settings.Commands;

namespace StreamMaster.Application.SchedulesDirect.Commands;


public record RemoveStation(List<StationRequest> Requests) : IRequest<bool>;

public class RemoveStationHandler(ILogger<RemoveStation> logger, IJobStatusService jobStatusService, ISchedulesDirect schedulesDirect, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache)
: BaseMediatorRequestHandler(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache), IRequestHandler<RemoveStation, bool>
{
    public async Task<bool> Handle(RemoveStation request, CancellationToken cancellationToken)
    {
        Setting setting = await GetSettingsAsync().ConfigureAwait(false);
        if (!setting.SDSettings.SDEnabled)
        {
            return false;
        }

        UpdateSettingRequest updateSettingRequest = new()
        {
            SDSettings = new SDSettingsRequest
            {
                SDStationIds = setting.SDSettings.SDStationIds
            }
        };

        List<string> toDelete = [];
        foreach (StationRequest stationRequest in request.Requests)
        {
            StationIdLineup station = new(stationRequest.StationId, stationRequest.LineUp);
            StationIdLineup? existing = updateSettingRequest.SDSettings.SDStationIds.FirstOrDefault(x => x.Lineup == station.Lineup && x.StationId == station.StationId);
            if (existing == null)
            {
                logger.LogInformation("Remove Station: Does not exists {StationIdLineup}", station.StationId);
                continue;
            }
            logger.LogInformation("Remove Station {StationIdLineup}", station.StationId);
            _ = updateSettingRequest.SDSettings.SDStationIds.Remove(existing);
            toDelete.Add(station.StationId);
        }


        if (toDelete.Count > 0)
        {
            _ = await Sender.Send(updateSettingRequest, cancellationToken).ConfigureAwait(false);

            schedulesDirect.ResetEPGCache();
            jobStatusService.SetSyncForceNextRun(Extra: true);

            //foreach (EPGFileDto epg in await Repository.EPGFile.GetEPGFiles())
            //{
            //    await Sender.Send(new RefreshEPGFileRequest(epg.Id), cancellationToken).ConfigureAwait(false);
            //}
        }

        return true;
    }
}