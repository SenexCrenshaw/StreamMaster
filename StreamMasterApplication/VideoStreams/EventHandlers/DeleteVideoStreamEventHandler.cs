using StreamMasterApplication.ChannelGroups.Events;
using StreamMasterApplication.ChannelGroups.Queries;
using StreamMasterApplication.VideoStreams.Events;

namespace StreamMasterApplication.VideoStreams.EventHandlers;

public class DeleteVideoStreamEventHandler : BaseMediatorRequestHandler, INotificationHandler<DeleteVideoStreamEvent>
{
    public DeleteVideoStreamEventHandler(ILogger<DeleteVideoStreamEvent> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext)
: base(logger, repository, mapper, settingsService, publisher, sender, hubContext) { }

    public async Task Handle(DeleteVideoStreamEvent notification, CancellationToken cancellationToken = default)
    {
        ChannelGroupDto? channelGroup = await Sender.Send(new GetChannelGroupFromVideoStreamId(notification.VideoStreamId), cancellationToken).ConfigureAwait(false);
        if (channelGroup != null)
        {
            await Publisher.Publish(new UpdateChannelGroupEvent(channelGroup, false), cancellationToken).ConfigureAwait(false);
        }
        await HubContext.Clients.All.VideoStreamsRefresh().ConfigureAwait(false);
    }
}