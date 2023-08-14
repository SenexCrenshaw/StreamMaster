using MediatR;

using Microsoft.AspNetCore.SignalR;

using StreamMasterApplication.ChannelGroups.Commands;
using StreamMasterApplication.Hubs;

namespace StreamMasterApplication.ChannelGroups.EventHandlers;

public class UpdateChannelGroupEventHandler : INotificationHandler<UpdateChannelGroupEvent>
{
    private readonly IHubContext<StreamMasterHub, IStreamMasterHub> _hubContext;
    private readonly ISender _sender;

    public UpdateChannelGroupEventHandler(IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, ISender sender)
    {
        _hubContext = hubContext;
        _sender = sender;
    }

    public async Task Handle(UpdateChannelGroupEvent notification, CancellationToken cancellationToken)
    {
        await _sender.Send(new UpdateChannelGroupCountRequest(notification.ChannelGroupDto.Name), cancellationToken).ConfigureAwait(false);
        await _hubContext.Clients.All.ChannelGroupsRefresh().ConfigureAwait(false);
    }
}