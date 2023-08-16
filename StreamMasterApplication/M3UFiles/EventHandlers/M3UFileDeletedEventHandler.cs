using MediatR;

using Microsoft.AspNetCore.SignalR;

using StreamMasterApplication.Hubs;

namespace StreamMasterApplication.M3UFiles.EventHandlers;

public class M3UFileDeletedEventHandler : INotificationHandler<M3UFileDeletedEvent>
{
    private readonly IHubContext<StreamMasterHub, IStreamMasterHub> _hubContext;

    public M3UFileDeletedEventHandler(
         IHubContext<StreamMasterHub, IStreamMasterHub> hubContext
        )
    {
        _hubContext = hubContext;
    }

    public async Task Handle(M3UFileDeletedEvent notification, CancellationToken cancellationToken)
    {
        await _hubContext.Clients.All.M3UFilesRefresh().ConfigureAwait(false);
    }
}
