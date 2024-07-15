namespace StreamMaster.Application.Common.Events;

public class M3UFileChangedEvent(M3UFileDto m3UFile) : BaseEvent
{
    public M3UFileDto M3UFile { get; } = m3UFile;
}
