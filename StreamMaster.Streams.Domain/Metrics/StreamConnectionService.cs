using System.Collections.Concurrent;

using Microsoft.Extensions.Logging;

using StreamMaster.Domain.Configuration;

namespace StreamMaster.Streams.Domain.Metrics;

public class StreamConnectionService : IStreamConnectionService
{
    private readonly ConcurrentDictionary<string, StreamConnectionMetricManager> streamConnectionMetrics = new();
    public StreamConnectionService(ILogger<StreamConnectionService> logger)
    {
        string metricsFolder = BuildInfo.StreamHealthFolder;

        if (Directory.Exists(metricsFolder))
        {
            foreach (string file in Directory.GetFiles(metricsFolder, "*.json"))
            {
                try
                {
                    string id = Path.GetFileNameWithoutExtension(file);
                    StreamConnectionMetricData? metricData = StreamConnectionMetricSerializer.Load(file);
                    if (metricData is null)
                    {
                        logger.LogWarning("Failed to load metric file: {File}", file);
                        continue;
                    }
                    streamConnectionMetrics.TryAdd(id, new StreamConnectionMetricManager(id, metricData.StreamUrl));
                }
                catch (Exception ex)
                {
                    // Log and continue, as one corrupted file should not stop the loading process
                    logger.LogWarning(ex, "Failed to load metric file: {File}", file);
                }
            }
        }
    }

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
