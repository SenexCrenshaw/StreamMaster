using StreamMaster.Application.Interfaces;

namespace StreamMaster.Application.Icons.EventHandlers;

public class IconFileAddedEventHandler : INotificationHandler<IconFileAddedEvent>
{
    private readonly IHubContext<StreamMasterHub, IStreamMasterHub> _hubContext;
    private readonly ILogger<IconFileAddedEventHandler> _logger;

    public IconFileAddedEventHandler(
          IHubContext<StreamMasterHub, IStreamMasterHub> hubContext,
        ILogger<IconFileAddedEventHandler> logger
        )
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task Handle(IconFileAddedEvent notification, CancellationToken cancellationToken)
    {
        //await _hubContext.Clients.All.IconsRefresh().ConfigureAwait(false);
    }
}
