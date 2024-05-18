using StreamMaster.Application.VideoStreams.Events;

namespace StreamMaster.Application.VideoStreams.EventHandlers;

public class DeleteVideoStreamsEventHandler(ILogger<DeleteVideoStreamsEvent> logger, IPublisher Publisher, ISender Sender, IHubContext<StreamMasterHub, IStreamMasterHub> HubContext)
    : INotificationHandler<DeleteVideoStreamsEvent>
{
    public async Task Handle(DeleteVideoStreamsEvent notification, CancellationToken cancellationToken)
    {
        //List<ChannelGroup> channelGroups = await Sender.Send(new GetChannelGroupsFromVideoStreamIds(notification.VideoStreamIds), cancellationToken).ConfigureAwait(false);
        //if (channelGroups.Count != 0)
        //{
        //    await Publisher.Publish(new UpdateChannelGroupsEvent(channelGroups), cancellationToken).ConfigureAwait(false);
        //}

        await HubContext.Clients.All.VideoStreamsRefresh().ConfigureAwait(false);
    }
}