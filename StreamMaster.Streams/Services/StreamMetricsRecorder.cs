using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.Metrics;

using StreamMaster.Domain.Extensions;
using StreamMaster.Streams.Domain.Metrics;

namespace StreamMaster.Streams.Services;

/// <summary>
/// Service for managing and recording stream metrics.
/// </summary>
public class StreamMetricsRecorder : IStreamMetricsRecorder, IDisposable
{
    private readonly Meter _meter;
    private readonly Counter<long> _bytesReadCounter;
    private readonly Histogram<double> _kbpsHistogram;
    private readonly Histogram<double> _latencyHistogram;
    private readonly BPSMetrics _bpsStatistics = new();
    private readonly ConcurrentQueue<double> _latencies = new();
    private readonly DateTime _startTime = SMDT.UtcNow;

    private long _bytesRead;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="StreamMetricsRecorder"/> class.
    /// </summary>
    public StreamMetricsRecorder()
    {
        _meter = new Meter("StreamHandlerMetrics", "1.0");
        _bytesReadCounter = _meter.CreateCounter<long>("bytes_read");
        _kbpsHistogram = _meter.CreateHistogram<double>("kbps", "kbps");
        _latencyHistogram = _meter.CreateHistogram<double>("latency", "ms");
    }

    /// <inheritdoc/>
    public async Task<int> RecordMetricsAsync(Func<ValueTask<int>> action, CancellationToken cancellationToken)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        int bytesRead = 0;

        try
        {
            bytesRead = await action().ConfigureAwait(false);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            Debug.WriteLine(ex);
        }
        catch (Exception)
        {
        }
        finally
        {
            stopwatch.Stop();
            RecordMetrics(bytesRead, stopwatch.Elapsed.TotalMilliseconds);
        }

        return bytesRead;
    }

    /// <inheritdoc/>
    public void RecordMetrics(int bytesRead, double latency)
    {
        Interlocked.Add(ref _bytesRead, bytesRead);
        _bytesReadCounter.Add(bytesRead);
        _latencyHistogram.Record(latency);

        _bpsStatistics.AddBytesRead(bytesRead);
        double kbps = GetKbps();
        _kbpsHistogram.Record(kbps);

        _latencies.Enqueue(latency);
        while (_latencies.Count > 100) // Maintain a fixed queue size
        {
            _latencies.TryDequeue(out _);
        }
    }

    /// <inheritdoc/>
    public StreamHandlerMetrics Metrics => new()
    {
        BytesRead = GetBytesRead(),
        Kbps = GetKbps(),
        StartTime = _startTime,
        AverageLatency = GetAverageLatency()
    };

    /// <summary>
    /// Gets the total bytes read.
    /// </summary>
    public long GetBytesRead()
    {
        return Interlocked.Read(ref _bytesRead);
    }

    /// <summary>
    /// Gets the average latency recorded.
    /// </summary>
    public double GetAverageLatency()
    {
        return _latencies.IsEmpty ? 0 : _latencies.Average();
    }

    /// <summary>
    /// Gets the current kilobits per second (kbps).
    /// </summary>
    public double GetKbps()
    {
        return _bpsStatistics.BitsPerSecond / 1000.0;
    }

    /// <summary>
    /// Releases unmanaged and managed resources.
    /// </summary>
    /// <param name="disposing">Indicates whether the method is called from Dispose or finalizer.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _meter.Dispose();
            }
            _disposed = true;
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
