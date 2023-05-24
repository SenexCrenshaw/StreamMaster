using StreamMasterDomain.Dto;

namespace StreamMasterApplication.Common.Events;

public class IconFileAddedEvent : BaseEvent
{
    public IconFileAddedEvent(IconFileDto item)
    {
        Item = item;
    }

    public IconFileDto Item { get; }
}
