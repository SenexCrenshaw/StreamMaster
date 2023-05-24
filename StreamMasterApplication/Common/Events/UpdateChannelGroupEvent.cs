using StreamMasterDomain.Dto;

namespace StreamMasterApplication.Common.Events;

public class UpdateChannelGroupEvent : BaseEvent
{
    public UpdateChannelGroupEvent(ChannelGroupDto item)
    {
        Item = item;
    }

    public ChannelGroupDto Item { get; }
}
