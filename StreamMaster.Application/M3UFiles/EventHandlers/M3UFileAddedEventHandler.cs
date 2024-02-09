using StreamMaster.Application.M3UFiles.Commands;
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
        ProcessM3UFileRequest processM3UFileRequest = new(notification.M3UFileId, forceRun: notification.ForecRun);
        await _taskQueue.ProcessM3UFile(processM3UFileRequest, cancellationToken: cancellationToken).ConfigureAwait(false);

    }
}
