using StreamMaster.Application.Services;

namespace StreamMaster.Application.SchedulesDirect.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record AddLineupRequest(string Lineup) : IRequest<APIResponse>;

public class AddLineupRequestHandler(ISchedulesDirect schedulesDirect, IBackgroundTaskQueue backgroundTaskQueue, IMessageService messageService, IDataRefreshService dataRefreshService, IJobStatusService jobStatusService, ILogger<AddLineupRequest> logger, IOptionsMonitor<SDSettings> intSettings)
: IRequestHandler<AddLineupRequest, APIResponse>
{
    private readonly SDSettings sdSettings = intSettings.CurrentValue;

    public async Task<APIResponse> Handle(AddLineupRequest request, CancellationToken cancellationToken)
    {
        JobStatusManager jobManager = jobStatusService.GetJobManager(JobType.SDSync, EPGHelper.SchedulesDirectId);

        if (!sdSettings.SDEnabled)
        {
            return APIResponse.ErrorWithMessage("SD is not enabled");
        }
        logger.LogInformation("Add line up {lineup}", request.Lineup);
        int changesRemaining = await schedulesDirect.AddLineup(request.Lineup, cancellationToken).ConfigureAwait(false);
        if (changesRemaining > -1)
        {
            schedulesDirect.ResetCache("SubscribedLineups");
            jobManager.SetForceNextRun();
            await backgroundTaskQueue.EPGSync(cancellationToken).ConfigureAwait(false);
            //await dataRefreshService.Refresh("GetStationPreviews");
            //await dataRefreshService.Refresh("GetSubscribedLineup");
            //await dataRefreshService.Refresh("GetSelectedStationIds");
            await dataRefreshService.RefreshSchedulesDirect();
            await messageService.SendSuccess($"Subscribed lineup, {changesRemaining} changes remaining");
            return APIResponse.Ok;
        }
        return APIResponse.Ok;
    }
}