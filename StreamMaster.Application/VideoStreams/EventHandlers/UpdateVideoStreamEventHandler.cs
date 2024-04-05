using StreamMaster.Application.ChannelGroups.Commands;
using StreamMaster.Application.ChannelGroups.Events;
using StreamMaster.Application.ChannelGroups.Queries;
using StreamMaster.Application.VideoStreams.Events;

namespace StreamMaster.Application.VideoStreams.EventHandlers;

public class UpdateVideoStreamEventHandler(ILogger<UpdateVideoStreamEvent> logger, IPublisher Publisher, ISender Sender, IHubContext<StreamMasterHub, IStreamMasterHub> HubContext)
    : INotificationHandler<UpdateVideoStreamEvent>
{
    public async Task Handle(UpdateVideoStreamEvent notification, CancellationToken cancellationToken = default)
    {
        //if (notification.ToggelVisibility)
        //{
        //    ChannelGroupDto? channelGroup = await Sender.Send(new GetChannelGroupByNameRequest(notification.VideoStream.User_Tvg_group), cancellationToken).ConfigureAwait(false);
        //    if (channelGroup != null)
        //    {
        //        await Publisher.Publish(new UpdateChannelGroupEvent(channelGroup, false, false), cancellationToken).ConfigureAwait(false);
        //    }
        //}

        APIResponse<ChannelGroupDto?> channelGroup = await Sender.Send(new GetChannelGroupByNameRequest(notification.VideoStream.User_Tvg_group), cancellationToken).ConfigureAwait(false);
        if (channelGroup?.Data != null)
        {
            await Sender.Send(new UpdateChannelGroupCountRequest(channelGroup.Data, false), cancellationToken).ConfigureAwait(false);
            await Publisher.Publish(new UpdateChannelGroupEvent(channelGroup.Data, false, false), cancellationToken).ConfigureAwait(false);
        }

        await HubContext.Clients.All.VideoStreamLinksRefresh([notification.VideoStream.Id]);

        await HubContext.Clients.All.VideoStreamsRefresh([notification.VideoStream]).ConfigureAwait(false);
        await HubContext.Clients.All.StreamGroupVideoStreamsRefresh().ConfigureAwait(false);
    }
}