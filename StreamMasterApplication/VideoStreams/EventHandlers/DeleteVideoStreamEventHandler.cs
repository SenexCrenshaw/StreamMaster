using MediatR;

using Microsoft.AspNetCore.SignalR;

using StreamMasterApplication.Hubs;
using StreamMasterApplication.VideoStreams.Events;

namespace StreamMasterApplication.VideoStreams.EventHandlers;

public class DeleteVideoStreamEventHandler : INotificationHandler<DeleteVideoStreamEvent>
{
    private readonly IHubContext<StreamMasterHub, IStreamMasterHub> _hubContext;

    public DeleteVideoStreamEventHandler(
         IHubContext<StreamMasterHub, IStreamMasterHub> hubContext
        )
    {
        _hubContext = hubContext;
    }

    public async Task Handle(DeleteVideoStreamEvent notification, CancellationToken cancellationToken)
    {
        await _hubContext.Clients.All.VideoStreamDtoDelete(notification.VideoFileId).ConfigureAwait(false);
    }
}
