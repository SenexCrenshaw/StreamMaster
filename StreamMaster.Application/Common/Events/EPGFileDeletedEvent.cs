using StreamMaster.Domain.Common;

namespace StreamMaster.Application.Common.Events;

public class EPGFileDeletedEvent : BaseEvent
{
    public EPGFileDeletedEvent(int _epgFileId)
    {
        EPGFileId = _epgFileId;
    }

    public int EPGFileId { get; }
}
