using MediatR;

using Microsoft.AspNetCore.SignalR;

using StreamMasterApplication.Hubs;

namespace StreamMasterApplication.M3UFiles.EventHandlers;

public class M3UFileChangedEventHandler : INotificationHandler<M3UFileChangedEvent>
{
    private readonly IHubContext<StreamMasterHub, IStreamMasterHub> _hubContext;

    public M3UFileChangedEventHandler(
        IHubContext<StreamMasterHub, IStreamMasterHub> hubContext
        )
    {
        _hubContext = hubContext;
    }

    public async Task Handle(M3UFileChangedEvent notification, CancellationToken cancellationToken)
    {
        await _hubContext.Clients.All.M3UFilesRefresh().ConfigureAwait(false);
    }
}
