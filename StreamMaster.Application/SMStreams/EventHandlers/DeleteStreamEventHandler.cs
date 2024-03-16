using StreamMaster.Application.ChannelGroups.Events;
using StreamMaster.Application.SMStreams.Events;

namespace StreamMaster.Application.SMStreams.EventHandlers;

public class DeleteStreamEventHandler(ILogger<DeleteStreamEvent> logger, IMapper mapper, IPublisher Publisher, ISender Sender, IHubContext<StreamMasterHub, IStreamMasterHub> HubContext)
    : INotificationHandler<DeleteStreamEvent>
{
    public async Task Handle(DeleteStreamEvent notification, CancellationToken cancellationToken = default)
    {
        if (notification.ChannelGroup != null)
        {
            ChannelGroupDto channelGroupDTO = mapper.Map<ChannelGroupDto>(notification.ChannelGroup);
            await Publisher.Publish(new UpdateChannelGroupEvent(channelGroupDTO, false, false), cancellationToken).ConfigureAwait(false);
        }
        await HubContext.Clients.All.StreamsRefresh().ConfigureAwait(false);
    }
}