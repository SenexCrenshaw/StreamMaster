using StreamMaster.Domain.Repository;
using StreamMaster.Domain.Services;

using StreamMaster.Application.ChannelGroups.Events;

namespace StreamMaster.Application.ChannelGroups.EventHandlers;
public class CreateChannelGroupEventHandler(ILogger<CreateChannelGroupEvent> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache)
: BaseMediatorRequestHandler(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache), INotificationHandler<CreateChannelGroupEvent>
{
    public async Task Handle(CreateChannelGroupEvent notification, CancellationToken cancellationToken)
    {
        await HubContext.Clients.All.ChannelGroupCreated(notification.ChannelGroup).ConfigureAwait(false);
    }
}