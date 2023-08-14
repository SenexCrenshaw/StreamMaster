using StreamMasterDomain.Dto;

namespace StreamMasterApplication.Common.Events;

public class UpdateChannelGroupEvent : BaseEvent
{
    public UpdateChannelGroupEvent(ChannelGroupDto item)
    {
        ChannelGroupDto = item;
    }

    public ChannelGroupDto ChannelGroupDto { get; }
}
