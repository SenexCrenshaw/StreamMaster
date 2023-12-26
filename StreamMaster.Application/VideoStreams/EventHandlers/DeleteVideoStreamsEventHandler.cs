using StreamMaster.Domain.Dto;
using StreamMaster.Domain.Repository;
using StreamMaster.Domain.Services;

using StreamMaster.Application.ChannelGroups.Events;
using StreamMaster.Application.ChannelGroups.Queries;
using StreamMaster.Application.VideoStreams.Events;

namespace StreamMaster.Application.VideoStreams.EventHandlers;

public class DeleteVideoStreamsEventHandler : BaseMediatorRequestHandler, INotificationHandler<DeleteVideoStreamsEvent>
{
    public DeleteVideoStreamsEventHandler(ILogger<DeleteVideoStreamsEvent> logger, IRepositoryWrapper repository, IMapper mapper,ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache)
: base(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache) { }

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