using Microsoft.Extensions.Logging;

using System.Collections.Concurrent;
using System.Diagnostics;

namespace StreamMaster.Streams.Buffers;

public sealed partial class ClientReadStream : Stream, IClientReadStream
{
    private CancellationTokenSource _clientMasterToken;
    private readonly ILogger<ClientReadStream> logger;
    private readonly IClientStreamerConfiguration config;
    private Func<ICircularRingBuffer> _bufferDelegate;

    private long accumulatedBytesRead = 0;
    public ClientReadStream(Func<ICircularRingBuffer> bufferDelegate, ILogger<ClientReadStream> logger, IClientStreamerConfiguration config)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.config = config ?? throw new ArgumentNullException(nameof(config));
        ClientId = config.ClientId;
        _clientMasterToken = config.ClientMasterToken;
        _bufferDelegate = bufferDelegate ?? throw new ArgumentNullException(nameof(bufferDelegate));

    }

    private readonly ConcurrentDictionary<Guid, SemaphoreSlim> _bufferSwitchSemaphores = new();

    //private readonly ConcurrentDictionary<Guid, PerformanceBpsMetrics> _performanceMetrics = new();

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

    public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken)
    {
        if (IsCancelled)
        {
            return 0;
        }
        Stopwatch stopWatch = Stopwatch.StartNew();

        if (_readCancel == null || _readCancel.IsCancellationRequested)
        {
            _readCancel = new CancellationTokenSource();
        }

        using CancellationTokenSource linkedCts = CancellationTokenSource.CreateLinkedTokenSource(_readCancel.Token, cancellationToken);

        linkedCts.CancelAfter(TimeSpan.FromSeconds(30));

        int bytesRead = 0;

        SemaphoreSlim semaphore = _bufferSwitchSemaphores.GetOrAdd(config.ClientId, new SemaphoreSlim(1, 1));
        if (semaphore.CurrentCount == 0)
        {
            int aa = 1;
        }
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
            stopWatch.Stop();
            double bps = metrics.GetBitsPerSecond();
            if (bps > -1)
            {
                _bitsPerSecond.WithLabels(ClientId.ToString(), Buffer.Id.ToString(), Buffer.VideoStreamName).Set(bps);

                _bytesReadCounter.WithLabels(ClientId.ToString(), Buffer.Id.ToString(), Buffer.VideoStreamName).Inc(bytesRead);

                _readDuration.WithLabels(ClientId.ToString(), Buffer.Id.ToString(), Buffer.VideoStreamName).Set(stopWatch.ElapsedMilliseconds);
            }

            if (bytesRead == 0)
            {
                logger.LogDebug("Read 0 bytes for ClientId: {ClientId}", ClientId);
                bytesRead = 1;
            }

            if (semaphore.CurrentCount == 0)
            {
                _ = semaphore.Release();
            }

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
    private bool _disposed = false; // To track whether Dispose has been called

    protected override void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _bitsPerSecond.RemoveLabelled(ClientId.ToString(), Buffer.Id.ToString(), Buffer.VideoStreamName);
                _readDuration.RemoveLabelled(ClientId.ToString(), Buffer.Id.ToString(), Buffer.VideoStreamName);
                _bytesReadCounter.RemoveLabelled(ClientId.ToString(), Buffer.Id.ToString(), Buffer.VideoStreamName);
                _readErrorsCounter.RemoveLabelled(ClientId.ToString(), Buffer.Id.ToString(), Buffer.VideoStreamName);
                _readCancellationCounter.RemoveLabelled(ClientId.ToString(), Buffer.Id.ToString(), Buffer.VideoStreamName);
                _bufferSwitchSemaphores.Clear();

            }

            // Dispose unmanaged resources here if any

            _disposed = true;
        }

        // Call the base class implementation of Dispose
        base.Dispose(disposing);
    }

    // Public implementation of Dispose pattern callable by consumers
    public new void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    // Finalizer in case Dispose wasn't called
    ~ClientReadStream()
    {
        Dispose(false);
    }
}