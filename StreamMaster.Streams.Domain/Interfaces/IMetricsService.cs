using StreamMaster.Streams.Domain.Statistics;

namespace StreamMaster.Streams.Domain.Interfaces
{
    public interface IMetricsService
    {
        //double GetAverageLatency();
        //long GetBytesRead();
        //double GetKbps();
        StreamHandlerMetrics Metrics { get; }
        void RecordMetrics(int bytesRead, double latency);
    }
}