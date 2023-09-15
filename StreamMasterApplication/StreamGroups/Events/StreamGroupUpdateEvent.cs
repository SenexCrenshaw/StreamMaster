namespace StreamMasterApplication.StreamGroups.Events;

public class StreamGroupUpdateEvent : BaseEvent
{
    public StreamGroupUpdateEvent(StreamGroupDto streamGroup)
    {
        StreamGroup = streamGroup;
    }
    public StreamGroupDto StreamGroup { get; set; }
}