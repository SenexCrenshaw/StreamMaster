namespace StreamMaster.Streams.Buffers;

public sealed partial class ClientReadStream : Stream, IClientReadStream
{
    private readonly ILogger<ClientReadStream> logger;
    //private readonly IClientStreamerConfiguration config;
    private readonly IClientStatisticsManager _clientStatisticsManager;
    public ClientReadStream(IClientStatisticsManager clientStatisticsManager, ILoggerFactory loggerFactory, ClientStreamerConfiguration config)
    {
        //this.config = config ?? throw new ArgumentNullException(nameof(config));

        logger = loggerFactory.CreateLogger<ClientReadStream>();

        UniqueRequestId = config.UniqueRequestId;

        _clientStatisticsManager = clientStatisticsManager;
        _clientStatisticsManager.RegisterClient(config);

        //BoundedChannelOptions options = new(100)
        //{
        //    SingleReader = true,
        //    SingleWriter = true
        //};
        Channel = new ByteTrackingChannel(loggerFactory.CreateLogger<ByteTrackingChannel>(), 200, 10);

        logger.LogInformation("Starting client read stream for UniqueRequestId: {UniqueRequestId}", UniqueRequestId);
    }

    public IByteTrackingChannel<byte[]> Channel { get; private set; }

    private bool IsCancelled { get; set; }

    private string UniqueRequestId { get; set; }
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
            bytesRead = await ReadAsync(buffer.AsMemory(), cancellationToken);
        }
        catch (TaskCanceledException ex)
        {
            logger.LogInformation(ex, "Read Task ended for UniqueRequestId: {UniqueRequestId}", UniqueRequestId);
        }
        catch (Exception ex)
        {
            //
            //if (setting.EnablePrometheus)
            //{
            //    _readErrorsCounter.WithLabels(UniqueRequestId.ToString(), StreamName).Inc();
            //}

            logger.LogError(ex, "Error reading buffer for UniqueRequestId: {UniqueRequestId}", UniqueRequestId);
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
                _clientStatisticsManager.UnRegisterClient(UniqueRequestId);
                //if (!string.IsNullOrEmpty(StreamName))
                //{
                //    _bitsPerSecond.RemoveLabelled(UniqueRequestId.ToString(), StreamName);
                //    _bytesReadCounter.RemoveLabelled(UniqueRequestId.ToString(), StreamName);
                //    _readErrorsCounter.RemoveLabelled(UniqueRequestId.ToString(), StreamName);
                //}
                //_bitsPerSecond.Unpublish();

                //_bitsPerSecond.Unpublish();

                //_readErrorsCounter.Unpublish();

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