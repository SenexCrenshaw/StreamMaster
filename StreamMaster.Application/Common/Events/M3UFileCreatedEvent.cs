namespace StreamMaster.Application.Common.Events;

public class M3UFileAddedEvent : BaseEvent
{
    public M3UFileAddedEvent(int M3UFileId, bool ForecRun)
    {
        this.M3UFileId = M3UFileId;
        this.ForecRun = ForecRun;

    }

    public bool ForecRun { get; }
    public int M3UFileId { get; }
}
