namespace StreamMaster.Application.Common.Events;

public class EPGFileChangedEvent(EPGFileDto epgFile) : BaseEvent
{
    public EPGFileDto EPGFile { get; } = epgFile;
}
