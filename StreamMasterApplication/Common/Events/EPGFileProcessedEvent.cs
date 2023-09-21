using StreamMasterDomain.Dto;

namespace StreamMasterApplication.Common.Events;

public class EPGFileProcessedEvent : BaseEvent
{
    public EPGFileProcessedEvent(EPGFileDto ePGFile)
    {
        EPGFile = ePGFile;
    }

    public EPGFileDto EPGFile { get; }
}
