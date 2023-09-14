using StreamMasterApplication.ChannelGroups.Events;

namespace StreamMasterApplication.ChannelGroups.EventHandlers;

public class DeleteChannelGroupEventHandler(ILogger<DeleteChannelGroupEvent> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache)
: BaseMediatorRequestHandler(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache), INotificationHandler<DeleteChannelGroupEvent>
{
    public async Task Handle(DeleteChannelGroupEvent notification, CancellationToken cancellationToken)
    {
        await HubContext.Clients.All.ChannelGroupDelete(notification.ChannelGroupId).ConfigureAwait(false);
        if (notification.VideoStreams.Any())
        {
            await HubContext.Clients.All.VideoStreamsRefresh(notification.VideoStreams.ToArray()).ConfigureAwait(false);
        }

    }
}