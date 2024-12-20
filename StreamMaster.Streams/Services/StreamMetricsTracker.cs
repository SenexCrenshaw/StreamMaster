using System.Collections.Concurrent;
using System.Diagnostics.Metrics;

using StreamMaster.Domain.Extensions;
using StreamMaster.Streams.Domain.Statistics;

namespace StreamMaster.Streams.Services;

public class StreamMetricsTracker : IMetricsService, IDisposable
{
    private DateTime StartTime { get; } = SMDT.UtcNow;
    private readonly Meter _meter;
    private readonly Counter<long> _bytesReadCounter;
    private readonly Histogram<double> _kbpsHistogram;
    private readonly Histogram<double> _latencyHistogram;
    private readonly BPSStatistics _bpsStatistics = new();
    private readonly ConcurrentQueue<double> _latencies = new();
    private long _bytesRead;
    private bool disposedValue;

    public StreamMetricsTracker()
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
        //_bytesReadCounter.Add(bytesRead, new KeyValuePair<string, object>("streamId", "my-stream-id"));
        //        _latencyHistogram.Record(latency, new KeyValuePair<string, object>("streamId", "my-stream-id"));

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

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {

                _meter.Dispose();

            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            disposedValue = true;
        }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~StreamMetricsTracker()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
