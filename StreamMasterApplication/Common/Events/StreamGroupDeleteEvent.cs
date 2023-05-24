namespace StreamMasterApplication.Common.Events;

public class StreamGroupDeleteEvent : BaseEvent
{
    public StreamGroupDeleteEvent(int streamGroupId)
    {
        StreamGroupId = streamGroupId;
    }

    public int StreamGroupId { get; }
}