using StreamMaster.Application.Services;
using StreamMaster.Application.Settings.Commands;

namespace StreamMaster.Application.SchedulesDirect.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record RemoveStationRequest(List<StationRequest> Requests) : IRequest<APIResponse>;

public class RemoveStationRequestHandler(ILogger<RemoveStationRequest> logger, IBackgroundTaskQueue backgroundTaskQueue, IDataRefreshService dataRefreshService, IJobStatusService jobStatusService, ISchedulesDirect schedulesDirect, ISender Sender, IOptionsMonitor<SDSettings> intSettings)
: IRequestHandler<RemoveStationRequest, APIResponse>
{
    private readonly SDSettings sdsettings = intSettings.CurrentValue;

    public async Task<APIResponse> Handle(RemoveStationRequest request, CancellationToken cancellationToken)
    {
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

            StationIdLineup? existing = updateSetting.SDSettings.SDStationIds.FirstOrDefault(x => x.Lineup == stationRequest.Lineup && x.StationId == stationRequest.StationId);
            if (existing == null)
            {
                logger.LogInformation("Remove Station: No stations exist {StationIdLineup}", stationRequest.StationId);
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
            //schedulesDirect.ClearAllCaches();
            JobStatusManager jobManager = jobStatusService.GetJobManageSDSync(EPGHelper.SchedulesDirectId);
            jobManager.SetForceNextRun();
            await backgroundTaskQueue.EPGSync(cancellationToken).ConfigureAwait(false);
            await dataRefreshService.RefreshSchedulesDirect();


            //foreach (EPGFileDto epg in await Repositorywrapper.EPGFile.GetEPGFiles())
            //{
            //    await Sender.Send(new RefreshEPGFileRequest(epg.Id), cancellationToken).ConfigureAwait(false);
            //}
        }

        return APIResponse.Ok;
    }
}