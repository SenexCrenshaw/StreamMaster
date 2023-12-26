using StreamMaster.Domain.Common;
using StreamMaster.Domain.Dto;

namespace StreamMaster.Application.StreamGroups.Events;

public class StreamGroupCreateEvent : BaseEvent
{
    public StreamGroupCreateEvent(StreamGroupDto streamGroup)
    {
        StreamGroup = streamGroup;
    }

    public StreamGroupDto StreamGroup { get; set; }

}