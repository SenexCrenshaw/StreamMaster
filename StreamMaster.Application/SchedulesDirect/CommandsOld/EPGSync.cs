namespace StreamMaster.Application.SchedulesDirect.CommandsOld;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record EPGSync() : IRequest<bool>;

public class SDSyncHandler(ISchedulesDirect schedulesDirect, ILogger<EPGSync> logger, IHubContext<StreamMasterHub, IStreamMasterHub> HubContext, IOptionsMonitor<SDSettings> intsettings)
: IRequestHandler<EPGSync, bool>
{
    private readonly SDSettings settings = intsettings.CurrentValue;

    public async Task<bool> Handle(EPGSync request, CancellationToken cancellationToken)
    {

        if (settings.SDEnabled)
        {
            APIResponse response = await schedulesDirect.SDSync(cancellationToken).ConfigureAwait(false);
            if (!response.IsError)
            {
                logger.LogInformation("Updated Schedules Direct");
                await HubContext.Clients.All.DataRefresh("SchedulesDirect");

            }
        }

        return true;
    }
}