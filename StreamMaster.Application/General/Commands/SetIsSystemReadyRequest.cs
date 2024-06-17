namespace StreamMaster.Application.General.Commands;

public record SetIsSystemReadyRequest(bool IsSystemReady) : IRequest;

public class SetIsSystemReadyRequestHandler(
    IDataRefreshService dataRefreshService) : IRequestHandler<SetIsSystemReadyRequest>
{
    public async Task Handle(SetIsSystemReadyRequest request, CancellationToken cancellationToken)
    {
        BuildInfo.SetIsSystemReady = request.IsSystemReady;
        await dataRefreshService.RefreshSettings(true).ConfigureAwait(false);
        //await hubContext.Clients.All.DataRefresh("Settings");
        //await hubContext.Clients.All.SystemStatusUpdate(new SDSystemStatus { IsSystemReady = request.IsSystemReady }).ConfigureAwait(false);
    }
}
