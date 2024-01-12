using StreamMaster.Application.ChannelGroups.Events;

namespace StreamMaster.Application.ChannelGroups.EventHandlers;
public class CreateChannelGroupEventHandler(ILogger<CreateChannelGroupEvent> logger, IHubContext<StreamMasterHub, IStreamMasterHub> HubContext)
: INotificationHandler<CreateChannelGroupEvent>
{
    public async Task Handle(CreateChannelGroupEvent notification, CancellationToken cancellationToken)
    {
        await HubContext.Clients.All.ChannelGroupCreated(notification.ChannelGroup).ConfigureAwait(false);
    }
}