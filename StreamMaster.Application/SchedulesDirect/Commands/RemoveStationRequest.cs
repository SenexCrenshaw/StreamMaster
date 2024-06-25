using StreamMaster.Application.Settings.Commands;

namespace StreamMaster.Application.SchedulesDirect.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record RemoveStationRequest(List<StationRequest> Requests) : IRequest<APIResponse>;

public class RemoveStationRequestHandler(ILogger<RemoveStationRequest> logger, IDataRefreshService dataRefreshService, IJobStatusService jobStatusService, ISchedulesDirect schedulesDirect, ISender Sender, IOptionsMonitor<SDSettings> intsettings)
: IRequestHandler<RemoveStationRequest, APIResponse>
{
    private readonly SDSettings sdsettings = intsettings.CurrentValue;

    public async Task<APIResponse> Handle(RemoveStationRequest request, CancellationToken cancellationToken)
    {
        JobStatusManager jobManager = jobStatusService.GetJobManager(JobType.SDSync, EPGHelper.SchedulesDirectId);


        if (!sdsettings.SDEnabled)
        {
            return APIResponse.ErrorWithMessage("SD is not enabled");
        }

        UpdateSettingParameters updateSetting = new()
        {
            SDSettings = new SDSettingsRequest
            {
                SDStationIds = sdsettings.SDStationIds
            }
        };

        List<string> toDelete = [];
        foreach (StationRequest stationRequest in request.Requests)
        {
            //StationIdLineup station = new(stationRequest.StationId, stationRequest.LineUp, request.co);
            StationIdLineup? existing = updateSetting.SDSettings.SDStationIds.FirstOrDefault(x => x.Lineup == stationRequest.Lineup && x.StationId == stationRequest.StationId);
            if (existing == null)
            {
                logger.LogInformation("Remove Station: Does not exists {StationIdLineup}", stationRequest.StationId);
                continue;
            }
            logger.LogInformation("Remove Station {StationIdLineup}", stationRequest.StationId);
            _ = updateSetting.SDSettings.SDStationIds.Remove(existing);
            toDelete.Add(stationRequest.StationId);
        }



        if (toDelete.Count > 0)
        {
            _ = await Sender.Send(new UpdateSettingRequest(updateSetting), cancellationToken).ConfigureAwait(false);

            schedulesDirect.ResetEPGCache();
            jobManager.SetForceNextRun();
            await dataRefreshService.RefreshSelectedStationIds();


            //foreach (EPGFileDto epg in await Repository.EPGFile.GetEPGFiles())
            //{
            //    await Sender.Send(new RefreshEPGFileRequest(epg.Id), cancellationToken).ConfigureAwait(false);
            //}
        }

        return APIResponse.Ok;
    }
}