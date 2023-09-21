using StreamMasterApplication.StreamGroups.Events;

namespace StreamMasterApplication.StreamGroups.EventHandlers;

public class StreamGroupUpdateEventHandler : INotificationHandler<StreamGroupUpdateEvent>
{
    private readonly IHubContext<StreamMasterHub, IStreamMasterHub> _hubContext;

    public StreamGroupUpdateEventHandler(
         IHubContext<StreamMasterHub, IStreamMasterHub> hubContext
        )
    {
        _hubContext = hubContext;
    }

    public async Task Handle(StreamGroupUpdateEvent notification, CancellationToken cancellationToken)
    {
        await _hubContext.Clients.All.StreamGroupsRefresh([notification.StreamGroup]).ConfigureAwait(false);
    }
}