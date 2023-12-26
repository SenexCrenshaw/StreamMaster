using StreamMaster.Domain.Dto;
using StreamMaster.Domain.Repository;
using StreamMaster.Domain.Services;

using StreamMaster.Application.ChannelGroups.Events;
using StreamMaster.Application.ChannelGroups.Queries;
using StreamMaster.Application.VideoStreams.Events;

namespace StreamMaster.Application.VideoStreams.EventHandlers;

public class DeleteVideoStreamEventHandler : BaseMediatorRequestHandler, INotificationHandler<DeleteVideoStreamEvent>
{
    public DeleteVideoStreamEventHandler(ILogger<DeleteVideoStreamEvent> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache)
: base(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache) { }

    public async Task Handle(DeleteVideoStreamEvent notification, CancellationToken cancellationToken = default)
    {
        ChannelGroupDto? channelGroup = await Sender.Send(new GetChannelGroupFromVideoStreamId(notification.VideoStreamId), cancellationToken).ConfigureAwait(false);
        if (channelGroup != null)
        {
            await Sender.Send(new UpdateChannelGroupEvent(channelGroup, true, false), cancellationToken).ConfigureAwait(false);
            await Publisher.Publish(new UpdateChannelGroupEvent(channelGroup, false, false), cancellationToken).ConfigureAwait(false);
        }
        await HubContext.Clients.All.VideoStreamsRefresh().ConfigureAwait(false);
    }
}