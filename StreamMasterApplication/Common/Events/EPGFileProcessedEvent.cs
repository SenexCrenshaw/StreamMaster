using StreamMasterDomain.Dto;

namespace StreamMasterApplication.Common.Events;

public class EPGFileProcessedEvent : BaseEvent
{
    public EPGFileProcessedEvent(EPGFilesDto ePGFile)
    {
        EPGFile = ePGFile;
    }

    public EPGFilesDto EPGFile { get; }
}
