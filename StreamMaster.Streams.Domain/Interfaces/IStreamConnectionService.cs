using StreamMaster.Streams.Domain.Metrics;

namespace StreamMaster.Streams.Domain.Interfaces
{
    public interface IStreamConnectionService
    {
        StreamConnectionMetricManager GetOrAdd(string id, string streamUrl);
        StreamConnectionMetricManager? Get(string id);
        List<StreamConnectionMetricData> GetMetrics();
        void Remove(string id);
    }
}