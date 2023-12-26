using StreamMaster.Domain.Common;
using StreamMaster.Domain.Dto;

namespace StreamMaster.Application.Common.Events;

public class EPGFileChangedEvent : BaseEvent
{
    public EPGFileChangedEvent(EPGFileDto epgFile)
    {
        EPGFile = epgFile;
    }

    public EPGFileDto EPGFile { get; }
}
