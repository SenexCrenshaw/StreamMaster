﻿namespace StreamMasterApplication.ChannelGroups.Events;

public class UpdateChannelGroupEvent(ChannelGroupDto channelGroup, bool channelGroupToggelVisibility, bool channelGroupNameChanged) : BaseEvent
{
    public bool ChannelGroupToggelVisibility { get; set; } = channelGroupToggelVisibility;
    public ChannelGroupDto ChannelGroup { get; } = channelGroup;
    public bool ChannelGroupNameChanged { get; internal set; } = channelGroupNameChanged;
}
