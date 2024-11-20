using StreamMaster.Application.Services;
using StreamMaster.Application.Settings.Commands;

namespace StreamMaster.Application.SchedulesDirect.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record SetStationsRequest(List<StationRequest> Requests) : IRequest<APIResponse>;

public class SetStationsHandler(ILogger<SetStationsRequest> logger, IDataRefreshService dataRefreshService, IBackgroundTaskQueue backgroundTaskQueue, IJobStatusService jobStatusService, ISchedulesDirect schedulesDirect, ISender Sender, IOptionsMonitor<SDSettings> _sdSettings)
: IRequestHandler<SetStationsRequest, APIResponse>
{
    public async Task<APIResponse> Handle(SetStationsRequest request, CancellationToken cancellationToken)
    {
        if (request.Requests.Count == 0)
        {
            return APIResponse.Ok;
        }

        if (!_sdSettings.CurrentValue.SDEnabled)
        {
            return APIResponse.Ok;
        }

        UpdateSettingParameters updateSetting = new()
        {
            SDSettings = new SDSettingsRequest
            {
                SDStationIds = _sdSettings.CurrentValue.SDStationIds
            }
        };

        bool changed = false;

        foreach (StationRequest stationRequest in request.Requests)
        {
            if (updateSetting.SDSettings.SDStationIds.Any(x => x.Lineup == stationRequest.Lineup && x.StationId == stationRequest.StationId))
            {
                continue;
            }
            logger.LogInformation("Added Station {StationIdLineup}", stationRequest.StationId);
            StationIdLineup station = new(stationRequest.StationId, stationRequest.Lineup);
            updateSetting.SDSettings.SDStationIds.Add(station);
            changed = true;
        }

        List<StationIdLineup> newStatsions = updateSetting.SDSettings.SDStationIds.DeepCopy();

        foreach (StationIdLineup station in newStatsions)
        {
            if (request.Requests.Any(a => a.StationId == station.StationId && a.Lineup == station.Lineup))
            {
                continue;
            }
            logger.LogInformation("Removed Station {StationIdLineup}", station.StationId);
            updateSetting.SDSettings.SDStationIds.Remove(station);
            changed = true;
        }

        if (changed)
        {
            _ = await Sender.Send(new UpdateSettingRequest(updateSetting), cancellationToken).ConfigureAwait(false);

            schedulesDirect.ResetCache("SubscribedLineups");

            JobStatusManager jobManager = jobStatusService.GetJobManageSDSync(EPGHelper.SchedulesDirectId);
            await backgroundTaskQueue.EPGSync(cancellationToken).ConfigureAwait(false);
            jobManager.SetForceNextRun();
            await dataRefreshService.RefreshSchedulesDirect();
        }
        return APIResponse.Ok;
    }
}