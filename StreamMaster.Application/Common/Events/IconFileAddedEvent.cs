using StreamMaster.Domain.Common;
using StreamMaster.Domain.Dto;

namespace StreamMaster.Application.Common.Events;

public class IconFileAddedEvent : BaseEvent
{
    public IconFileAddedEvent(IconFileDto item)
    {
        Item = item;
    }

    public IconFileDto Item { get; }
}
