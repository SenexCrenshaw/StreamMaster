using StreamMaster.Application.Services;
using StreamMaster.Application.Settings.Commands;

namespace StreamMaster.Application.SchedulesDirect.Commands;

[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record StationRequest(string StationId, string Lineup);

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record AddStationRequest(List<StationRequest> Requests) : IRequest<APIResponse>;

public class AddStationHandler(ILogger<AddStationRequest> logger, IBackgroundTaskQueue backgroundTaskQueue, IDataRefreshService dataRefreshService, IJobStatusService jobStatusService, ISchedulesDirect schedulesDirect, ISender Sender, IOptionsMonitor<SDSettings> intSettings)
: IRequestHandler<AddStationRequest, APIResponse>
{
    private readonly SDSettings sdsettings = intSettings.CurrentValue;

    public async Task<APIResponse> Handle(AddStationRequest request, CancellationToken cancellationToken)
    {
        if (!request.Requests.Any())
        {
            return APIResponse.Ok;
        }


        if (!sdsettings.SDEnabled)
        {
            return APIResponse.Ok;
        }


        UpdateSettingParameters updateSetting = new()
        {
            SDSettings = new SDSettingsRequest
            {
                SDStationIds = sdsettings.SDStationIds
            }
        };

        bool changed = false;

        foreach (StationRequest stationRequest in request.Requests)
        {

            if (updateSetting.SDSettings.SDStationIds.Any(x => x.Lineup == stationRequest.Lineup && x.StationId == stationRequest.StationId))
            {
                logger.LogInformation("Add Station: Already exists {StationIdLineup}", stationRequest.StationId);
                continue;
            }
            logger.LogInformation("Added Station {StationIdLineup}", stationRequest.StationId);
            StationIdLineup station = new(stationRequest.StationId, stationRequest.Lineup);
            updateSetting.SDSettings.SDStationIds.Add(station);
            changed = true;
        }

        if (changed)
        {
            _ = await Sender.Send(new UpdateSettingRequest(updateSetting), cancellationToken).ConfigureAwait(false);

            schedulesDirect.ResetCache("SubscribedLineups");

            JobStatusManager jobManager = jobStatusService.GetJobManageSDSync(EPGHelper.SchedulesDirectId);
            jobManager.SetForceNextRun();
            await backgroundTaskQueue.EPGSync(cancellationToken).ConfigureAwait(false);

            await dataRefreshService.RefreshSchedulesDirect();
        }
        return APIResponse.Ok;
    }
}