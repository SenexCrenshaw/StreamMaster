namespace StreamMaster.Application.EPGFiles.EventHandlers;

public class EPGFileChangedEventHandler : INotificationHandler<EPGFileChangedEvent>
{
    private readonly IHubContext<StreamMasterHub, IStreamMasterHub> _hubContext;

    public EPGFileChangedEventHandler(
        IHubContext<StreamMasterHub, IStreamMasterHub> hubContext
        )
    {
        _hubContext = hubContext;
    }

    public async Task Handle(EPGFileChangedEvent notification, CancellationToken cancellationToken)
    {
        await _hubContext.Clients.All.EPGFilesRefresh().ConfigureAwait(false);
        await _hubContext.Clients.All.CacheHandler("epgSelector").ConfigureAwait(false);
    }
}
