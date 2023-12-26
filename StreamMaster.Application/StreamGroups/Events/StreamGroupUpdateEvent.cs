using StreamMaster.Domain.Common;
using StreamMaster.Domain.Dto;

namespace StreamMaster.Application.StreamGroups.Events;

public class StreamGroupUpdateEvent : BaseEvent
{
    public StreamGroupUpdateEvent(StreamGroupDto streamGroup)
    {
        StreamGroup = streamGroup;
    }
    public StreamGroupDto StreamGroup { get; set; }
}