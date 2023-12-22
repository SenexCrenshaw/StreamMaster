using Microsoft.Extensions.Logging;

using Prometheus;

using StreamMasterApplication.Common.Interfaces;

using System.Diagnostics;

namespace StreamMasterInfrastructure.VideoStreamManager.Buffers;

public sealed class ClientReadStream(Func<ICircularRingBuffer> bufferDelegate, ILogger<ClientReadStream> logger, IClientStreamerConfiguration config) : Stream, IClientReadStream
{
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
        Buckets = Histogram.LinearBuckets(0, 20000000, 11),
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

    private Func<ICircularRingBuffer> _bufferDelegate = bufferDelegate ?? throw new ArgumentNullException(nameof(bufferDelegate));
    private CancellationTokenSource _clientMasterToken = config.ClientMasterToken;
    //private readonly ConcurrentDictionary<Guid, SemaphoreSlim> _clientSemaphores = [];

    private CancellationTokenSource _readCancel = new();

    private bool IsCancelled { get; set; }
    private Guid ClientId { get; set; } = config.ClientId;
    public ICircularRingBuffer Buffer => _bufferDelegate();
    public Guid Id { get; } = Guid.NewGuid();
    public override bool CanRead => true;
    public override bool CanSeek => false;
    public override bool CanWrite => false;
    public override long Length => throw new NotSupportedException();
    public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
    public override void Flush()
    { }

    public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken)
    {
        if (IsCancelled)
        {
            return 0;
        }

        if (_readCancel == null || _readCancel.IsCancellationRequested)
        {
            _readCancel = new CancellationTokenSource();
        }

        using CancellationTokenSource linkedCts = CancellationTokenSource.CreateLinkedTokenSource(_readCancel.Token, cancellationToken);

        linkedCts.CancelAfter(TimeSpan.FromSeconds(30));

        Stopwatch stopwatch = new();
        stopwatch.Start();

        int bytesRead = 0;

        try
        {
            // Use the ReadChunkMemory method to read data
            bytesRead = await Buffer.ReadChunkMemory(ClientId, buffer, linkedCts.Token);
        }
        finally
        {
            stopwatch.Stop();
        }

        if (bytesRead > 0)
        {
            double seconds = stopwatch.Elapsed.TotalSeconds;
            double bps = bytesRead / seconds;

            _bytesReadCounter.WithLabels(ClientId.ToString(), Buffer.Id.ToString(), Buffer.VideoStreamName).Inc(bytesRead);
            _bytesPerSecondHistogram.WithLabels(ClientId.ToString(), Buffer.Id.ToString(), Buffer.VideoStreamName).Observe(bps);
            _bytesPerSecond.WithLabels(ClientId.ToString(), Buffer.Id.ToString(), Buffer.VideoStreamName).Set(bps);
            _readDurationHistogram.WithLabels(ClientId.ToString(), Buffer.Id.ToString(), Buffer.VideoStreamName).Observe(stopwatch.Elapsed.TotalMilliseconds);
        }
        else
        {
            logger.LogDebug("Read 0 bytes for ClientId: {ClientId}", ClientId);
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
        _readCancel.Cancel();
        //SemaphoreSlim semaphore = _clientSemaphores.GetOrAdd(config.ClientId, new SemaphoreSlim(1, 1));

        //await semaphore.WaitAsync();
        try
        {

            ClientId = config.ClientId;
            _clientMasterToken = config.ClientMasterToken;
            _bufferDelegate = bufferDelegate;
        }
        finally
        {
            //semaphore.Release();
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