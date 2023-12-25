using Microsoft.Extensions.Logging;

using Prometheus;

using StreamMasterApplication.Common.Interfaces;

using StreamMasterDomain.Metrics;

using System.Collections.Concurrent;

namespace StreamMasterInfrastructure.VideoStreamManager.Buffers;

public sealed class ClientReadStream : Stream, IClientReadStream
{
    //private Func<ICircularRingBuffer> _bufferDelegate = bufferDelegate ?? throw new ArgumentNullException(nameof(bufferDelegate));
    private CancellationTokenSource _clientMasterToken;


    private readonly ILogger<ClientReadStream> logger;
    private readonly IClientStreamerConfiguration config;
    private Func<ICircularRingBuffer> _bufferDelegate;
    private readonly System.Timers.Timer bpsTimer;
    private long accumulatedBytesRead = 0;
    public ClientReadStream(Func<ICircularRingBuffer> bufferDelegate, ILogger<ClientReadStream> logger, IClientStreamerConfiguration config)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.config = config ?? throw new ArgumentNullException(nameof(config));
        ClientId = config.ClientId;
        _clientMasterToken = config.ClientMasterToken;
        this._bufferDelegate = bufferDelegate ?? throw new ArgumentNullException(nameof(bufferDelegate));
        bpsTimer = new System.Timers.Timer(10000)
        {
            AutoReset = true
        };
        bpsTimer.Elapsed += BpsTimerElapsed;
        bpsTimer.Start();

    }
    private readonly Gauge _bytesPerSecond = Metrics.CreateGauge(
    "client_read_stream_bytes_per_second",
    "Bytes per second read from the client stream.",
    new GaugeConfiguration
    {
        LabelNames = ["client_id", "circular_buffer_id", "video_stream_name"]
    });

    private readonly Histogram _bytesPerSecondHistogram = Metrics.CreateHistogram(
    "client_read_stream_bytes_per_second_histogram",
    "Histogram of bytes per second read from the client stream.",
    new HistogramConfiguration
    {
        // Adjusting buckets to cover from kbps to 100 Mbps
        // Note: 1 Mbps = 1,000,000 bytes per second
        // Example: Buckets at 1,000 (1 kbps), 10,000 (10 kbps), 100,000 (100 kbps), 1,000,000 (1 Mbps), ..., 100,000,000 (100 Mbps)
        Buckets = [1000, 10000, 100000, 1000000, 10000000, 20000000, 30000000, 40000000, 50000000, 60000000, 70000000, 80000000, 90000000, 100000000],
        LabelNames = ["client_id", "circular_buffer_id", "video_stream_name"]
    });

    private readonly Histogram _readDurationHistogram = Metrics.CreateHistogram(
            "client_read_stream_duration_milliseconds",
            "Duration of read operations in milliseconds.",
            new HistogramConfiguration
            {
                Buckets = Histogram.LinearBuckets(start: 0.001, width: 0.01, count: 100),
                LabelNames = ["client_id", "circular_buffer_id", "video_stream_name"]
            });

    private readonly Counter _bytesReadCounter = Metrics.CreateCounter(
            "client_read_stream_bytes_read_total",
            "Total number of bytes read.", new CounterConfiguration
            {
                LabelNames = ["client_id", "circular_buffer_id", "video_stream_name"]
            });

    private readonly Counter _readErrorsCounter = Metrics.CreateCounter(
            "client_read_stream_errors_total",
            "Total number of read errors."
            , new CounterConfiguration
            {
                LabelNames = ["client_id", "circular_buffer_id", "video_stream_name"]
            });
    private readonly Counter _readCancellationCounter = Metrics.CreateCounter(
            "client_read_stream_cancellations_total",
            "Total number of read cancellations.",
            new CounterConfiguration
            {
                LabelNames = ["client_id", "circular_buffer_id", "video_stream_name"]
            });


    private readonly ConcurrentDictionary<Guid, SemaphoreSlim> _bufferSwitchSemaphores = new();

    private readonly ConcurrentDictionary<Guid, PerformanceBpsMetrics> _performanceMetrics = new();

    private CancellationTokenSource _readCancel = new();

    private bool IsCancelled { get; set; }
    private Guid ClientId { get; set; }
    public ICircularRingBuffer Buffer => _bufferDelegate();
    public Guid Id { get; } = Guid.NewGuid();
    public override bool CanRead => true;
    public override bool CanSeek => false;
    public override bool CanWrite => false;
    public override long Length => throw new NotSupportedException();
    public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
    public override void Flush()
    { }

    private void BpsTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
    {
        // Calculate bps over the past 10 seconds
        double bps = accumulatedBytesRead / 10.0;

        // Record bps in the Histogram
        _bytesPerSecondHistogram.WithLabels(ClientId.ToString(), Buffer.Id.ToString(), Buffer.VideoStreamName).Observe(bps);

        // Reset the accumulated bytes
        accumulatedBytesRead = 0;

    }
    public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken)
    {
        if (IsCancelled)
        {
            return 0;
        }

        PerformanceBpsMetrics metrics = _performanceMetrics.GetOrAdd(ClientId, new PerformanceBpsMetrics(ClientId));

        if (_readCancel == null || _readCancel.IsCancellationRequested)
        {
            _readCancel = new CancellationTokenSource();
        }

        using CancellationTokenSource linkedCts = CancellationTokenSource.CreateLinkedTokenSource(_readCancel.Token, cancellationToken);

        linkedCts.CancelAfter(TimeSpan.FromSeconds(30));

        int bytesRead = 0;
        SemaphoreSlim semaphore = _bufferSwitchSemaphores.GetOrAdd(config.ClientId, new SemaphoreSlim(1, 1));

        try
        {
            // Use the ReadChunkMemory method to read data

            await semaphore.WaitAsync(cancellationToken);
            bytesRead = await Buffer.ReadChunkMemory(ClientId, buffer, linkedCts.Token);
            accumulatedBytesRead += bytesRead;
            metrics.RecordBytesProcessed(bytesRead);
        }
        catch (TaskCanceledException ex)
        {
            logger.LogInformation(ex, "ReadAsync cancelled ended for ClientId: {ClientId}", ClientId);
            bytesRead = 1;
        }
        finally
        {
            (double metricBytesRead, double bps, long elapsedMilliseconds) = metrics.GetBytesPerSecond();
            if (bps > -1)
            {
                //logger.LogDebug("Read {BytesRead} bytes for ClientId: {ClientId} {Bps} bps {elapsedMilliseconds}", metricBytesRead, ClientId, bps, elapsedMilliseconds);
                _bytesPerSecond.WithLabels(ClientId.ToString(), Buffer.Id.ToString(), Buffer.VideoStreamName).Set(bps);

                _bytesReadCounter.WithLabels(ClientId.ToString(), Buffer.Id.ToString(), Buffer.VideoStreamName).Inc(metricBytesRead);
                _bytesPerSecondHistogram.WithLabels(ClientId.ToString(), Buffer.Id.ToString(), Buffer.VideoStreamName).Observe(bps);
                _bytesPerSecond.WithLabels(ClientId.ToString(), Buffer.Id.ToString(), Buffer.VideoStreamName).Set(bps);
                _readDurationHistogram.WithLabels(ClientId.ToString(), Buffer.Id.ToString(), Buffer.VideoStreamName).Observe(elapsedMilliseconds / 1000);
            }

            if (bytesRead == 0)
            {
                logger.LogDebug("Read 0 bytes for ClientId: {ClientId}", ClientId);
                bytesRead = 1;
            }
            _ = semaphore.Release();
        }

        return bytesRead;
    }

    public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        if (IsCancelled)
        {
            return 0;
        }

        int bytesRead = 0;
        try
        {
            // Directly use ReadChunkMemory to read data into the buffer
            bytesRead = await Buffer.ReadChunkMemory(ClientId, buffer, cancellationToken);
        }
        catch (TaskCanceledException ex)
        {
            _readCancellationCounter.WithLabels(ClientId.ToString(), Buffer.Id.ToString(), Buffer.VideoStreamName).Inc();
            logger.LogInformation(ex, "Read Task ended for ClientId: {ClientId}", ClientId);
        }
        catch (Exception ex)
        {
            _readErrorsCounter.WithLabels(ClientId.ToString(), Buffer.Id.ToString(), Buffer.VideoStreamName).Inc();
            logger.LogError(ex, "Error reading buffer for ClientId: {ClientId}", ClientId);
        }

        return bytesRead;
    }



    public async Task SetBufferDelegate(Func<ICircularRingBuffer> bufferDelegate, IClientStreamerConfiguration config)
    {
        SemaphoreSlim semaphore = _bufferSwitchSemaphores.GetOrAdd(config.ClientId, new SemaphoreSlim(1, 1));

        _readCancel.Cancel();

        await semaphore.WaitAsync();
        try
        {

            ClientId = config.ClientId;
            _clientMasterToken = config.ClientMasterToken;
            _bufferDelegate = bufferDelegate;
        }
        finally
        {
            _ = semaphore.Release();
        }

        logger.LogInformation("Setting buffer delegate for Buffer.Id: {Id} Circular.Id: {Buffer.Id} {Name} ClientId: {ClientId}", Id, Buffer.Id, config.ChannelName, config.ClientId);
    }


    public override long Seek(long offset, SeekOrigin origin)
    {
        throw new NotSupportedException();
    }

    public override void SetLength(long value)
    {
        throw new NotSupportedException();
    }
    public override int Read(byte[] buffer, int offset, int count)
    {
        return 0;
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        throw new NotSupportedException();
    }

    public void Cancel()
    {
        IsCancelled = true;
    }
}