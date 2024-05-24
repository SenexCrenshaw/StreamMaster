using StreamMaster.Application.Settings.Commands;

namespace StreamMaster.Application.SchedulesDirect.CommandsOld;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record RemoveStation(List<StationRequest> Requests) : IRequest<bool>;

public class RemoveStationHandler(ILogger<RemoveStation> logger, IJobStatusService jobStatusService, ISchedulesDirect schedulesDirect, ISender Sender, IOptionsMonitor<SDSettings> intsettings)
: IRequestHandler<RemoveStation, bool>
{
    private readonly SDSettings sdsettings = intsettings.CurrentValue;

    public async Task<bool> Handle(RemoveStation request, CancellationToken cancellationToken)
    {
        JobStatusManager jobManager = jobStatusService.GetJobManager(JobType.SDSync, EPGHelper.SchedulesDirectId);


        if (!sdsettings.SDEnabled)
        {
            return false;
        }

        UpdateSettingParameters updateSettingRequest = new()
        {
            SDSettings = new SDSettingsRequest
            {
                SDStationIds = sdsettings.SDStationIds
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
            jobManager.SetForceNextRun();

            //foreach (EPGFileDto epg in await Repository.EPGFile.GetEPGFiles())
            //{
            //    await Sender.Send(new RefreshEPGFileRequest(epg.Id), cancellationToken).ConfigureAwait(false);
            //}
        }

        return true;
    }
}