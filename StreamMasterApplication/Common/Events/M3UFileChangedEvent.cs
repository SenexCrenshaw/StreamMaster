using StreamMasterDomain.Dto;

namespace StreamMasterApplication.Common.Events;

public class M3UFileChangedEvent : BaseEvent
{
    public M3UFileChangedEvent(M3UFilesDto m3UFile)
    {
        M3UFile = m3UFile;
    }

    public M3UFilesDto M3UFile { get; }
}
