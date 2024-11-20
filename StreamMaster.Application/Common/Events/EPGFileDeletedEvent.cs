using StreamMaster.Domain.Common;

namespace StreamMaster.Application.Common.Events;

public class EPGFileDeletedEvent(int _epgFileId) : BaseEvent
{
    public int EPGFileId { get; } = _epgFileId;
}
