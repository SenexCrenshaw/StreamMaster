namespace StreamMaster.Application.Common.Events;

public class M3UFileProcessEvent : BaseEvent
{
    public M3UFileProcessEvent(int M3UFileId, bool ForecRun)
    {
        this.M3UFileId = M3UFileId;
        this.ForecRun = ForecRun;

    }

    public bool ForecRun { get; }
    public int M3UFileId { get; }
}
