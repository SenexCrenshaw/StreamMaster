namespace StreamMaster.Application.Common.Events;

public class EPGFileProcessedEvent(EPGFileDto ePGFile) : BaseEvent
{
    public EPGFileDto EPGFile { get; } = ePGFile;
}
