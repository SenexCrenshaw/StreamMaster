using StreamMaster.Application.ChannelGroups.Commands;
using StreamMaster.Application.ChannelGroups.Events;
using StreamMaster.Application.StreamGroupChannelGroupLinks.Queries;

namespace StreamMaster.Application.ChannelGroups.EventHandlers;

public class UpdateChannelGroupEventHandler(ILogger<UpdateChannelGroupEvent> logger, IDataRefreshService dataRefreshService, IRepositoryWrapper Repository, ISender Sender, IHubContext<StreamMasterHub, IStreamMasterHub> HubContext)
    : INotificationHandler<UpdateChannelGroupEvent>
{
    public async Task Handle(UpdateChannelGroupEvent notification, CancellationToken cancellationToken)
    {
        List<FieldData> fds = [];
        if (notification.ChannelGroupToggelVisibility)
        {
            await Sender.Send(new UpdateChannelGroupCountRequest(notification.ChannelGroup), cancellationToken).ConfigureAwait(false);
            fds.AddRange([
                new(ChannelGroup.APIName, notification.ChannelGroup.Id, "IsHidden", notification.ChannelGroup.IsHidden),
                new(ChannelGroup.APIName, notification.ChannelGroup.Id, "ActiveCount", notification.ChannelGroup.ActiveCount),
                new(ChannelGroup.APIName, notification.ChannelGroup.Id, "TotalCount", notification.ChannelGroup.TotalCount),
                new(ChannelGroup.APIName, notification.ChannelGroup.Id, "HiddenCount", notification.ChannelGroup.HiddenCount),
            ]);

        }

        if (notification.ChannelGroupNameChanged)
        {
            fds.Add(new(ChannelGroup.APIName, notification.ChannelGroup.Id, "Name", notification.ChannelGroup.Name));
        }

        if (fds.Count > 0)
        {
            await dataRefreshService.SetField(fds).ConfigureAwait(false);

        }

        if (notification.ChannelGroupToggelVisibility)
        {
            await dataRefreshService.ClearByTag(ChannelGroup.APIName, "IsHidden").ConfigureAwait(false);
        }

        if (notification.ChannelGroupNameChanged)
        {
            await dataRefreshService.ClearByTag(ChannelGroup.APIName, "Name").ConfigureAwait(false);
        }

        // await dataRefreshService.RefreshChannelGroups();


        IEnumerable<StreamGroupDto> sgs = await Sender.Send(new GetStreamGroupsFromChannelGroupQuery(notification.ChannelGroup.Id), cancellationToken).ConfigureAwait(false);

        if (sgs.Any())
        {
            await HubContext.Clients.All.StreamGroupsRefresh(sgs.ToArray()).ConfigureAwait(false);
        }
    }
}