using System.Threading.Channels;

namespace StreamMaster.Streams.Buffers;
public class ByteTrackingChannel : IByteTrackingChannel<byte[]>
{
    private readonly Channel<byte[]> _channel;
    private long _currentByteSize;
    private readonly int _debugIntervalSeconds;
    private readonly CancellationTokenSource _debugCts;
    private readonly ILogger<ByteTrackingChannel> logger;

    public ByteTrackingChannel(ILogger<ByteTrackingChannel> logger, int itemCapacity, int debugIntervalSeconds = 0)
    {
        _channel = Channel.CreateBounded<byte[]>(itemCapacity);
        _currentByteSize = 0;
        _debugIntervalSeconds = debugIntervalSeconds;
        _debugCts = new CancellationTokenSource();
        this.logger = logger;
        if (_debugIntervalSeconds > 0)
        {
            StartDebugLogging(_debugCts.Token);
        }
    }

    public long CurrentByteSize => Interlocked.Read(ref _currentByteSize);

    public async Task<bool> WriteAsync(byte[] data, CancellationToken cancellationToken = default)
    {
        if (await _channel.Writer.WaitToWriteAsync(cancellationToken).ConfigureAwait(false))
        {
            Interlocked.Add(ref _currentByteSize, data.Length);
            await _channel.Writer.WriteAsync(data, cancellationToken).ConfigureAwait(false);
            return true;
        }
        return false;
    }

    public bool CanPeek => _channel.Reader.CanPeek;

    public async ValueTask<byte[]> ReadAsync(CancellationToken cancellationToken = default)
    {
        byte[] data = await _channel.Reader.ReadAsync(cancellationToken).ConfigureAwait(false);
        Interlocked.Add(ref _currentByteSize, -data.Length);
        return data;
    }

    public void Complete()
    {
        _channel.Writer.Complete();
        _debugCts.Cancel();
    }

    private void StartDebugLogging(CancellationToken cancellationToken)
    {
        Task.Run(async () =>
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(_debugIntervalSeconds), cancellationToken).ConfigureAwait(false);
                logger.LogDebug("Current byte size in channel: {CurrentByteSize} bytes", CurrentByteSize);
            }
        }, cancellationToken);
    }
}
