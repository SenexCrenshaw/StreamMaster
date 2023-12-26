using StreamMaster.Domain.Common;
using StreamMaster.Domain.Dto;

namespace StreamMaster.Application.Common.Events;

public class EPGFileAddedEvent : BaseEvent
{
    public EPGFileAddedEvent(EPGFileDto item)
    {
        Item = item;
    }

    public EPGFileDto Item { get; }
}
