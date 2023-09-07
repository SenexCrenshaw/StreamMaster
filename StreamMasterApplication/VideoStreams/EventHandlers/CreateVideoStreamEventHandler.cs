using StreamMasterApplication.ChannelGroups.Events;
using StreamMasterApplication.ChannelGroups.Queries;
using StreamMasterApplication.VideoStreams.Events;

namespace StreamMasterApplication.VideoStreams.EventHandlers;

public class CreateVideoStreamEventHandler(ILogger<CreateVideoStreamEvent> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext) : BaseMediatorRequestHandler(logger, repository, mapper, publisher, sender, hubContext), INotificationHandler<CreateVideoStreamEvent>
{
    public async Task Handle(CreateVideoStreamEvent notification, CancellationToken cancellationToken = default)
    {
        ChannelGroupDto? channelGroup = await Sender.Send(new GetChannelGroupByName(notification.VideoStream.User_Tvg_group), cancellationToken).ConfigureAwait(false);
        if (channelGroup != null)
        {
            await Publisher.Publish(new UpdateChannelGroupEvent(channelGroup), cancellationToken).ConfigureAwait(false);
        }
        await HubContext.Clients.All.VideoStreamsRefresh().ConfigureAwait(false);
    }
}