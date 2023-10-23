﻿namespace StreamMasterApplication.EPGFiles.EventHandlers;

public class EPGFileDeletedEventHandler : INotificationHandler<EPGFileDeletedEvent>
{
    private readonly IHubContext<StreamMasterHub, IStreamMasterHub> _hubContext;

    public EPGFileDeletedEventHandler(
  IHubContext<StreamMasterHub, IStreamMasterHub> hubContext
        )
    {
        _hubContext = hubContext;
    }

    public async Task Handle(EPGFileDeletedEvent notification, CancellationToken cancellationToken)
    {
        await _hubContext.Clients.All.EPGFilesRefresh().ConfigureAwait(false);
        await _hubContext.Clients.All.ChannelGroupsRefresh().ConfigureAwait(false);
    }
}
