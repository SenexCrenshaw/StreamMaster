using StreamMaster.Application.Services;

namespace StreamMaster.Application.EPGFiles.EventHandlers;

public class EPGFileAddedEventHandler(
    IHubContext<StreamMasterHub, IStreamMasterHub> hubContext,
    IBackgroundTaskQueue taskQueue
        ) : INotificationHandler<EPGFileAddedEvent>
{
    public async Task Handle(EPGFileAddedEvent notification, CancellationToken cancellationToken)
    {
        await taskQueue.ProcessEPGFile(notification.Item.Id, cancellationToken).ConfigureAwait(false);
    }
}
