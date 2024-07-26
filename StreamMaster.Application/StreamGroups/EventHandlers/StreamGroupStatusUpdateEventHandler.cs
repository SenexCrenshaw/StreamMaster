using StreamMaster.Application.Interfaces;
using StreamMaster.Application.StreamGroups.Events;

namespace StreamMaster.Application.StreamGroups.EventHandlers;

public class StreamGroupStatusUpdateEventHandler(IHubContext<StreamMasterHub, IStreamMasterHub> hubContext) : INotificationHandler<StreamGroupStatusUpdateEvent>
{
    public static void Handle(StreamGroupStatusUpdateEvent notification, CancellationToken cancellationToken)
    {

        //await _hubContext.Clients.All.StreamingStatusDtoUpdate(status).ConfigureAwait(false);
    }

    Task INotificationHandler<StreamGroupStatusUpdateEvent>.Handle(StreamGroupStatusUpdateEvent notification, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
