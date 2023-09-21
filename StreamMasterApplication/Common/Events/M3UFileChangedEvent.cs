using StreamMasterDomain.Dto;

namespace StreamMasterApplication.Common.Events;

public class M3UFileChangedEvent : BaseEvent
{
    public M3UFileChangedEvent(M3UFileDto m3UFile)
    {
        M3UFile = m3UFile;
    }

    public M3UFileDto M3UFile { get; }
}
