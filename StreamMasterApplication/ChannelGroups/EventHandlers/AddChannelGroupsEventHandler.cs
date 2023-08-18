using MediatR;

using Microsoft.AspNetCore.SignalR;
using StreamMasterApplication.ChannelGroups.Events;
using StreamMasterApplication.Hubs;

namespace StreamMasterApplication.ChannelGroups.EventHandlers;

public class AddChannelGroupsEventHandler : INotificationHandler<AddChannelGroupsEvent>
{
    private readonly IHubContext<StreamMasterHub, IStreamMasterHub> _hubContext;
    private readonly ISender _sender;

    public AddChannelGroupsEventHandler(ISender sender,
        IHubContext<StreamMasterHub, IStreamMasterHub> hubContext
        )
    {
        _sender = sender;
        _hubContext = hubContext;
    }

    public async Task Handle(AddChannelGroupsEvent notification, CancellationToken cancellationToken)
    {
        await _hubContext.Clients.All.ChannelGroupsRefresh().ConfigureAwait(false);
    }
}