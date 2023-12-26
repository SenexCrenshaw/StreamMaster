using StreamMaster.Domain.Repository;
using StreamMaster.Domain.Services;

using StreamMaster.Application.ChannelGroups.Events;

namespace StreamMaster.Application.ChannelGroups.EventHandlers;



public class DeleteChannelGroupsEventHandler(ILogger<DeleteChannelGroupsEvent> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache)
: BaseMediatorRequestHandler(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache), INotificationHandler<DeleteChannelGroupsEvent>
{
    public async Task Handle(DeleteChannelGroupsEvent notification, CancellationToken cancellationToken)
    {
        await HubContext.Clients.All.ChannelGroupsDelete(notification.ChannelGroupIds).ConfigureAwait(false);

        if (notification.VideoStreams.Any())
        {
            await HubContext.Clients.All.VideoStreamsRefresh(notification.VideoStreams.ToArray()).ConfigureAwait(false);
        }
    }
}