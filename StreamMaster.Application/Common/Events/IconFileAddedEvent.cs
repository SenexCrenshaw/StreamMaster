using StreamMaster.Domain.Common;
using StreamMaster.Domain.Dto;

namespace StreamMaster.Application.Common.Events;

public class IconFileAddedEvent(LogoFileDto item) : BaseEvent
{
    public LogoFileDto Item { get; } = item;
}
