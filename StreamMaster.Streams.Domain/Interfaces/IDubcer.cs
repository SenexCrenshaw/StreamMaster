using System.Threading.Channels;

namespace StreamMaster.Streams.Domain.Interfaces;
public interface IDubcer
{
    void DubcerChannels(ChannelReader<byte[]> channelReader, ChannelWriter<byte[]> channelWriter, CancellationToken cancellationToken);
}