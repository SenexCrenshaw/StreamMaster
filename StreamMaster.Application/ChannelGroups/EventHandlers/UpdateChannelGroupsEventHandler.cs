using StreamMaster.Application.ChannelGroups.Commands;
using StreamMaster.Application.ChannelGroups.Events;
using StreamMaster.Application.Interfaces;


namespace StreamMaster.Application.ChannelGroups.EventHandlers;

public class UpdateChannelGroupsEventHandler(ILogger<UpdateChannelGroupsEvent> logger, IMapper mapper, ISender Sender, IHubContext<StreamMasterHub, IStreamMasterHub> HubContext)
    : INotificationHandler<UpdateChannelGroupsEvent>
{
    public async Task Handle(UpdateChannelGroupsEvent notification, CancellationToken cancellationToken)
    {
        await Sender.Send(new UpdateChannelGroupCountsRequest(notification.ChannelGroups), cancellationToken).ConfigureAwait(false);
        List<int> ids = notification.ChannelGroups.Select(x => x.Id).ToList();
        //IEnumerable<StreamGroupDto> sgs = await Sender.Send(new GetStreamGroupsFromChannelGroupsQuery(ids), cancellationToken).ConfigureAwait(false);

        //ChannelGroupDto[] dtos = mapper.Map<ChannelGroupDto[]>(notification.ChannelGroups);
        //await HubContext.Clients.All.ChannelGroupsRefresh(dtos).ConfigureAwait(false);
        //if (sgs.Any())
        //{
        //    await HubContext.Clients.All.StreamGroupsRefresh(sgs.ToArray()).ConfigureAwait(false);
        //}


    }
}