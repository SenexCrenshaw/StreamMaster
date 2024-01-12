using StreamMaster.Application.ChannelGroups.Commands;
using StreamMaster.Application.ChannelGroups.Events;
using StreamMaster.Application.ChannelGroups.Queries;
using StreamMaster.Application.VideoStreams.Events;

namespace StreamMaster.Application.VideoStreams.EventHandlers;

public class CreateVideoStreamEventHandler(ILogger<CreateVideoStreamEvent> logger, IPublisher Publisher, ISender Sender)
    : INotificationHandler<CreateVideoStreamEvent>
{
    public async Task Handle(CreateVideoStreamEvent notification, CancellationToken cancellationToken = default)
    {
        ChannelGroupDto? channelGroup = await Sender.Send(new GetChannelGroupByName(notification.VideoStream.User_Tvg_group), cancellationToken).ConfigureAwait(false);
        if (channelGroup != null)
        {
            await Sender.Send(new UpdateChannelGroupCountRequest(channelGroup, true), cancellationToken).ConfigureAwait(false);
            await Publisher.Publish(new UpdateChannelGroupEvent(channelGroup, false, false), cancellationToken).ConfigureAwait(false);
        }
        //await HubContext.Clients.All.VideoStreamsRefresh().ConfigureAwait(false);
    }
}