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

    public void RecordSuccessConnect(long connectionTime)
    {
        lock (lockObj)
        {
            MetricData.TimeToConnectMs = connectionTime;
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

    public void RecordConnectionAttempt(long connectionTime)
    {
        lock (lockObj)
        {
            MetricData.TotalConnectionAttempts++;
            if (connectionTime != 0)
            {
                RecordSuccessConnect(connectionTime);
            }
            else
            {
                RecordError();
            }
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
