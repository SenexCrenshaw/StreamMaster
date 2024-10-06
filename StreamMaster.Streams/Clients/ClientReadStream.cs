using StreamMaster.Streams.Domain;
using StreamMaster.Streams.Domain.Events;
using StreamMaster.Streams.Domain.Helpers;
using StreamMaster.Streams.Domain.Statistics;
using StreamMaster.Streams.Services;

using System.Diagnostics;
using System.Threading.Channels;

namespace StreamMaster.Streams.Clients;


public class ClientReadStream : Stream, IClientReadStream
{
    private readonly ILogger<ClientReadStream> logger;
    private readonly MetricsService MetricsService = new();

    public event EventHandler<StreamTimedOut> ClientStreamTimedOut;

    public StreamHandlerMetrics Metrics => MetricsService.Metrics;

    public TrackedChannel Channel { get; } = ChannelHelper.GetChannel(0);

    private bool IsCancelled { get; set; }
    public Guid Id { get; } = Guid.NewGuid();
    public override bool CanRead => true;
    public override bool CanSeek => false;
    public override bool CanWrite => false;
    public override long Length => throw new NotSupportedException();
    public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
    public override void Flush() { }


    private readonly string uniqueRequestId;


    private DateTime _lastReadTime;
    private readonly CancellationTokenSource? _monitorCts = null;

    public ClientReadStream(ILoggerFactory loggerFactory, string UniqueRequestId)
    {
        uniqueRequestId = UniqueRequestId;
        logger = loggerFactory.CreateLogger<ClientReadStream>();

        Setting? setting = SettingsHelper.GetSetting<Setting>(BuildInfo.SettingsFile);
        double ClientReadTimeOutSeconds = setting?.ClientReadTimeOutSeconds ?? 5;

        _lastReadTime = DateTime.UtcNow;

        // Only initialize monitoring if timeout is not zero
        if (ClientReadTimeOutSeconds > 0)
        {
            _monitorCts = new CancellationTokenSource();
            Task _monitorTask = Task.Run(async () =>
            {
                while (!_monitorCts.Token.IsCancellationRequested)
                {
                    await Task.Delay(1000, _monitorCts.Token); // Check every second
                    if (IsCancelled)
                    {
                        break;
                    }

                    TimeSpan timeSinceLastRead = DateTime.UtcNow - _lastReadTime;
                    if (timeSinceLastRead > TimeSpan.FromSeconds(ClientReadTimeOutSeconds))
                    {
                        // No read in last specified seconds
                        logger.LogWarning("No read in last {ClientReadTimeOutSeconds} seconds for UniqueRequestId: {UniqueRequestId}", ClientReadTimeOutSeconds, UniqueRequestId);
                        OnClientStreamTimedOut(new StreamTimedOut(uniqueRequestId, DateTime.UtcNow));

                        Cancel();
                        break;
                    }
                }
            }, _monitorCts.Token);
        }
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
            bytesRead = await ReadAsync(buffer.AsMemory(), cancellationToken);
        }
        catch (TaskCanceledException ex)
        {
            logger.LogInformation(ex, "Read Task ended for UniqueRequestId: {UniqueRequestId}", uniqueRequestId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error reading buffer for UniqueRequestId: {UniqueRequestId}", uniqueRequestId);
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
        ArgumentNullException.ThrowIfNull(buffer);

        if (offset < 0 || offset >= buffer.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(offset), "Offset is out of range.");
        }

        if (count < 0 || offset + count > buffer.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(count), "Count is out of range.");
        }

        byte[] dataToWrite = new byte[count];
        Array.Copy(buffer, offset, dataToWrite, 0, count);

        try
        {
            // Writing the data to the channel
            Channel.Write(dataToWrite);
        }
        catch (ChannelClosedException ex)
        {
            logger.LogError(ex, "Attempted to write to a closed channel for UniqueRequestId: {UniqueRequestId}", uniqueRequestId);
            throw new InvalidOperationException("The channel is closed and cannot accept writes.", ex);
        }
    }
    public void Cancel()
    {
        IsCancelled = true;
    }

    private bool _disposed = false;


    protected virtual void OnClientStreamTimedOut(StreamTimedOut e)
    {
        ClientStreamTimedOut?.Invoke(this, e);
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _monitorCts?.Cancel();
                _monitorCts?.Dispose();
            }

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
        _lastReadTime = DateTime.UtcNow;

        if (IsCancelled)
        {
            return 0;
        }

        Stopwatch stopWatch = Stopwatch.StartNew();

        int bytesRead = 0;

        try
        {
            //CancellationTokenSource timedToken = new(TimeSpan.FromSeconds(30));
            //using CancellationTokenSource linkedCts = CancellationTokenSource.CreateLinkedTokenSource(timedToken.Token, cancellationToken);

            byte[] read = await Channel.ReadAsync(cancellationToken);
            bytesRead = read.Length;

            if (bytesRead == 0)
            {
                return 0;
            }
            read[..bytesRead].CopyTo(buffer);

            if (cancellationToken.IsCancellationRequested)
            {
                logger.LogWarning("ReadAsync timedToken cancelled for UniqueRequestId: {UniqueRequestId}", uniqueRequestId);
                return bytesRead;
            }
        }
        catch (ChannelClosedException ex)
        {
            logger.LogInformation(ex, "ReadAsync closed for UniqueRequestId: {UniqueRequestId}", uniqueRequestId);
        }
        catch (TaskCanceledException ex)
        {
            logger.LogInformation(ex, "ReadAsync cancelled ended for UniqueRequestId: {UniqueRequestId}", uniqueRequestId);
            logger.LogInformation("ReadAsync {cancellationToken}", cancellationToken.IsCancellationRequested);
            bytesRead = 0;
        }
        catch (Exception ex)
        {
            //logger.LogInformation(ex, "ReadAsync {cancellationToken}", cancellationToken.IsCancellationRequested);
            bytesRead = 0;
        }
        finally
        {
            stopWatch.Stop();
            MetricsService.RecordMetrics(bytesRead, stopWatch.Elapsed.TotalMilliseconds);
            if (bytesRead == 0)
            {
                logger.LogDebug("Read 0 bytes for UniqueRequestId: {UniqueRequestId}", uniqueRequestId);
            }
        }

        return bytesRead;
    }
}