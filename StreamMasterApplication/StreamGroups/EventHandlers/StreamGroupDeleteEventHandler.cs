﻿using MediatR;

using Microsoft.AspNetCore.SignalR;

using StreamMasterApplication.Hubs;
using StreamMasterApplication.StreamGroups.Events;

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
        await _hubContext.Clients.All.StreamGroupsRefresh().ConfigureAwait(false);
    }
}