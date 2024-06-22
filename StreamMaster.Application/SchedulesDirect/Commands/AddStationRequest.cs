using StreamMaster.Application.Settings.Commands;

namespace StreamMaster.Application.SchedulesDirect.Commands;

[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record StationRequest(string StationId, string LineUp);

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record AddStationRequest(List<StationRequest> Requests) : IRequest<APIResponse>;

public class AddStationHandler(ILogger<AddStationRequest> logger, IJobStatusService jobStatusService, ISchedulesDirect schedulesDirect, ISender Sender, IOptionsMonitor<SDSettings> intsettings)
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

        UpdateSettingParameters updateSettingRequest = new()
        {
            SDSettings = new SDSettingsRequest
            {
                SDStationIds = sdsettings.SDStationIds
            }
        };

        foreach (StationRequest stationRequest in request.Requests)
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

        _ = await Sender.Send(updateSettingRequest, cancellationToken).ConfigureAwait(false);

        schedulesDirect.ResetCache("SubscribedLineups");
        jobManager.SetForceNextRun();

        return APIResponse.Ok;
    }
}