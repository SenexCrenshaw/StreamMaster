namespace StreamMaster.Application.Common.Events;

public class M3UFileAddedEvent : BaseEvent
{
    public M3UFileAddedEvent(int M3UFileId)
    {
        this.M3UFileId = M3UFileId;
    }

    public int M3UFileId { get; }
}
