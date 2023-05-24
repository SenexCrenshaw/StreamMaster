using MediatR;

using Microsoft.AspNetCore.SignalR;

using StreamMasterApplication.Hubs;

namespace StreamMasterApplication.ChannelGroups.EventHandlers;

public class DeleteChannelGroupEventHandler : INotificationHandler<DeleteChannelGroupEvent>
{
    private readonly IHubContext<StreamMasterHub, IStreamMasterHub> _hubContext;

    public DeleteChannelGroupEventHandler(IHubContext<StreamMasterHub, IStreamMasterHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task Handle(DeleteChannelGroupEvent notification, CancellationToken cancellationToken)
    {
        await _hubContext.Clients.All.ChannelGroupDtoDelete(notification.ChannelGroupId).ConfigureAwait(false);
    }
}
