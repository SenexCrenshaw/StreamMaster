using StreamMasterDomain.Dto;

namespace StreamMasterApplication.Common.Events;

public class AddChannelGroupsEvent : BaseEvent
{
}

public class AddChannelGroupEvent : BaseEvent
{
    public AddChannelGroupEvent(ChannelGroupDto item)
    {
        Item = item;
    }

    public ChannelGroupDto Item { get; }
}
