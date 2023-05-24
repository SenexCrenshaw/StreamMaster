using MediatR;

using Microsoft.AspNetCore.SignalR;

using StreamMasterApplication.Hubs;

namespace StreamMasterApplication.ChannelGroups.EventHandlers;

public class UpdateChannelGroupEventHandler : INotificationHandler<UpdateChannelGroupEvent>
{
    private readonly IHubContext<StreamMasterHub, IStreamMasterHub> _hubContext;

    public UpdateChannelGroupEventHandler(IHubContext<StreamMasterHub, IStreamMasterHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task Handle(UpdateChannelGroupEvent notification, CancellationToken cancellationToken)
    {
        await _hubContext.Clients.All.ChannelGroupDtoUpdate(notification.Item).ConfigureAwait(false);
    }
}
