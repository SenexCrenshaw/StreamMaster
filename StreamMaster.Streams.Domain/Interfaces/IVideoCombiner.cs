using StreamMaster.Domain.Models;
using StreamMaster.Streams.Domain.Events;

using System.Threading.Channels;

namespace StreamMaster.Streams.Domain.Interfaces
{
    public interface IVideoCombiner : IBroadcasterBase
    {
        public SMStreamInfo SMStreamInfo { get; }
        string Id { get; }
        event EventHandler<VideoCombinerStopped>? OnVideoCombinerStoppedEvent;
        Task CombineVideosAsync(Stream stream1, Stream stream2, Stream stream3, Stream stream4, ChannelWriter<byte[]> writer, CancellationToken cancellationToken);
    }
}