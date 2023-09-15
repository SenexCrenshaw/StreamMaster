namespace StreamMasterApplication.StreamGroups.Events;

public class StreamGroupCreateEvent : BaseEvent
{
    public StreamGroupCreateEvent(StreamGroupDto streamGroup)
    {
        StreamGroup = streamGroup;
    }

    public StreamGroupDto StreamGroup { get; set; }

}