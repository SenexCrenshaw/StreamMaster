namespace StreamMaster.Application.M3UFiles.EventHandlers;

public class M3UFileDeletedEventHandler(
     IHubContext<StreamMasterHub, IStreamMasterHub> hubContext
        ) : INotificationHandler<M3UFileDeletedEvent>
{
    public async Task Handle(M3UFileDeletedEvent notification, CancellationToken cancellationToken)
    {
        await hubContext.Clients.All.M3UFilesRefresh().ConfigureAwait(false);
    }
}
