using StreamMaster.Domain.Cache;

namespace StreamMaster.Application.General.Commands;

public record SetIsSystemReadyRequest(bool IsSystemReady) : IRequest;

public class SetIsSystemReadyRequestHandler : IRequestHandler<SetIsSystemReadyRequest>
{
    private readonly IHubContext<StreamMasterHub, IStreamMasterHub> _hubContext;
    private readonly IMemoryCache _memoryCache;

    public SetIsSystemReadyRequestHandler(
        IHubContext<StreamMasterHub, IStreamMasterHub> hubContext,
        IMemoryCache memoryCache)
    {
        _hubContext = hubContext;
        _memoryCache = memoryCache;
    }

    public async Task Handle(SetIsSystemReadyRequest request, CancellationToken cancellationToken)
    {
        _memoryCache.SetIsSystemReady(request.IsSystemReady);
        await _hubContext.Clients.All.SystemStatusUpdate(new SDSystemStatus { IsSystemReady = request.IsSystemReady }).ConfigureAwait(false);
    }
}
