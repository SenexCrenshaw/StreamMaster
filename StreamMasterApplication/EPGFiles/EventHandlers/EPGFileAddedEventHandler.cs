using MediatR;

using Microsoft.AspNetCore.SignalR;

using StreamMasterApplication.Hubs;
using StreamMasterApplication.Services;

namespace StreamMasterApplication.EPGFiles.EventHandlers;

public class EPGFileAddedEventHandler : INotificationHandler<EPGFileAddedEvent>
{
    private readonly IHubContext<StreamMasterHub, IStreamMasterHub> _hubContext;
    private readonly IBackgroundTaskQueue _taskQueue;

    public EPGFileAddedEventHandler(
        IHubContext<StreamMasterHub, IStreamMasterHub> hubContext,
        IBackgroundTaskQueue taskQueue
        )
    {
        _taskQueue = taskQueue;
        _hubContext = hubContext;
    }

    public async Task Handle(EPGFileAddedEvent notification, CancellationToken cancellationToken)
    {
        await _taskQueue.ProcessEPGFile(notification.Item.Id, cancellationToken).ConfigureAwait(false);
        await _hubContext.Clients.All.EPGFilesDtoUpdate(notification.Item).ConfigureAwait(false);
    }
}
