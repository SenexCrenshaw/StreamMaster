namespace StreamMaster.Application.SchedulesDirect.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record RemoveLineupRequest(string Lineup) : IRequest<APIResponse>;

public class RemoveLineupRequestHandler(ISchedulesDirect schedulesDirect, IDataRefreshService dataRefreshService, IMessageService messageService, IJobStatusService jobStatusService, ILogger<RemoveLineupRequest> logger, IOptionsMonitor<SDSettings> intSettings)
: IRequestHandler<RemoveLineupRequest, APIResponse>
{
    private readonly SDSettings sdSettings = intSettings.CurrentValue;

    public async Task<APIResponse> Handle(RemoveLineupRequest request, CancellationToken cancellationToken)
    {
        JobStatusManager jobManager = jobStatusService.GetJobManager(JobType.SDSync, EPGHelper.SchedulesDirectId);

        if (!sdSettings.SDEnabled)
        {
            return APIResponse.ErrorWithMessage("Sd is not enabled");
        }
        logger.LogInformation("Remove line up {lineup}", request.Lineup);
        int changesRemaining = await schedulesDirect.RemoveLineup(request.Lineup, cancellationToken).ConfigureAwait(false);

        if (changesRemaining > -1)
        {
            //APIResponse response = await schedulesDirect.SDSync(cancellationToken);
            //if (!response.IsErrored)
            //{
            //    await HubContext.ClientChannels.All.SchedulesDirectsRefresh();

            //}
            //schedulesDirect.ResetCache(SDCommands.Status);
            //schedulesDirect.ResetCache(SDCommands.LineUps);
            //await hubContext.ClientChannels.All.SchedulesDirectsRefresh();
            schedulesDirect.ResetCache("SubscribedLineups");

            jobManager.SetForceNextRun();
            //await dataRefreshService.Refresh("GetSubscribedLineup");
            //await dataRefreshService.Refresh("GetSelectedStationIds");
            await dataRefreshService.RefreshSchedulesDirect();
            await messageService.SendSuccess($"Unsubscribed lineup, {changesRemaining} changes remaining");
            return APIResponse.Ok;
        }
        return APIResponse.Ok;
    }
}