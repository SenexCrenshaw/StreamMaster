using StreamMaster.Application.VideoStreams.Events;

namespace StreamMaster.Application.VideoStreams.EventHandlers;

public class UpdateVideoStreamsEventHandler(ILogger<UpdateVideoStreamsEvent> logger, IHubContext<StreamMasterHub, IStreamMasterHub> HubContext)
    : INotificationHandler<UpdateVideoStreamsEvent>
{
    public async Task Handle(UpdateVideoStreamsEvent notification, CancellationToken cancellationToken = default)
    {
        //if (notification.UpdateChannelGroup)
        //{
        //    List<string> ids = notification.VideoStreams.Select(x => x.Id).ToList();
        //    List<ChannelGroup> channelGroups = await Sender.Send(new GetChannelGroupsFromVideoStreamIds(ids), cancellationToken).ConfigureAwait(false);
        //    if (channelGroups.Any())
        //    {
        //        await Publisher.Publish(new UpdateChannelGroupsEvent(channelGroups), cancellationToken).ConfigureAwait(false);
        //    }
        //}

        if (notification.VideoStreams.Any())
        {
            await HubContext.Clients.All.VideoStreamsRefresh(notification.VideoStreams.ToArray()).ConfigureAwait(false);

        }
        else
        {
            await HubContext.Clients.All.VideoStreamsRefresh().ConfigureAwait(false);
        }

        await HubContext.Clients.All.StreamGroupVideoStreamsRefresh().ConfigureAwait(false);
    }
}