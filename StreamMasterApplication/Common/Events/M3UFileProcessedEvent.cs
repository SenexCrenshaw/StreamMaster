using StreamMasterDomain.Dto;

namespace StreamMasterApplication.Common.Events;

public class M3UFileProcessedEvent : BaseEvent
{
    public M3UFileProcessedEvent(M3UFilesDto m3UFile)
    {
        M3UFile = m3UFile;
    }

    public M3UFilesDto M3UFile { get; }
}
