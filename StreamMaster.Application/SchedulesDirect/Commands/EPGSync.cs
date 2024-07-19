namespace StreamMaster.Application.SchedulesDirect.Commands;


public record EPGSync() : IRequest<APIResponse>;

public class EPGSyncHandler(ISchedulesDirect schedulesDirect, ILogger<EPGSync> logger, IDataRefreshService dataRefreshService, IOptionsMonitor<SDSettings> intSettings)
: IRequestHandler<EPGSync, APIResponse>
{
    private readonly SDSettings settings = intSettings.CurrentValue;

    public async Task<APIResponse> Handle(EPGSync request, CancellationToken cancellationToken)
    {

        if (settings.SDEnabled)
        {
            APIResponse response = await schedulesDirect.SDSync(cancellationToken).ConfigureAwait(false);
            if (!response.IsError)
            {
                logger.LogInformation("Updated Schedules Direct");
                await dataRefreshService.RefreshSchedulesDirect();
            }
        }
        else
        {
            await dataRefreshService.RefreshStationPreviews();
        }
        //await HubContext.Clients.All.DataRefresh("GetEPGFiles");


        return APIResponse.Ok;
    }
}