using StreamMasterDomain.Dto;

namespace StreamMasterApplication.Common.Events;

public class M3UFileAddedEvent : BaseEvent
{
    public M3UFileAddedEvent(M3UFileDto item)
    {
        Item = item;
    }

    public M3UFileDto Item { get; }
}
