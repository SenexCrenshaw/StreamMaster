using StreamMaster.Domain.Extensions;
using StreamMaster.Streams.Domain.Statistics;

using System.Collections.Concurrent;
using System.Diagnostics.Metrics;

namespace StreamMaster.Streams.Services;

public class MetricsService : IMetricsService
{
    private DateTime StartTime { get; } = SMDT.UtcNow;
    private readonly Meter _meter;
    private readonly Counter<long> _bytesReadCounter;
    private readonly Histogram<double> _kbpsHistogram;
    private readonly Histogram<double> _latencyHistogram;
    private readonly BPSStatistics _bpsStatistics = new();
    private readonly ConcurrentQueue<double> _latencies = new();
    private long _bytesRead;

    public MetricsService()
    {
        _meter = new Meter("StreamHandlerMetrics", "1.0");
        _bytesReadCounter = _meter.CreateCounter<long>("bytes_read");
        _kbpsHistogram = _meter.CreateHistogram<double>("kbps", "kbps");
        _latencyHistogram = _meter.CreateHistogram<double>("latency", "ms");
    }

    public void RecordMetrics(int bytesRead, double latency)
    {
        Interlocked.Add(ref _bytesRead, bytesRead);
        _bytesReadCounter.Add(bytesRead);
        _latencyHistogram.Record(latency);
        _bpsStatistics.AddBytesRead(bytesRead);
        double kbps = GetKbps();
        _kbpsHistogram.Record(kbps);

        // Maintain a fixed size for the latency queue
        _latencies.Enqueue(latency);

        while (_latencies.Count > 100) // Adjust this number as needed
        {
            _latencies.TryDequeue(out _);
        }
    }

    public StreamHandlerMetrics Metrics => new()
    {
        BytesRead = GetBytesRead(),
        Kbps = GetKbps(),
        StartTime = StartTime,
        AverageLatency = GetAverageLatency()
    };

    public long GetBytesRead()
    {
        return Interlocked.Read(ref _bytesRead);
    }

    public double GetAverageLatency()
    {
        return _latencies.IsEmpty ? 0 : _latencies.Average();
    }

    public double GetKbps()
    {
        return _bpsStatistics.BitsPerSecond / 1000.0;
    }
}
