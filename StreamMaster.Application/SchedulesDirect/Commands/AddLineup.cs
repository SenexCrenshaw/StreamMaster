namespace StreamMaster.Application.SchedulesDirect.Commands;

public record AddLineup(string lineup) : IRequest<bool>;

public class AddLineupHandler(ISchedulesDirect schedulesDirect, IJobStatusService jobStatusService, ILogger<AddLineup> logger, IMemoryCache memoryCache)
: IRequestHandler<AddLineup, bool>
{
    public async Task<bool> Handle(AddLineup request, CancellationToken cancellationToken)
    {
        Setting setting = memoryCache.GetSetting();
        if (!setting.SDSettings.SDEnabled)
        {
            return false;
        }
        logger.LogInformation("Add line up {lineup}", request.lineup);
        if (await schedulesDirect.AddLineup(request.lineup, cancellationToken).ConfigureAwait(false))
        {
            schedulesDirect.ResetCache("SubscribedLineups");
            jobStatusService.SetSyncForceNextRun();
            //await HubContext.Clients.All.SchedulesDirectsRefresh();
            return true;
        }
        return false;
    }
}