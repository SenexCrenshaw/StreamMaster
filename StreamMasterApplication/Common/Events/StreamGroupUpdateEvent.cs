namespace StreamMasterApplication.Common.Events;

public class StreamGroupUpdateEvent : BaseEvent
{
    public StreamGroupUpdateEvent(StreamGroupDto streamGroupDto)
    {
        StreamGroupDto = streamGroupDto;
    }
    public StreamGroupDto StreamGroupDto { get; set; }
}