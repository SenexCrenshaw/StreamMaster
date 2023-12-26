using StreamMaster.Domain.Dto;
using StreamMaster.Domain.Repository;
using StreamMaster.Domain.Services;

using StreamMaster.Application.ChannelGroups.Commands;
using StreamMaster.Application.ChannelGroups.Events;
using StreamMaster.Application.ChannelGroups.Queries;
using StreamMaster.Application.VideoStreams.Events;

namespace StreamMaster.Application.VideoStreams.EventHandlers;

public class UpdateVideoStreamEventHandler(ILogger<UpdateVideoStreamEvent> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache) : BaseMediatorRequestHandler(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache), INotificationHandler<UpdateVideoStreamEvent>
{
    public async Task Handle(UpdateVideoStreamEvent notification, CancellationToken cancellationToken = default)
    {
        //if (notification.ToggelVisibility)
        //{
        //    ChannelGroupDto? channelGroup = await Sender.Send(new GetChannelGroupByName(notification.VideoStream.User_Tvg_group), cancellationToken).ConfigureAwait(false);
        //    if (channelGroup != null)
        //    {
        //        await Publisher.Publish(new UpdateChannelGroupEvent(channelGroup, false, false), cancellationToken).ConfigureAwait(false);
        //    }
        //}

        ChannelGroupDto? channelGroup = await Sender.Send(new GetChannelGroupByName(notification.VideoStream.User_Tvg_group), cancellationToken).ConfigureAwait(false);
        if (channelGroup != null)
        {
            await Sender.Send(new UpdateChannelGroupCountRequest(channelGroup, false), cancellationToken).ConfigureAwait(false);
            await Publisher.Publish(new UpdateChannelGroupEvent(channelGroup, false, false), cancellationToken).ConfigureAwait(false);
        }

        await HubContext.Clients.All.VideoStreamLinksRefresh([notification.VideoStream.Id]);

        await HubContext.Clients.All.VideoStreamsRefresh([notification.VideoStream]).ConfigureAwait(false);
        await HubContext.Clients.All.StreamGroupVideoStreamsRefresh().ConfigureAwait(false);
    }
}