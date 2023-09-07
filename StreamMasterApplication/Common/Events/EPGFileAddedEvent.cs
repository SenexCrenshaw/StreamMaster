using StreamMasterDomain.Dto;

namespace StreamMasterApplication.Common.Events;

public class EPGFileAddedEvent : BaseEvent
{
    public EPGFileAddedEvent(EPGFileDto item)
    {
        Item = item;
    }

    public EPGFileDto Item { get; }
}
