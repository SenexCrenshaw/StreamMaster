using StreamMasterApplication.ChannelGroups.Events;
using StreamMasterApplication.StreamGroupChannelGroups.Queries;

namespace StreamMasterApplication.ChannelGroups.EventHandlers;

public class UpdateChannelGroupEventHandler(ILogger<UpdateChannelGroupEvent> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache) : BaseMediatorRequestHandler(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache), INotificationHandler<UpdateChannelGroupEvent>
{
    public async Task Handle(UpdateChannelGroupEvent notification, CancellationToken cancellationToken)
    {

        await HubContext.Clients.All.ChannelGroupsRefresh([notification.ChannelGroup]).ConfigureAwait(false);

        if (notification.ChannelGroupToggelVisibility || notification.ChannelGroupNameChanged)
        {
            List<VideoStreamDto> streams = await repository.VideoStream.GetVideoStreamsForChannelGroup(notification.ChannelGroup.Id, cancellationToken).ConfigureAwait(false);
            if (streams.Any())
            {
                if (!notification.ChannelGroupNameChanged)
                {
                    IEnumerable<IDIsHidden> ids = streams.Select(a => new IDIsHidden { Id = a.Id, IsHidden = a.IsHidden });
                    await HubContext.Clients.All.VideoStreamsVisibilityRefresh(ids).ConfigureAwait(false);
                }
                else
                {
                    await HubContext.Clients.All.VideoStreamsRefresh(streams.ToArray()).ConfigureAwait(false);
                }
            }


            IEnumerable<StreamGroupDto> sgs = await Sender.Send(new GetStreamGroupsFromChannelGroupQuery(notification.ChannelGroup.Id), cancellationToken).ConfigureAwait(false);

            if (sgs.Any())
            {
                await HubContext.Clients.All.StreamGroupsRefresh(sgs.ToArray()).ConfigureAwait(false);
            }
        }
    }
}