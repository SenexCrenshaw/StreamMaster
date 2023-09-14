using StreamMasterApplication.ChannelGroups.Events;

namespace StreamMasterApplication.ChannelGroups.EventHandlers;



public class DeleteChannelGroupsEventHandler(ILogger<DeleteChannelGroupsEvent> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache)
: BaseMediatorRequestHandler(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache), INotificationHandler<DeleteChannelGroupsEvent>
{
    private readonly IHubContext<StreamMasterHub, IStreamMasterHub> _hubContext = hubContext;

    public async Task Handle(DeleteChannelGroupsEvent notification, CancellationToken cancellationToken)
    {
        if (notification.ChannelGroupIds.Any())
        {
            foreach (int id in notification.ChannelGroupIds)
            {
                MemoryCache.RemoveChannelGroupStreamCount(id);
            }
        }

        if (notification.VideoStreams.Any())
        {
            await HubContext.Clients.All.VideoStreamsRefresh(notification.VideoStreams.ToArray()).ConfigureAwait(false);
        }
    }
}