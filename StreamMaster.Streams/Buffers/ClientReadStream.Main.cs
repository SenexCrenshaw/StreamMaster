using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMaster.Domain.Cache;

namespace StreamMaster.Streams.Buffers;

public sealed partial class ClientReadStream : Stream, IClientReadStream
{
    private CancellationTokenSource _clientMasterToken;
    private readonly ILogger<ClientReadStream> logger;
    private readonly IClientStreamerConfiguration config;
    private Func<ICircularRingBuffer> _bufferDelegate;
    private readonly IStatisticsManager _statisticsManager;
    private long accumulatedBytesRead = 0;
    private readonly IMemoryCache memoryCache;

    public ClientReadStream(Func<ICircularRingBuffer> bufferDelegate, IMemoryCache memoryCache, IStatisticsManager _statisticsManager, ILogger<ClientReadStream> logger, IClientStreamerConfiguration config)
    {
        this.memoryCache = memoryCache;
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.config = config ?? throw new ArgumentNullException(nameof(config));

        ClientId = config.ClientId;
        _clientMasterToken = config.ClientMasterToken;
        _bufferDelegate = bufferDelegate ?? throw new ArgumentNullException(nameof(bufferDelegate));

        this._statisticsManager = _statisticsManager;

        _lastReadIndex = bufferDelegate().GetNextReadIndex();

        _statisticsManager.RegisterClient(config);
        logger.LogInformation("Starting client read stream for ClientId: {ClientId} at index {_lastReadIndex} ", ClientId, _lastReadIndex);
    }

    private long _lastReadIndex;
    private bool IsCancelled { get; set; }

    public bool IsPaused { get; set; }
    private Guid ClientId { get; set; }
    public ICircularRingBuffer Buffer => _bufferDelegate();
    public Guid Id { get; } = Guid.NewGuid();
    public override bool CanRead => true;
    public override bool CanSeek => false;
    public override bool CanWrite => false;
    public override long Length => throw new NotSupportedException();
    public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
    public override void Flush() { }


    public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        if (IsCancelled)
        {
            return 0;
        }

        int bytesRead = 0;
        try
        {
            bytesRead = await Buffer.ReadChunkMemory(_lastReadIndex, buffer, cancellationToken);
        }
        catch (TaskCanceledException ex)
        {
            //if (_statsEnabled)
            //{
            //    _readCancellationCounter.WithLabels(ClientId.ToString(), Buffer.Id.ToString(), Buffer.VideoStreamName).Inc();
            //}

            logger.LogInformation(ex, "Read Task ended for ClientId: {ClientId}", ClientId);
        }
        catch (Exception ex)
        {
            Setting setting = memoryCache.GetSetting();
            if (setting.EnablePrometheus)
            {
                _readErrorsCounter.WithLabels(ClientId.ToString(), Buffer.VideoStreamName).Inc();
            }

            logger.LogError(ex, "Error reading buffer for ClientId: {ClientId}", ClientId);
        }

        return bytesRead;
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

                _bitsPerSecond.RemoveLabelled(ClientId.ToString(), Buffer.VideoStreamName);
                _bitsPerSecond.Unpublish();

                _bytesReadCounter.RemoveLabelled(ClientId.ToString(), Buffer.VideoStreamName);
                _bitsPerSecond.Unpublish();

                _readErrorsCounter.RemoveLabelled(ClientId.ToString(), Buffer.VideoStreamName);
                _readErrorsCounter.Unpublish();

                _bufferSwitchSemaphore.Dispose();

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
        IsPaused = true;
    }

    public void UnPause()
    {
        IsPaused = false;
    }

    // Finalizer in case Dispose wasn't called
    ~ClientReadStream()
    {
        Dispose(false);
    }
}