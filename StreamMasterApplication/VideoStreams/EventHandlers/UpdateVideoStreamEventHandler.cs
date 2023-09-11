using StreamMasterApplication.ChannelGroups.Events;
using StreamMasterApplication.ChannelGroups.Queries;
using StreamMasterApplication.VideoStreams.Events;

namespace StreamMasterApplication.VideoStreams.EventHandlers;

public class UpdateVideoStreamEventHandler : BaseMediatorRequestHandler, INotificationHandler<UpdateVideoStreamEvent>
{
    public UpdateVideoStreamEventHandler(ILogger<UpdateVideoStreamEvent> logger, IRepositoryWrapper repository, IMapper mapper,ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext)
: base(logger, repository, mapper,settingsService, publisher, sender, hubContext) { }


    public async Task Handle(UpdateVideoStreamEvent notification, CancellationToken cancellationToken = default)
    {
        if (notification.UpdateChannelGroup)
        {
            ChannelGroupDto? channelGroup = await Sender.Send(new GetChannelGroupByName(notification.VideoStream.User_Tvg_group), cancellationToken).ConfigureAwait(false);
            if (channelGroup != null)
            {
                await Publisher.Publish(new UpdateChannelGroupEvent(channelGroup), cancellationToken).ConfigureAwait(false);
            }
        }

        await HubContext.Clients.All.VideoStreamsRefresh([notification.VideoStream]).ConfigureAwait(false);
        await HubContext.Clients.All.StreamGroupVideoStreamsRefresh().ConfigureAwait(false);
    }
}