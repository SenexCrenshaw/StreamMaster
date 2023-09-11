using StreamMasterApplication.ChannelGroups.Events;
using StreamMasterApplication.ChannelGroups.Queries;
using StreamMasterApplication.VideoStreams.Events;

namespace StreamMasterApplication.VideoStreams.EventHandlers;

public class DeleteVideoStreamsEventHandler : BaseMediatorRequestHandler, INotificationHandler<DeleteVideoStreamsEvent>
{
    public DeleteVideoStreamsEventHandler(ILogger<DeleteVideoStreamsEvent> logger, IRepositoryWrapper repository, IMapper mapper,ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext)
: base(logger, repository, mapper,settingsService, publisher, sender, hubContext) { }

    public async Task Handle(DeleteVideoStreamsEvent notification, CancellationToken cancellationToken)
    {
        List<ChannelGroupDto> channelGroups = await Sender.Send(new GetChannelGroupsFromVideoStreamIds(notification.VideoStreamIds), cancellationToken).ConfigureAwait(false);
        if (channelGroups.Any())
        {
            await Publisher.Publish(new UpdateChannelGroupsEvent(channelGroups), cancellationToken).ConfigureAwait(false);
        }

        await HubContext.Clients.All.VideoStreamsRefresh().ConfigureAwait(false);
    }
}