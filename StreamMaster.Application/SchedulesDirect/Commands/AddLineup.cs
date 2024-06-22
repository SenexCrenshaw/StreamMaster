namespace StreamMaster.Application.SchedulesDirect.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record AddLineupRequest(string Lineup) : IRequest<APIResponse>;

public class AddLineupRequestHandler(ISchedulesDirect schedulesDirect, IJobStatusService jobStatusService, ILogger<AddLineupRequest> logger, IOptionsMonitor<SDSettings> intsettings)
: IRequestHandler<AddLineupRequest, APIResponse>
{
    private readonly SDSettings sdsettings = intsettings.CurrentValue;

    public async Task<APIResponse> Handle(AddLineupRequest request, CancellationToken cancellationToken)
    {
        JobStatusManager jobManager = jobStatusService.GetJobManager(JobType.SDSync, EPGHelper.SchedulesDirectId);


        if (!sdsettings.SDEnabled)
        {
            return APIResponse.ErrorWithMessage("SD is not enabled");
        }
        logger.LogInformation("Add line up {lineup}", request.Lineup);
        if (await schedulesDirect.AddLineup(request.Lineup, cancellationToken).ConfigureAwait(false))
        {
            schedulesDirect.ResetCache("SubscribedLineups");
            jobManager.SetForceNextRun();
            //await HubContext.Clients.All.SchedulesDirectsRefresh();
            return APIResponse.Ok;
        }
        return APIResponse.Ok;
    }
}