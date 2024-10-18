namespace StreamMaster.Application.General.Commands;

public record SetIsSystemReadyRequest(bool IsSystemReady) : IRequest;

public class SetIsSystemReadyRequestHandler(ILogger<SetIsSystemReadyRequest> logger,
    IDataRefreshService dataRefreshService) : IRequestHandler<SetIsSystemReadyRequest>
{
    public async Task Handle(SetIsSystemReadyRequest request, CancellationToken cancellationToken)
    {
        BuildInfo.IsSystemReady = request.IsSystemReady;
        await dataRefreshService.RefreshSettings(true).ConfigureAwait(false);
        logger.LogInformation("System build {build}", BuildInfo.Release);
    }
}
