using StreamMaster.SchedulesDirect.Helpers;

namespace StreamMaster.Application.SchedulesDirect.Commands;

public record RemoveLineup(string lineup) : IRequest<bool>;

public class RemoveLineupHandler(ISchedulesDirect schedulesDirect, IJobStatusService jobStatusService, ILogger<RemoveLineup> logger, IHubContext<StreamMasterHub, IStreamMasterHub> HubContext, IMemoryCache memoryCache)
: IRequestHandler<RemoveLineup, bool>
{
    public async Task<bool> Handle(RemoveLineup request, CancellationToken cancellationToken)
    {
        JobStatusManager jobManager = jobStatusService.GetJobManager(JobType.SDSync, EPGHelper.SchedulesDirectId);
        Setting setting = memoryCache.GetSetting();
        if (!setting.SDSettings.SDEnabled)
        {
            return false;
        }
        logger.LogInformation("Remove line up {lineup}", request.lineup);
        if (await schedulesDirect.RemoveLineup(request.lineup, cancellationToken).ConfigureAwait(false))
        {
            if (await schedulesDirect.SDSync(cancellationToken))
            {
                await HubContext.Clients.All.SchedulesDirectsRefresh();

            }
            //schedulesDirect.ResetCache(SDCommands.Status);
            //schedulesDirect.ResetCache(SDCommands.LineUps);
            //await hubContext.Clients.All.SchedulesDirectsRefresh();
            schedulesDirect.ResetCache("SubscribedLineups");
            jobManager.SetForceNextRun();

            return true;
        }
        return false;
    }
}