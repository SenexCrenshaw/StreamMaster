using StreamMaster.Application.Services;

namespace StreamMaster.Application.EPGFiles.EventHandlers;

public class EPGFileAddedEventHandler(IBackgroundTaskQueue taskQueue)
    : INotificationHandler<EPGFileAddedEvent>
{
    public async Task Handle(EPGFileAddedEvent notification, CancellationToken cancellationToken)
    {
        await taskQueue.ProcessEPGFile(notification.Item.Id, cancellationToken).ConfigureAwait(false);
    }
}
