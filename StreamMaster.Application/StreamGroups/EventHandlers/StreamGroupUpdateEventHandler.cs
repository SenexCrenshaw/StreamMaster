﻿using StreamMaster.Application.Interfaces;
using StreamMaster.Application.StreamGroups.Events;

namespace StreamMaster.Application.StreamGroups.EventHandlers;

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
        //await _hubContext.Clients.All.StreamGroupsRefresh([notification.StreamGroup]).ConfigureAwait(false);
        //await _hubContext.Clients.All.StreamGroupsRefresh().ConfigureAwait(false);
    }
}