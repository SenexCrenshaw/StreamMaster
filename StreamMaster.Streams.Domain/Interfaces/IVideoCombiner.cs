using System.Threading.Channels;

namespace StreamMaster.Streams.Domain.Interfaces
{
    public interface IVideoCombiner
    {
        Task CombineVideosAsync(Stream stream1, Stream stream2, Stream stream3, Stream stream4, ChannelWriter<byte[]> writer, CancellationToken cancellationToken);
    }
}