using StreamMaster.Application.ChannelGroups.Events;

namespace StreamMaster.Application.ChannelGroups.EventHandlers;

public class DeleteChannelGroupEventHandler(ILogger<DeleteChannelGroupEvent> logger, IHubContext<StreamMasterHub, IStreamMasterHub> HubContext)
: INotificationHandler<DeleteChannelGroupEvent>
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