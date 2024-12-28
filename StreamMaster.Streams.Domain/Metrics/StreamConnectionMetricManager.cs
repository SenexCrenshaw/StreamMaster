using StreamMaster.Domain.Configuration;

namespace StreamMaster.Streams.Domain.Metrics;

public class StreamConnectionMetricManager
{
    public StreamConnectionMetricData MetricData { get; }

    private readonly Lock lockObj = new();

    private readonly string metricsFilePath;

    public StreamConnectionMetricManager(string id, string streamUrl)
    {
        metricsFilePath = Path.Combine(BuildInfo.StreamHealthFolder, $"{id}.json");
        MetricData = StreamConnectionMetricSerializer.Load(metricsFilePath, streamUrl);
    }

    public void RecordSuccessConnect()
    {
        lock (lockObj)
        {
            MetricData.LastSuccessConnect = DateTime.UtcNow;
            MetricData.RetryCount = 0;
        }
        SignalMetricsChanged();
    }

    public void RecordError()
    {
        lock (lockObj)
        {
            MetricData.LastErrorTime = DateTime.UtcNow;
        }
        SignalMetricsChanged();
    }

    public void IncrementRetryCount()
    {
        lock (lockObj)
        {
            MetricData.RetryCount++;
            MetricData.LastRetryTime = DateTime.UtcNow;
        }
        SignalMetricsChanged();
    }

    public int GetRetryCount()
    {
        lock (lockObj)
        {
            return MetricData.RetryCount;
        }
    }

    public void RecordConnectionAttempt()
    {
        lock (lockObj)
        {
            MetricData.TotalConnectionAttempts++;
        }
        SignalMetricsChanged();
    }

    private void SignalMetricsChanged()
    {
        lock (lockObj)
        {
            StreamConnectionMetricSerializer.SaveAsync(metricsFilePath, MetricData).ConfigureAwait(false);
        }
    }
}
