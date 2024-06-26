using StreamMaster.Application.M3UFiles.Commands;
using StreamMaster.Application.Services;

namespace StreamMaster.Application.M3UFiles.EventHandlers;

public class M3UFileProcessEventHandler(IBackgroundTaskQueue taskQueue) : INotificationHandler<M3UFileProcessEvent>
{
    public async Task Handle(M3UFileProcessEvent notification, CancellationToken cancellationToken)
    {
        ProcessM3UFileRequest processM3UFileRequest = new(notification.M3UFileId, ForceRun: notification.ForecRun);
        await taskQueue.ProcessM3UFile(processM3UFileRequest, cancellationToken: cancellationToken).ConfigureAwait(false);
    }
}
