using StreamMaster.Domain.Common;

namespace StreamMaster.Application.Common.Events;

public class DeleteChannelsEvent : BaseEvent
{
    public DeleteChannelsEvent(int channelId)
    {
        ChannelId = channelId;
    }

    public int ChannelId { get; }
}
