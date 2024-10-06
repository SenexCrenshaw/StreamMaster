using StreamMaster.Application.ChannelGroups.Events;


namespace StreamMaster.Application.ChannelGroups.EventHandlers;

public class UpdateChannelGroupsEventHandler(IChannelGroupService channelGroupService, IDataRefreshService dataRefreshService)
    : INotificationHandler<UpdateChannelGroupsEvent>
{
    public async Task Handle(UpdateChannelGroupsEvent notification, CancellationToken cancellationToken)
    {
        await channelGroupService.UpdateChannelGroupCountsRequestAsync(notification.ChannelGroups);
        await dataRefreshService.RefreshChannelGroups();

        //List<int> ids = notification.ChannelGroups.Select(x => x.Id).ToList();
        //IEnumerable<StreamGroupDto> sgs = await Sender.Send(new GetStreamGroupsFromChannelGroupsQuery(ids), cancellationToken).ConfigureAwait(false);

        //ChannelGroupDto[] dtos = mapper.Map<ChannelGroupDto[]>(notification.ChannelGroups);
        //await HubContext.ClientChannels.All.ChannelGroupsRefresh(dtos).ConfigureAwait(false);
        //if (sgs.Any())
        //{
        //    await HubContext.ClientChannels.All.StreamGroupsRefresh(sgs.ToArray()).ConfigureAwait(false);
        //}


    }
}