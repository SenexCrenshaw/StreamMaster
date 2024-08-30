using StreamMaster.Streams.Domain.Events;

namespace StreamMaster.Streams.Domain.Interfaces
{
    public interface IVideoCombiner : IBroadcasterBase
    {
        int Id { get; }
        event EventHandler<VideoCombinerStopped>? OnVideoCombinerStoppedEvent;
        Task CombineVideosAsync(IChannelBroadcaster channelBroadcaster1, IChannelBroadcaster channelBroadcaster2, IChannelBroadcaster channelBroadcaster3, IChannelBroadcaster channelBroadcaster4, CancellationToken cancellationToken);
    }
}