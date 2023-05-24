using StreamMasterDomain.Dto;

namespace StreamMasterApplication.Common.Events;

public class M3UFileAddedEvent : BaseEvent
{
    public M3UFileAddedEvent(M3UFilesDto item)
    {
        Item = item;
    }

    public M3UFilesDto Item { get; }
}
