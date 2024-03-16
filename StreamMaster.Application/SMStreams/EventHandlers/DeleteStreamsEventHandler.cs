using StreamMaster.Application.ChannelGroups.Events;
using StreamMaster.Application.ChannelGroups.Queries;
using StreamMaster.Application.SMStreams.Events;

namespace StreamMaster.Application.SMStreams.EventHandlers;

public class DeleteStreamsEventHandler(ILogger<DeleteStreamsEvent> logger, IPublisher Publisher, ISender Sender, IHubContext<StreamMasterHub, IStreamMasterHub> HubContext)
    : INotificationHandler<DeleteStreamsEvent>
{
    public async Task Handle(DeleteStreamsEvent notification, CancellationToken cancellationToken)
    {
        List<ChannelGroupDto> channelGroups = await Sender.Send(new GetChannelGroupsFromVideoStreamIds(notification.VideoStreamIds), cancellationToken).ConfigureAwait(false);
        if (channelGroups.Count != 0)
        {
            await Publisher.Publish(new UpdateChannelGroupsEvent(channelGroups), cancellationToken).ConfigureAwait(false);
        }

        await HubContext.Clients.All.StreamsRefresh().ConfigureAwait(false);
    }
}