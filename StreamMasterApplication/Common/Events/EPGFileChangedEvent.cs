using StreamMasterDomain.Dto;

namespace StreamMasterApplication.Common.Events;

public class EPGFileChangedEvent : BaseEvent
{
    public EPGFileChangedEvent(EPGFilesDto epgFile)
    {
        EPGFile = epgFile;
    }

    public EPGFilesDto EPGFile { get; }
}
