using StreamMaster.Streams.Domain.Helpers;

using System.Diagnostics;
using System.Threading.Channels;

namespace StreamMaster.Streams.Buffers;

public sealed class ClientReadStream : Stream, IClientReadStream
{
    private readonly CancellationTokenSource _readCancel = new();
    private readonly ILogger<ClientReadStream> logger;

    public ClientReadStream(ILoggerFactory loggerFactory, string UniqueRequestId)
    {
        logger = loggerFactory.CreateLogger<ClientReadStream>();

        this.UniqueRequestId = UniqueRequestId;

        Channel = ChannelHelper.GetChannel();
        //Channel = System.Threading.Channels.Channel.CreateUnbounded<byte[]>();

        logger.LogInformation("Starting client read stream for UniqueRequestId: {UniqueRequestId}", UniqueRequestId);
    }

    public Channel<byte[]> Channel { get; }

    private bool IsCancelled { get; set; }

    private string UniqueRequestId { get; }
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
            }

            // Dispose unmanaged resources here if any

            _disposed = true;
        }

        // Call the base class implementation of Dispose
        base.Dispose(disposing);
    }

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
    public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken)
    {
        if (IsCancelled)
        {
            return 0;
        }

        Stopwatch stopWatch = Stopwatch.StartNew();

        int bytesRead = 0;

        try
        {
            CancellationTokenSource timedToken = new(TimeSpan.FromSeconds(30));
            using CancellationTokenSource linkedCts = CancellationTokenSource.CreateLinkedTokenSource(_readCancel.Token, timedToken.Token, cancellationToken);

            byte[] read = await Channel.Reader.ReadAsync(linkedCts.Token);
            bytesRead = read.Length;

            if (bytesRead == 0)
            {
                return 0;
            }
            read[..bytesRead].CopyTo(buffer);

            if (timedToken.IsCancellationRequested)
            {
                logger.LogWarning("ReadAsync timedToken cancelled for UniqueRequestId: {UniqueRequestId}", UniqueRequestId);
                return bytesRead;
            }
        }
        catch (ChannelClosedException ex)
        {
            logger.LogInformation(ex, "ReadAsync closed for UniqueRequestId: {UniqueRequestId}", UniqueRequestId);
        }
        catch (TaskCanceledException ex)
        {
            logger.LogInformation(ex, "ReadAsync cancelled ended for UniqueRequestId: {UniqueRequestId}", UniqueRequestId);
            logger.LogInformation("ReadAsync {_readCancel.Token}", _readCancel.Token.IsCancellationRequested);
            logger.LogInformation("ReadAsync {cancellationToken}", cancellationToken.IsCancellationRequested);
            bytesRead = 1;
        }
        finally
        {
            stopWatch.Stop();

            if (bytesRead == 0)
            {
                logger.LogDebug("Read 0 bytes for UniqueRequestId: {UniqueRequestId}", UniqueRequestId);
            }
        }

        return bytesRead;
    }
}