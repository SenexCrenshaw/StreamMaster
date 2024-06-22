namespace StreamMaster.Application.SchedulesDirect.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record EPGSyncRequest() : IRequest<APIResponse>;

public class SDSyncHandler(ISchedulesDirect schedulesDirect, ILogger<EPGSyncRequest> logger, IHubContext<StreamMasterHub, IStreamMasterHub> HubContext, IOptionsMonitor<SDSettings> intsettings)
: IRequestHandler<EPGSyncRequest, APIResponse>
{
    private readonly SDSettings settings = intsettings.CurrentValue;

    public async Task<APIResponse> Handle(EPGSyncRequest request, CancellationToken cancellationToken)
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