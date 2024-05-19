using StreamMaster.Application.ChannelGroups.Events;

namespace StreamMaster.Application.ChannelGroups.EventHandlers;

public class DeleteChannelGroupEventHandler(IDataRefreshService dataRefreshService)
: INotificationHandler<DeleteChannelGroupEvent>
{
    public async Task Handle(DeleteChannelGroupEvent notification, CancellationToken cancellationToken)
    {
        await dataRefreshService.RefreshChannelGroups().ConfigureAwait(false);
    }
}