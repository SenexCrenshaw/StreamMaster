using StreamMaster.Domain.Common;
using StreamMaster.Domain.Dto;

namespace StreamMaster.Application.Common.Events;

public class EPGFileProcessedEvent : BaseEvent
{
    public EPGFileProcessedEvent(EPGFileDto ePGFile)
    {
        EPGFile = ePGFile;
    }

    public EPGFileDto EPGFile { get; }
}
