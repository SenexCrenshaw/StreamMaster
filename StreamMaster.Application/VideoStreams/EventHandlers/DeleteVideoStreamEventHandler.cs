using StreamMaster.Application.ChannelGroups.Events;
using StreamMaster.Application.VideoStreams.Events;

namespace StreamMaster.Application.VideoStreams.EventHandlers;

public class DeleteVideoStreamEventHandler(ILogger<DeleteVideoStreamEvent> logger, IMapper mapper, IPublisher Publisher, ISender Sender, IHubContext<StreamMasterHub, IStreamMasterHub> HubContext)
    : INotificationHandler<DeleteVideoStreamEvent>
{
    public async Task Handle(DeleteVideoStreamEvent notification, CancellationToken cancellationToken = default)
    {
        if (notification.ChannelGroup != null)
        {
            ChannelGroupDto channelGroupDTO = mapper.Map<ChannelGroupDto>(notification.ChannelGroup);
            await Publisher.Publish(new UpdateChannelGroupEvent(channelGroupDTO, false, false), cancellationToken).ConfigureAwait(false);
        }
        await HubContext.Clients.All.VideoStreamsRefresh().ConfigureAwait(false);
    }
}