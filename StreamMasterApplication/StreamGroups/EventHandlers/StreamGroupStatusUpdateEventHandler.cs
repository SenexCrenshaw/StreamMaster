using MediatR;

using Microsoft.AspNetCore.SignalR;

using StreamMasterApplication.Hubs;

namespace StreamMasterApplication.StreamGroups.EventHandlers;

public class StreamGroupStatusUpdateEventHandler : INotificationHandler<StreamGroupStatusUpdateEvent>
{
    private readonly IHubContext<StreamMasterHub, IStreamMasterHub> _hubContext;
    private readonly ISender _sender;

    public StreamGroupStatusUpdateEventHandler(
        ISender sender,
         IHubContext<StreamMasterHub, IStreamMasterHub> hubContext
        )
    {
        _sender = sender;
        _hubContext = hubContext;
    }

    public void Handle(StreamGroupStatusUpdateEvent notification, CancellationToken cancellationToken)
    {
        //var status = await _sender.Send(new GetStreamingStatus()).ConfigureAwait(false);
        //await _hubContext.Clients.All.StreamingStatusDtoUpdate(status).ConfigureAwait(false);
    }

    Task INotificationHandler<StreamGroupStatusUpdateEvent>.Handle(StreamGroupStatusUpdateEvent notification, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
