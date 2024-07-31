using System.Threading.Channels;

namespace StreamMaster.Streams.Domain.Interfaces;
public interface IVideoCombinerService
{
    Task CombineVideosServiceAsync(IClientConfiguration config, int SMChannelId1, int SMChannelId2, int SMChannelId3, int SMChannelId4, ChannelWriter<byte[]> channelWriter, CancellationToken cancellationToken);
}