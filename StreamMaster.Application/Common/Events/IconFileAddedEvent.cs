using StreamMaster.Domain.Common;
using StreamMaster.Domain.Dto;

namespace StreamMaster.Application.Common.Events;

public class IconFileAddedEvent : BaseEvent
{
    public IconFileAddedEvent(LogoFileDto item)
    {
        Item = item;
    }

    public LogoFileDto Item { get; }
}
