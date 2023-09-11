using StreamMasterApplication.ChannelGroups.Events;

namespace StreamMasterApplication.ChannelGroups.EventHandlers;
public class CreateChannelGroupEventHandler(ILogger<CreateChannelGroupEvent> logger, IRepositoryWrapper repository, IMapper mapper,ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext)
: BaseMediatorRequestHandler(logger, repository, mapper,settingsService, publisher, sender, hubContext), INotificationHandler<CreateChannelGroupEvent>
{
    public async Task Handle(CreateChannelGroupEvent notification, CancellationToken cancellationToken)
    {
        await HubContext.Clients.All.ChannelGroupsRefresh().ConfigureAwait(false);
    }
}