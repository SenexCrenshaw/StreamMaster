using StreamMaster.Application.ChannelGroups.Events;


namespace StreamMaster.Application.ChannelGroups.EventHandlers;

public class UpdateChannelGroupEventHandler(IDataRefreshService dataRefreshService, IChannelGroupService channelGroupService)
    : INotificationHandler<UpdateChannelGroupEvent>
{
    public async Task Handle(UpdateChannelGroupEvent notification, CancellationToken cancellationToken)
    {
        List<FieldData> fds = [];
        if (notification.ChannelGroupToggelVisibility)
        {
            await channelGroupService.UpdateChannelGroupCountRequestAsync(notification.ChannelGroup).ConfigureAwait(false);
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

    }
}