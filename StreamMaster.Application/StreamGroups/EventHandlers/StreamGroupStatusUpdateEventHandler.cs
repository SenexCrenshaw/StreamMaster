using StreamMaster.Application.StreamGroups.Events;

namespace StreamMaster.Application.StreamGroups.EventHandlers;

public class StreamGroupStatusUpdateEventHandler() : INotificationHandler<StreamGroupStatusUpdateEvent>
{
    public static void Handle(StreamGroupStatusUpdateEvent notification, CancellationToken cancellationToken)
    {

        //await _hubContext.ClientChannels.All.StreamingStatusDtoUpdate(status).ConfigureAwait(false);
    }

    Task INotificationHandler<StreamGroupStatusUpdateEvent>.Handle(StreamGroupStatusUpdateEvent notification, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
