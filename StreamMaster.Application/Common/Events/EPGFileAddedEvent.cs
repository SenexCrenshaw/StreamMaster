using StreamMaster.Domain.Common;
using StreamMaster.Domain.Dto;

namespace StreamMaster.Application.Common.Events;

public class EPGFileAddedEvent(EPGFileDto item) : BaseEvent
{
    public EPGFileDto Item { get; } = item;
}
