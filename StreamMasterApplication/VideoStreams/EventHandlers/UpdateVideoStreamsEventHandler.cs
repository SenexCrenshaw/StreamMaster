using StreamMasterApplication.VideoStreams.Events;

namespace StreamMasterApplication.VideoStreams.EventHandlers;

public class UpdateVideoStreamsEventHandler : BaseMediatorRequestHandler, INotificationHandler<UpdateVideoStreamsEvent>
{
    public UpdateVideoStreamsEventHandler(ILogger<UpdateVideoStreamsEvent> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache)
: base(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache) { }

    public async Task Handle(UpdateVideoStreamsEvent notification, CancellationToken cancellationToken = default)
    {
        //if (notification.UpdateChannelGroup)
        //{
        //    List<string> ids = notification.VideoStreams.Select(x => x.Id).ToList();
        //    List<ChannelGroupDto> channelGroups = await Sender.Send(new GetChannelGroupsFromVideoStreamIds(ids), cancellationToken).ConfigureAwait(false);
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