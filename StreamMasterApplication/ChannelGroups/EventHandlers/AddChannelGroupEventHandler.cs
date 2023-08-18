using MediatR;

using Microsoft.AspNetCore.SignalR;
using StreamMasterApplication.ChannelGroups.Events;
using StreamMasterApplication.Hubs;

namespace StreamMasterApplication.ChannelGroups.EventHandlers;

public class AddChannelGroupEventHandler : INotificationHandler<AddChannelGroupEvent>
{
    private readonly IHubContext<StreamMasterHub, IStreamMasterHub> _hubContext;

    public AddChannelGroupEventHandler(
        IHubContext<StreamMasterHub, IStreamMasterHub> hubContext
        )
    {
        _hubContext = hubContext;
    }

    public async Task Handle(AddChannelGroupEvent notification, CancellationToken cancellationToken)
    {
        await _hubContext.Clients.All.ChannelGroupsRefresh().ConfigureAwait(false);
    }
}