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

        StreamConnectionMetricData? metricData = StreamConnectionMetricSerializer.Load(metricsFilePath, streamUrl);
        if (metricData is not null)
        {
            MetricData = metricData;
        }
        else
        {
            MetricData = new StreamConnectionMetricData(streamUrl);
            SignalMetricsChanged();
        }
    }

    public void RecordSuccessConnect()
    {
        lock (lockObj)
        {
            MetricData.LastSuccessConnectTime = DateTime.UtcNow;
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

    public void SetLastConnectionAttemptTime()
    {
        lock (lockObj)
        {
            MetricData.LastConnectionAttemptTime = DateTime.UtcNow;
        }
        SignalMetricsChanged();
    }

    public void IncrementRetryCount()
    {
        lock (lockObj)
        {
            MetricData.RetryCount++;
            MetricData.LastConnectionAttemptTime = DateTime.UtcNow;
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
