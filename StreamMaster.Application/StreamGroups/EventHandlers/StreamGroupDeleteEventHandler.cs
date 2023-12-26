using MediatR;

using Microsoft.AspNetCore.SignalR;

using StreamMaster.Application.Hubs;
using StreamMaster.Application.StreamGroups.Events;

namespace StreamMaster.Application.StreamGroups.EventHandlers;

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
        await _hubContext.Clients.All.StreamGroupsRefresh().ConfigureAwait(false);
    }
}