using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using System.Threading.Channels;

namespace StreamMaster.Streams.Buffers;

public sealed partial class ClientReadStream : Stream, IClientReadStream
{
    private readonly CancellationTokenSource _clientMasterToken;
    private readonly ILogger<ClientReadStream> logger;
    private readonly IClientStreamerConfiguration config;
    private readonly IStatisticsManager _statisticsManager;
    private readonly IMemoryCache memoryCache;

    public string VideoStreamName { get; set; }

    public ClientReadStream(IMemoryCache memoryCache, IStatisticsManager _statisticsManager, ILoggerFactory loggerFactory, IClientStreamerConfiguration config)
    {
        this.memoryCache = memoryCache;

        this.config = config ?? throw new ArgumentNullException(nameof(config));

        logger = loggerFactory.CreateLogger<ClientReadStream>();
        _readLogger = loggerFactory.CreateLogger<ReadsLogger>();

        ClientId = config.ClientId;
        _clientMasterToken = config.ClientMasterToken;

        this._statisticsManager = _statisticsManager;

        _statisticsManager.RegisterClient(config);
        UnboundedChannelOptions options = new()
        {
            SingleReader = true,
            SingleWriter = true
        };
        ReadChannel = Channel.CreateUnbounded<byte[]>(options);

        logger.LogInformation("Starting client read stream for ClientId: {ClientId}", ClientId);
    }

    public Channel<byte[]> ReadChannel { get; private set; }

    private bool IsCancelled { get; set; }

    private Guid ClientId { get; set; }
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


            logger.LogInformation(ex, "Read Task ended for ClientId: {ClientId}", ClientId);
        }
        catch (Exception ex)
        {
            //Setting setting = memoryCache.GetSetting();
            //if (setting.EnablePrometheus)
            //{
            //    _readErrorsCounter.WithLabels(ClientId.ToString(), VideoStreamName).Inc();
            //}

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
                if (!string.IsNullOrEmpty(VideoStreamName))
                {
                    _bitsPerSecond.RemoveLabelled(ClientId.ToString(), VideoStreamName);
                    _bytesReadCounter.RemoveLabelled(ClientId.ToString(), VideoStreamName);
                    _readErrorsCounter.RemoveLabelled(ClientId.ToString(), VideoStreamName);
                }
                _bitsPerSecond.Unpublish();

                _bitsPerSecond.Unpublish();

                _readErrorsCounter.Unpublish();

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