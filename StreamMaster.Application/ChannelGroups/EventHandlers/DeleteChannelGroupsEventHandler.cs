using StreamMaster.Application.ChannelGroups.Events;

namespace StreamMaster.Application.ChannelGroups.EventHandlers;



public class DeleteChannelGroupsEventHandler(ILogger<DeleteChannelGroupsEvent> logger, IHubContext<StreamMasterHub, IStreamMasterHub> HubContext)
: INotificationHandler<DeleteChannelGroupsEvent>
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