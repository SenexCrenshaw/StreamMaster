using System.Collections.Concurrent;

namespace StreamMaster.Streams.Domain.Metrics;

public class StreamConnectionService : IStreamConnectionService
{
    private readonly ConcurrentDictionary<string, StreamConnectionMetricManager> streamConnectionMetrics = new();

    public StreamConnectionMetricManager? Get(string id)
    {
        return streamConnectionMetrics.TryGetValue(id, out StreamConnectionMetricManager? metric) ? metric : null;
    }

    public List<StreamConnectionMetricData> GetMetrics()
    {
        return [.. streamConnectionMetrics.Values.Select(a => a.MetricData)];
    }

    public void Remove(string id)
    {
        streamConnectionMetrics.TryRemove(id, out _);
    }

    public StreamConnectionMetricManager GetOrAdd(string id, string streamUrl)
    {
        return streamConnectionMetrics.GetOrAdd(id, _ => new StreamConnectionMetricManager(id, streamUrl));
    }
}
