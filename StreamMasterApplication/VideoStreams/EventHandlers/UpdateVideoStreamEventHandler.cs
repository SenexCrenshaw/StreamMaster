using StreamMasterApplication.VideoStreams.Events;

namespace StreamMasterApplication.VideoStreams.EventHandlers;

public class UpdateVideoStreamEventHandler : BaseMediatorRequestHandler, INotificationHandler<UpdateVideoStreamEvent>
{
    public UpdateVideoStreamEventHandler(ILogger<UpdateVideoStreamEvent> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache)
: base(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache) { }


    public async Task Handle(UpdateVideoStreamEvent notification, CancellationToken cancellationToken = default)
    {
        //if (notification.UpdateChannelGroup)
        //{
        //    ChannelGroupDto? channelGroup = await Sender.Send(new GetChannelGroupByName(notification.VideoStream.User_Tvg_group), cancellationToken).ConfigureAwait(false);
        //    if (channelGroup != null)
        //    {
        //        await Publisher.Publish(new UpdateChannelGroupEvent(channelGroup, false, false), cancellationToken).ConfigureAwait(false);
        //    }
        //}

        await HubContext.Clients.All.VideoStreamsRefresh([notification.VideoStream]).ConfigureAwait(false);
        await HubContext.Clients.All.StreamGroupVideoStreamsRefresh().ConfigureAwait(false);
    }
}