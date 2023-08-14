using MediatR;

using Microsoft.AspNetCore.SignalR;

using StreamMasterApplication.ChannelGroups.Commands;
using StreamMasterApplication.Hubs;

namespace StreamMasterApplication.ChannelGroups.EventHandlers;

public class UpdateChannelGroupsEventHandler : INotificationHandler<UpdateChannelGroupsEvent>
{
    private readonly IHubContext<StreamMasterHub, IStreamMasterHub> _hubContext;
    private readonly ISender _sender;
    public UpdateChannelGroupsEventHandler(
        IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, ISender sender
        )
    {
        _hubContext = hubContext;
        _sender = sender;
    }

    public async Task Handle(UpdateChannelGroupsEvent notification, CancellationToken cancellationToken)
    {
        await _sender.Send(new UpdateChannelGroupCountsRequest(notification.ChannelGroups.Select(a => a.Name)), cancellationToken);
        await _hubContext.Clients.All.ChannelGroupsRefresh().ConfigureAwait(false);
    }
}