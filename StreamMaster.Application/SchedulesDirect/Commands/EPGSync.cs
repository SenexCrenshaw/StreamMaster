using StreamMaster.Domain.Configuration;

namespace StreamMaster.Application.SchedulesDirect.Commands;

public record EPGSync() : IRequest<bool>;

public class SDSyncHandler(ISchedulesDirect schedulesDirect, ILogger<EPGSync> logger, IHubContext<StreamMasterHub, IStreamMasterHub> HubContext, IOptionsMonitor<SDSettings> intsettings)
: IRequestHandler<EPGSync, bool>
{
    private readonly SDSettings settings = intsettings.CurrentValue;

    public async Task<bool> Handle(EPGSync request, CancellationToken cancellationToken)
    {

        if (settings.SDEnabled)
        {
            if (await schedulesDirect.SDSync(cancellationToken).ConfigureAwait(false))
            {
                logger.LogInformation("Updated Schedules Direct");
                await HubContext.Clients.All.SchedulesDirectsRefresh();
            }
        }

        return true;
    }
}