using MediatR;

using Microsoft.AspNetCore.SignalR;

using StreamMasterApplication.Hubs;

namespace StreamMasterApplication.StreamGroups.EventHandlers;

public class StreamGroupUpdateEventHandler : INotificationHandler<StreamGroupUpdateEvent>
{
    private readonly IHubContext<StreamMasterHub, IStreamMasterHub> _hubContext;

    public StreamGroupUpdateEventHandler(
         IHubContext<StreamMasterHub, IStreamMasterHub> hubContext
        )
    {
        _hubContext = hubContext;
    }

    public async Task Handle(StreamGroupUpdateEvent notification, CancellationToken cancellationToken)
    {
        await _hubContext.Clients.All.StreamGroupDtoUpdate(notification.StreamGroup).ConfigureAwait(false);
    }
}