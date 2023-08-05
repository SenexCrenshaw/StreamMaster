using StreamMasterDomain.Dto;

namespace StreamMasterApplication.Common.Events;

public class AddChannelGroupsEvent : BaseEvent
{
    public AddChannelGroupsEvent(List<ChannelGroupDto> items)
    {
        Items = items;
    }

    public List<ChannelGroupDto> Items { get; }
}

public class AddChannelGroupEvent : BaseEvent
{
    public AddChannelGroupEvent(ChannelGroupDto item)
    {
        Item = item;
    }

    public ChannelGroupDto Item { get; }
}
