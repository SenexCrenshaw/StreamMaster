using StreamMaster.Application.ChannelGroups.Events;
using StreamMaster.Application.VideoStreams.Events;

namespace StreamMaster.Application.VideoStreams.EventHandlers;

public class DeleteVideoStreamEventHandler(ILogger<DeleteVideoStreamEvent> logger, IMapper mapper, IPublisher Publisher, ISender Sender, IHubContext<StreamMasterHub, IStreamMasterHub> HubContext)
    : INotificationHandler<DeleteVideoStreamEvent>
{
    public async Task Handle(DeleteVideoStreamEvent notification, CancellationToken cancellationToken = default)
    {
        //ChannelGroupDto? channelGroup = await Sender.Send(new GetChannelGroupFromVideoStreamId(notification.VideoStreamId), cancellationToken).ConfigureAwait(false);
        if (notification.ChannelGroup != null)
        {
            ChannelGroupDto channelGroupDTO = mapper.Map<ChannelGroupDto>(notification.ChannelGroup);
            //await Sender.Send(new UpdateChannelGroupEvent(channelGroupDTO, true, false), cancellationToken).ConfigureAwait(false);
            await Publisher.Publish(new UpdateChannelGroupEvent(channelGroupDTO, false, false), cancellationToken).ConfigureAwait(false);
        }
        await HubContext.Clients.All.VideoStreamsRefresh().ConfigureAwait(false);
    }
}