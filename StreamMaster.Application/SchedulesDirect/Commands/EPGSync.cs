namespace StreamMaster.Application.SchedulesDirect.Commands;

public record EPGSync() : IRequest<bool>;

public class SDSyncHandler(ISchedulesDirect schedulesDirect, ILogger<EPGSync> logger, IHubContext<StreamMasterHub, IStreamMasterHub> HubContext, IMemoryCache memoryCache)
: IRequestHandler<EPGSync, bool>
{
    public async Task<bool> Handle(EPGSync request, CancellationToken cancellationToken)
    {
        Setting setting = memoryCache.GetSetting();
        if (setting.SDSettings.SDEnabled)
        {
            if (await schedulesDirect.SDSync(0, cancellationToken).ConfigureAwait(false))
            {
                logger.LogInformation("Updated Schedules Direct");
                await HubContext.Clients.All.SchedulesDirectsRefresh();
            }
        }

        return true;
    }
}