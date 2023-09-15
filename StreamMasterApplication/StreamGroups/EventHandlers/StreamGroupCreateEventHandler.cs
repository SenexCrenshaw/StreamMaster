using StreamMasterApplication.StreamGroups.Events;

namespace StreamMasterApplication.StreamGroups.EventHandlers;

public class StreamGroupCreateEventHandler : INotificationHandler<StreamGroupCreateEvent>
{
    private readonly IHubContext<StreamMasterHub, IStreamMasterHub> _hubContext;

    public StreamGroupCreateEventHandler(IHubContext<StreamMasterHub, IStreamMasterHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task Handle(StreamGroupCreateEvent notification, CancellationToken cancellationToken)
    {
        await _hubContext.Clients.All.StreamGroupsRefresh().ConfigureAwait(false);
    }
}