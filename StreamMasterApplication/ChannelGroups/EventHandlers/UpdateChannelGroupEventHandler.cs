using StreamMasterApplication.ChannelGroups.Commands;
using StreamMasterApplication.ChannelGroups.Events;
using StreamMasterApplication.ChannelGroups.Queries;
using StreamMasterApplication.StreamGroupChannelGroups.Queries;
using StreamMasterApplication.VideoStreams.Queries;

namespace StreamMasterApplication.ChannelGroups.EventHandlers;

public class UpdateChannelGroupEventHandler(ILogger<UpdateChannelGroupEvent> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext) : BaseMediatorRequestHandler(logger, repository, mapper, settingsService, publisher, sender, hubContext), INotificationHandler<UpdateChannelGroupEvent>
{
    public async Task Handle(UpdateChannelGroupEvent notification, CancellationToken cancellationToken)
    {
        if (notification.ChannelGroup == null)
        {
            return;
        }

        await Sender.Send(new UpdateChannelGroupCountRequest(notification.ChannelGroup), cancellationToken).ConfigureAwait(false);
        IEnumerable<StreamGroupDto> sgs = await Sender.Send(new GetStreamGroupsFromChannelGroupQuery(notification.ChannelGroup.Id), cancellationToken).ConfigureAwait(false);
        ChannelGroupDto? ret = await Sender.Send(new GetChannelGroup(notification.ChannelGroup.Id), cancellationToken).ConfigureAwait(false);

        if (ret != null)
        {
            await HubContext.Clients.All.ChannelGroupsRefresh([ret]).ConfigureAwait(false);
        }


        if (notification.ToggelVisibility)
        {
            List<VideoStreamDto> streams = await Sender.Send(new GetVideoStreamsForChannelGroups([notification.ChannelGroup.Id]), cancellationToken).ConfigureAwait(false);

            if (streams.Any())
            {
                IEnumerable<IDIsHidden> ids = streams.Select(a => new IDIsHidden { Id = a.Id, IsHidden = a.IsHidden });
                await HubContext.Clients.All.VideoStreamsVisibilityRefresh(ids).ConfigureAwait(false);
            }
        }

        if (sgs.Any())
        {
            await HubContext.Clients.All.StreamGroupsRefresh(sgs.ToArray()).ConfigureAwait(false);
        }
    }
}