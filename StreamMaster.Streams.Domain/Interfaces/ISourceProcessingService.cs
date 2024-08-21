using System.Threading.Channels;

namespace StreamMaster.Streams.Domain.Interfaces
{
    public interface ISourceProcessingService
    {
        Task ProcessSourceChannelReaderAsync(ChannelReader<byte[]> sourceChannelReader, Channel<byte[]> newChannel, IMetricsService metricsService, CancellationToken token);
        Task ProcessSourceStreamAsync(Stream sourceStream, Channel<byte[]> newChannel, IMetricsService metricsService, CancellationToken token);
    }
}