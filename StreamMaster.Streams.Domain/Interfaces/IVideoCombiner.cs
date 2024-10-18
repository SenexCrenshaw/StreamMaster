using StreamMaster.Streams.Domain.Events;

namespace StreamMaster.Streams.Domain.Interfaces
{
    public interface IVideoCombiner : IBroadcasterBase
    {
        int Id { get; }
        event EventHandler<VideoCombinerStopped>? OnVideoCombinerStoppedEvent;
        Task CombineVideosAsync(int SMChannelId1, int SMChannelId2, int SMChannelId3, int SMChannelId4, CancellationToken cancellationToken);
    }
}