namespace StreamMaster.Streams.Domain.Interfaces
{
    public interface ISourceProcessingService
    {
        Task ProcessSourceChannelReaderAsync(TrackedChannel sourceChannelReader, TrackedChannel newChannel, IMetricsService metricsService, CancellationToken token);
        Task ProcessSourceStreamAsync(Stream sourceStream, TrackedChannel newChannel, IMetricsService metricsService, CancellationToken token);
    }
}