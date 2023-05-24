using MediatR;

using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;

using StreamMasterApplication.Hubs;

namespace StreamMasterApplication.General.Commands;

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
        await _hubContext.Clients.All.SystemStatusUpdate(new SystemStatus { IsSystemReady = request.IsSystemReady }).ConfigureAwait(false);
    }
}
