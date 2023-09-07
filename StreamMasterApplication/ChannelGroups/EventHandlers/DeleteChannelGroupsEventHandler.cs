using StreamMasterApplication.ChannelGroups.Events;

namespace StreamMasterApplication.ChannelGroups.EventHandlers;



public class DeleteChannelGroupsEventHandler(ILogger<DeleteChannelGroupsEvent> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext)
: BaseMediatorRequestHandler(logger, repository, mapper, publisher, sender, hubContext), INotificationHandler<DeleteChannelGroupsEvent>
{
    private readonly IHubContext<StreamMasterHub, IStreamMasterHub> _hubContext = hubContext;

    public async Task Handle(DeleteChannelGroupsEvent notification, CancellationToken cancellationToken)
    {
        await HubContext.Clients.All.ChannelGroupsRefresh().ConfigureAwait(false);
        if (notification.VideoStreams.Any())
        {
            await HubContext.Clients.All.VideoStreamsRefresh(notification.VideoStreams.ToArray()).ConfigureAwait(false);
        }
    }
}