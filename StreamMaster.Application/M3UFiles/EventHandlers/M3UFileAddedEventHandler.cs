using StreamMaster.Application.Services;

namespace StreamMaster.Application.M3UFiles.EventHandlers;

public class M3UFileAddedEventHandler : INotificationHandler<M3UFileAddedEvent>
{

    private readonly IBackgroundTaskQueue _taskQueue;

    public M3UFileAddedEventHandler(

        IBackgroundTaskQueue taskQueue
        )
    {
        _taskQueue = taskQueue;

    }

    public async Task Handle(M3UFileAddedEvent notification, CancellationToken cancellationToken)
    {
        await _taskQueue.ProcessM3UFile(notification.M3UFileId, cancellationToken: cancellationToken).ConfigureAwait(false);

    }
}
