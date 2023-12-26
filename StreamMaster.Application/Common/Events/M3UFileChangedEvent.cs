using StreamMaster.Domain.Common;
using StreamMaster.Domain.Dto;

namespace StreamMaster.Application.Common.Events;

public class M3UFileChangedEvent : BaseEvent
{
    public M3UFileChangedEvent(M3UFileDto m3UFile)
    {
        M3UFile = m3UFile;
    }

    public M3UFileDto M3UFile { get; }
}
