using StreamMaster.Domain.Configuration;

namespace StreamMaster.Application.General.Commands;

public record SetIsSystemReadyRequest(bool IsSystemReady) : IRequest;

public class SetIsSystemReadyRequestHandler(
    IHubContext<StreamMasterHub, IStreamMasterHub> hubContext) : IRequestHandler<SetIsSystemReadyRequest>
{
    public async Task Handle(SetIsSystemReadyRequest request, CancellationToken cancellationToken)
    {
        BuildInfo.SetIsSystemReady = request.IsSystemReady;
        await hubContext.Clients.All.SystemStatusUpdate(new SDSystemStatus { IsSystemReady = request.IsSystemReady }).ConfigureAwait(false);
    }
}
