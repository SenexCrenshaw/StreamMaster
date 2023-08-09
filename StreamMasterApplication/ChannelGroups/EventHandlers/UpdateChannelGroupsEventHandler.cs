using MediatR;

using Microsoft.AspNetCore.SignalR;

using StreamMasterApplication.Hubs;

namespace StreamMasterApplication.ChannelGroups.EventHandlers;

public class UpdateChannelGroupsEventHandler : INotificationHandler<UpdateChannelGroupsEvent>
{
    private readonly IHubContext<StreamMasterHub, IStreamMasterHub> _hubContext;

    public UpdateChannelGroupsEventHandler(
        IHubContext<StreamMasterHub, IStreamMasterHub> hubContext
        )
    {
        _hubContext = hubContext;
    }

    public async Task Handle(UpdateChannelGroupsEvent notification, CancellationToken cancellationToken)
    {
        await _hubContext.Clients.All.ChannelGroupsRefresh().ConfigureAwait(false);
    }
}