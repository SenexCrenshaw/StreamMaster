namespace StreamMaster.Application.SchedulesDirect.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record RemoveLineupRequest(string Lineup) : IRequest<APIResponse>;

public class RemoveLineupRequestHandler(ISchedulesDirect schedulesDirect, IJobStatusService jobStatusService, ILogger<RemoveLineupRequest> logger, IHubContext<StreamMasterHub, IStreamMasterHub> HubContext, IOptionsMonitor<SDSettings> intsettings)
: IRequestHandler<RemoveLineupRequest, APIResponse>
{
    private readonly SDSettings sdsettings = intsettings.CurrentValue;

    public async Task<APIResponse> Handle(RemoveLineupRequest request, CancellationToken cancellationToken)
    {
        JobStatusManager jobManager = jobStatusService.GetJobManager(JobType.SDSync, EPGHelper.SchedulesDirectId);

        if (!sdsettings.SDEnabled)
        {
            return APIResponse.ErrorWithMessage("Sd is not enabled");
        }
        logger.LogInformation("Remove line up {lineup}", request.Lineup);
        if (await schedulesDirect.RemoveLineup(request.Lineup, cancellationToken).ConfigureAwait(false))
        {
            APIResponse response = await schedulesDirect.SDSync(cancellationToken);
            //if (!response.IsError)
            //{
            //    await HubContext.Clients.All.SchedulesDirectsRefresh();

            //}
            //schedulesDirect.ResetCache(SDCommands.Status);
            //schedulesDirect.ResetCache(SDCommands.LineUps);
            //await hubContext.Clients.All.SchedulesDirectsRefresh();
            schedulesDirect.ResetCache("SubscribedLineups");
            jobManager.SetForceNextRun();

            return APIResponse.Ok;
        }
        return APIResponse.Ok;
    }
}