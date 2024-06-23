namespace StreamMaster.Application.SchedulesDirect.Commands;


public record EPGSync() : IRequest<APIResponse>;

public class EPGSyncHandler(ISchedulesDirect schedulesDirect, ILogger<EPGSync> logger, IHubContext<StreamMasterHub, IStreamMasterHub> HubContext, IOptionsMonitor<SDSettings> intsettings)
: IRequestHandler<EPGSync, APIResponse>
{
    private readonly SDSettings settings = intsettings.CurrentValue;

    public async Task<APIResponse> Handle(EPGSync request, CancellationToken cancellationToken)
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
        else
        {
            await HubContext.Clients.All.DataRefresh("GetStationChannelNames");
        }
        await HubContext.Clients.All.DataRefresh("GetEPGFiles");


        return APIResponse.Ok;
    }
}