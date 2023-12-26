

using StreamMaster.Domain.Dto;
using StreamMaster.Domain.Repository;
using StreamMaster.Domain.Services;

using StreamMaster.Application.ChannelGroups.Commands;
using StreamMaster.Application.ChannelGroups.Events;
using StreamMaster.Application.StreamGroupChannelGroups.Queries;

namespace StreamMaster.Application.ChannelGroups.EventHandlers;

public class UpdateChannelGroupsEventHandler(ILogger<UpdateChannelGroupsEvent> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache) : BaseMediatorRequestHandler(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache), INotificationHandler<UpdateChannelGroupsEvent>
{
    public async Task Handle(UpdateChannelGroupsEvent notification, CancellationToken cancellationToken)
    {
        await Sender.Send(new UpdateChannelGroupCountsRequest(notification.ChannelGroups), cancellationToken).ConfigureAwait(false);
        List<int> ids = notification.ChannelGroups.Select(x => x.Id).ToList();
        IEnumerable<StreamGroupDto> sgs = await Sender.Send(new GetStreamGroupsFromChannelGroupsQuery(ids), cancellationToken).ConfigureAwait(false);

        await HubContext.Clients.All.ChannelGroupsRefresh(notification.ChannelGroups.ToArray()).ConfigureAwait(false);
        if (sgs.Any())
        {
            await HubContext.Clients.All.StreamGroupsRefresh(sgs.ToArray()).ConfigureAwait(false);
        }


    }
}