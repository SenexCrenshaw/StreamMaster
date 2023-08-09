using MediatR;

using Microsoft.AspNetCore.SignalR;

using StreamMasterApplication.Hubs;
using StreamMasterApplication.VideoStreams.Events;

namespace StreamMasterApplication.VideoStreams.EventHandlers;

public class DeleteVideoStreamsEventHandler : INotificationHandler<DeleteVideoStreamsEvent>
{
    private readonly IHubContext<StreamMasterHub, IStreamMasterHub> _hubContext;

    public DeleteVideoStreamsEventHandler(
         IHubContext<StreamMasterHub, IStreamMasterHub> hubContext
        )
    {
        _hubContext = hubContext;
    }

    public async Task Handle(DeleteVideoStreamsEvent notification, CancellationToken cancellationToken)
    {
        await _hubContext.Clients.All.VideoStreamsRefresh().ConfigureAwait(false);
    }
}