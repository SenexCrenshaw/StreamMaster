using StreamMaster.Domain.Common;

namespace StreamMaster.Application.Common.Events;

public class M3UFileDeletedEvent : BaseEvent
{
    public M3UFileDeletedEvent(int m3uFileId)
    {
        M3UFileId = m3uFileId;
    }

    public int M3UFileId { get; }
}
