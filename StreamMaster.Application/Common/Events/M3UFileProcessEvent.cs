namespace StreamMaster.Application.Common.Events;

public class M3UFileProcessEvent(int M3UFileId, bool ForecRun) : BaseEvent
{
    public bool ForecRun { get; } = ForecRun;
    public int M3UFileId { get; } = M3UFileId;
}
