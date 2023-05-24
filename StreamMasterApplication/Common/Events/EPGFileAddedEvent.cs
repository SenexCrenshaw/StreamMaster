using StreamMasterDomain.Dto;

namespace StreamMasterApplication.Common.Events;

public class EPGFileAddedEvent : BaseEvent
{
    public EPGFileAddedEvent(EPGFilesDto item)
    {
        Item = item;
    }

    public EPGFilesDto Item { get; }
}
