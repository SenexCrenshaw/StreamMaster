using StreamMasterApplication.ChannelGroups.Commands;
using StreamMasterApplication.ChannelGroups.Events;
using StreamMasterApplication.ChannelGroups.Queries;
using StreamMasterApplication.StreamGroupChannelGroups.Queries;

namespace StreamMasterApplication.ChannelGroups.EventHandlers;

public class UpdateChannelGroupEventHandler(ILogger<UpdateChannelGroupEvent> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext) : BaseMediatorRequestHandler(logger, repository, mapper, publisher, sender, hubContext), INotificationHandler<UpdateChannelGroupEvent>
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

        if (sgs.Any())
        {
            await HubContext.Clients.All.StreamGroupsRefresh(sgs.ToArray()).ConfigureAwait(false);
        }
    }
}