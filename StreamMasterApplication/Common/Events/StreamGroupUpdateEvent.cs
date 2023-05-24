using StreamMasterDomain.Dto;

namespace StreamMasterApplication.Common.Events;

public class StreamGroupUpdateEvent : BaseEvent
{
    public StreamGroupUpdateEvent(StreamGroupDto streamGroup)
    {
        StreamGroup = streamGroup;
    }

    public StreamGroupDto StreamGroup { get; }
}