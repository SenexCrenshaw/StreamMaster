using StreamMasterDomain.Dto;

namespace StreamMasterApplication.Common.Events;

public class EPGFileChangedEvent : BaseEvent
{
    public EPGFileChangedEvent(EPGFileDto epgFile)
    {
        EPGFile = epgFile;
    }

    public EPGFileDto EPGFile { get; }
}
