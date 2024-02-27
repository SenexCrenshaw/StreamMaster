using StreamMaster.Domain.Configuration;

namespace StreamMaster.Application.SchedulesDirect.Commands;

public record AddLineup(string lineup) : IRequest<bool>;

public class AddLineupHandler(ISchedulesDirect schedulesDirect, IJobStatusService jobStatusService, ILogger<AddLineup> logger, IOptionsMonitor<SDSettings> intsettings)
: IRequestHandler<AddLineup, bool>
{
    private readonly SDSettings sdsettings = intsettings.CurrentValue;

    public async Task<bool> Handle(AddLineup request, CancellationToken cancellationToken)
    {
        JobStatusManager jobManager = jobStatusService.GetJobManager(JobType.SDSync, EPGHelper.SchedulesDirectId);


        if (!sdsettings.SDEnabled)
        {
            return false;
        }
        logger.LogInformation("Add line up {lineup}", request.lineup);
        if (await schedulesDirect.AddLineup(request.lineup, cancellationToken).ConfigureAwait(false))
        {
            schedulesDirect.ResetCache("SubscribedLineups");
            jobManager.SetForceNextRun();
            //await HubContext.Clients.All.SchedulesDirectsRefresh();
            return true;
        }
        return false;
    }
}