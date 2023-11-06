using StreamMasterApplication.Services;

namespace StreamMasterApplication.M3UFiles.EventHandlers;

public class M3UFileAddedEventHandler : INotificationHandler<M3UFileAddedEvent>
{
    private readonly IHubContext<StreamMasterHub, IStreamMasterHub> _hubContext;
    private readonly IBackgroundTaskQueue _taskQueue;

    public M3UFileAddedEventHandler(
        IHubContext<StreamMasterHub, IStreamMasterHub> hubContext,
        IBackgroundTaskQueue taskQueue
        )
    {
        _taskQueue = taskQueue;
        _hubContext = hubContext;
    }

    public async Task Handle(M3UFileAddedEvent notification, CancellationToken cancellationToken)
    {
        await _taskQueue.ProcessM3UFile(notification.Item.Id, cancellationToken: cancellationToken).ConfigureAwait(false);
        await _hubContext.Clients.All.M3UFilesRefresh().ConfigureAwait(false);
        await _hubContext.Clients.All.ChannelGroupsRefresh().ConfigureAwait(false);
        await _hubContext.Clients.All.VideoStreamsRefresh().ConfigureAwait(false);
    }
}
