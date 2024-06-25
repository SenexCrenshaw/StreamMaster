using StreamMaster.Application.Settings.Commands;

namespace StreamMaster.Application.SchedulesDirect.Commands;

[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record StationRequest(string StationId, string Lineup, string Country, string PostalCode);

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record AddStationRequest(List<StationRequest> Requests) : IRequest<APIResponse>;

public class AddStationHandler(ILogger<AddStationRequest> logger, IDataRefreshService dataRefreshService, IJobStatusService jobStatusService, ISchedulesDirect schedulesDirect, ISender Sender, IOptionsMonitor<SDSettings> intsettings)
: IRequestHandler<AddStationRequest, APIResponse>
{
    private readonly SDSettings sdsettings = intsettings.CurrentValue;

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

        JobStatusManager jobManager = jobStatusService.GetJobManager(JobType.SDSync, EPGHelper.SchedulesDirectId);

        UpdateSettingParameters updateSetting = new()
        {
            SDSettings = new SDSettingsRequest
            {
                SDStationIds = sdsettings.SDStationIds
            }
        };

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
        }

        if (updateSetting.SDSettings.SDStationIds.Count > 0)
        {
            _ = await Sender.Send(new UpdateSettingRequest(updateSetting), cancellationToken).ConfigureAwait(false);

            schedulesDirect.ResetCache("SubscribedLineups");
            jobManager.SetForceNextRun();
            await dataRefreshService.RefreshSelectedStationIds();
        }
        return APIResponse.Ok;
    }
}