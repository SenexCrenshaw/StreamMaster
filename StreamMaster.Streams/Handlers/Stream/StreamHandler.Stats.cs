using StreamMaster.Streams.Domain.Statistics;

using System.Collections.Concurrent;
using System.Diagnostics.Metrics;

namespace StreamMaster.Streams.Streams;

public sealed partial class StreamHandler
{
    private static readonly Meter Meter = new("StreamHandlerMetrics", "1.0");
    private static readonly Counter<long> BytesReadCounter = Meter.CreateCounter<long>("bytes_read");
    private static readonly Counter<long> BytesWrittenCounter = Meter.CreateCounter<long>("bytes_written");
    private static readonly Histogram<double> KbpsHistogram = Meter.CreateHistogram<double>("kbps", "kbps");
    private static readonly Histogram<double> LatencyHistogram = Meter.CreateHistogram<double>("latency", "ms");
    private static readonly Counter<int> ErrorCounter = Meter.CreateCounter<int>("errors");

    private readonly BPSStatistics _bpsStatistics = new();
    private long bytesRead;
    private long bytesWritten;

    private DateTime lastErrorTime;
    private readonly TimeSpan errorThreshold = TimeSpan.FromMinutes(5);
    private readonly DateTime startTime;

    private readonly ConcurrentQueue<double> latencies = new();
    private int errorCount;

    public long GetBytesRead()
    {
        return Interlocked.Read(ref bytesRead);
    }

    public long GetBytesWritten()
    {
        return Interlocked.Read(ref bytesWritten);
    }
    public int GetErrorCount()
    {
        return Volatile.Read(ref errorCount);
    }

    public double GetAverageLatency()
    {
        return latencies.IsEmpty ? 0 : latencies.Average();
    }

    public double GetKbps()
    {
        return _bpsStatistics.BitsPerSecond / 1000.0;
    }

    public DateTime GetStartTime()
    {
        return startTime;
    }

    private void SetMetrics(int bytesRead, int bytesWritten, double latency)
    {
        Interlocked.Add(ref this.bytesRead, bytesRead);
        Interlocked.Add(ref this.bytesWritten, bytesWritten);


        BytesReadCounter.Add(bytesRead);
        BytesWrittenCounter.Add(bytesWritten);
        LatencyHistogram.Record(latency);

        _bpsStatistics.AddBytesRead(bytesRead);
        _bpsStatistics.AddBytesWritten(bytesWritten);

        double kbps = GetKbps();
        KbpsHistogram.Record(kbps);

        // Add latency to the queue
        latencies.Enqueue(latency);
        // Maintain a fixed size for the latency queue
        while (latencies.Count > 100) // Adjust this number as needed
        {
            latencies.TryDequeue(out _);
        }
    }
    public bool IsHealthy()
    {
        // Check if the stream is active
        //if (!streamActive)
        //{
        //    logger.LogWarning("StreamHandler is unhealthy: stream is not active.");
        //    return false;
        //}

        // Check if there are clients connected
        //if (ChannelCount == 0)
        //{
        //    logger.LogWarning("StreamHandler is unhealthy: no clients connected.");
        //    return false;
        //}

        // Check for any recent errors
        if ((DateTime.UtcNow - lastErrorTime) < errorThreshold)
        {
            logger.LogWarning("StreamHandler is unhealthy: recent errors detected.");
            return false;
        }

        double kbps = GetKbps();
        if (kbps < 1000)
        {
            logger.LogWarning($"StreamHandler is unhealthy: slow kbps {kbps}.");
            return false;
        }

        // If all checks pass, the handler is healthy
        logger.LogInformation("StreamHandler is healthy.");
        return true;
    }
}