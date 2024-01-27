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
    private readonly IStatisticsManager _statisticsManager;
    private long accumulatedBytesRead = 0;
    private bool _paused = false;

    public ClientReadStream(Func<ICircularRingBuffer> bufferDelegate, IStatisticsManager _statisticsManager, ILogger<ClientReadStream> logger, IClientStreamerConfiguration config)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.config = config ?? throw new ArgumentNullException(nameof(config));

        ClientId = config.ClientId;
        _clientMasterToken = config.ClientMasterToken;
        _bufferDelegate = bufferDelegate ?? throw new ArgumentNullException(nameof(bufferDelegate));

        this._statisticsManager = _statisticsManager;

        _lastReadIndex = bufferDelegate().GetNextReadIndex();

        //if (_lastReadIndex > StreamHandler.ChunkSize)
        //{
        //    _lastReadIndex -= StreamHandler.ChunkSize;
        //}

        _statisticsManager.RegisterClient(config);
        logger.LogInformation("Starting client read stream for ClientId: {ClientId} at index {_lastReadIndex} ", ClientId, _lastReadIndex);
    }

    public void SetLastIndex(long index)
    {
        //logger.LogInformation("Setting last index for ClientId: {ClientId} to {index} was {_lastReadIndex}", ClientId, index, _lastReadIndex);
        //_lastReadIndex = index;
    }

    private readonly ConcurrentDictionary<Guid, SemaphoreSlim> _bufferSwitchSemaphores = new();

    private CancellationTokenSource _readCancel = new();

    private long _lastReadIndex;
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

        //if (_readCancel == null || _readCancel.IsCancellationRequested)
        //{
        //    _readCancel = new CancellationTokenSource();
        //}

        //using CancellationTokenSource linkedCts = CancellationTokenSource.CreateLinkedTokenSource(_readCancel.Token, cancellationToken);

        //linkedCts.CancelAfter(TimeSpan.FromSeconds(30));

        int bytesRead = 0;

        SemaphoreSlim semaphore = _bufferSwitchSemaphores.GetOrAdd(config.ClientId, new SemaphoreSlim(1, 1));
        if (semaphore.CurrentCount == 0)
        {
            int aa = 1;
        }
        try
        {
            while (true)
            {
                await semaphore.WaitAsync(cancellationToken);
                if (_readCancel == null || _readCancel.IsCancellationRequested)
                {
                    _readCancel = new CancellationTokenSource();
                }

                using CancellationTokenSource linkedCts = CancellationTokenSource.CreateLinkedTokenSource(_readCancel.Token, cancellationToken);

                //maybe skip on pause loop
                bytesRead = await Buffer.ReadChunkMemory(_lastReadIndex, buffer, linkedCts.Token);
                if (bytesRead != 0)
                {
                    accumulatedBytesRead += bytesRead;
                    metrics.RecordBytesProcessed(bytesRead);
                }

                _lastReadIndex += bytesRead;
                _ = semaphore.Release();
                if (!_paused && !Buffer.IsPaused)
                {
                    break;
                }

            }
        }
        catch (TaskCanceledException ex)
        {
            logger.LogInformation(ex, "ReadAsync cancelled ended for ClientId: {ClientId}", ClientId);
            logger.LogInformation("ReadAsync {_readCancel.Token}", _readCancel.Token.IsCancellationRequested);
            logger.LogInformation("ReadAsync {cancellationToken}", cancellationToken.IsCancellationRequested);
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
                // bytesRead = 1;
            }

            if (semaphore.CurrentCount == 0)
            {
                _ = semaphore.Release();
            }

        }
        //_lastReadIndex += bytesRead;
        _statisticsManager.AddBytesRead(ClientId, bytesRead);
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
            bytesRead = await Buffer.ReadChunkMemory(_lastReadIndex, buffer, cancellationToken);
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
            _lastReadIndex = bufferDelegate().GetNextReadIndex();
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
                _statisticsManager.UnRegisterClient(ClientId);

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



    public void Pause()
    {
        _paused = true;
    }

    public void UnPause()
    {
        _paused = false;
    }

    // Finalizer in case Dispose wasn't called
    ~ClientReadStream()
    {
        Dispose(false);
    }
}