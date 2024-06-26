namespace StreamMaster.Application.M3UFiles.EventHandlers;

public class M3UFileChangedEventHandler(IDataRefreshService dataRefreshService) : INotificationHandler<M3UFileChangedEvent>
{


    public async Task Handle(M3UFileChangedEvent notification, CancellationToken cancellationToken)
    {
        await dataRefreshService.RefreshAllM3U();
        //await _hubContext.Clients.All.M3UFilesRefresh().ConfigureAwait(false);
    }
}
