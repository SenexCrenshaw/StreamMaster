using MediatR;

using Microsoft.AspNetCore.SignalR;

using StreamMasterApplication.Hubs;

namespace StreamMasterApplication.StreamGroups.EventHandlers;

public class StreamGroupDeleteEventHandler : INotificationHandler<StreamGroupDeleteEvent>
{
    private readonly IHubContext<StreamMasterHub, IStreamMasterHub> _hubContext;

    public StreamGroupDeleteEventHandler(
         IHubContext<StreamMasterHub, IStreamMasterHub> hubContext
        )
    {
        _hubContext = hubContext;
    }

    public async Task Handle(StreamGroupDeleteEvent notification, CancellationToken cancellationToken)
    {
        await _hubContext.Clients.All.StreamGroupDtoDelete(notification.StreamGroupId).ConfigureAwait(false);
    }
}