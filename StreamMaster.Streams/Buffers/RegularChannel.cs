using System.Threading.Channels;

namespace StreamMaster.Streams.Buffers;

public class RegularChannel<T> : ISMRegularChannel<T>
{
    private readonly Channel<T> _channel;

    public RegularChannel(int? itemSize = null)
    {
        if (itemSize > 0)
        {
            BoundedChannelOptions options = new(100)
            {
                SingleReader = true,
                SingleWriter = true
            };
            _channel = Channel.CreateBounded<T>(options);
        }
        else
        {
            UnboundedChannelOptions options = new()
            {
                SingleReader = true,
                SingleWriter = true
            };
            _channel = Channel.CreateUnbounded<T>(options);
        }
    }

    public async Task<bool> WriteAsync(T item, CancellationToken cancellationToken = default)
    {
        if (await _channel.Writer.WaitToWriteAsync(cancellationToken).ConfigureAwait(false))
        {
            await _channel.Writer.WriteAsync(item, cancellationToken).ConfigureAwait(false);
            return true;
        }
        return false;
    }
    public IAsyncEnumerable<T> ReadAllAsync(CancellationToken cancellationToken = default)
    {
        return _channel.Reader.ReadAllAsync(cancellationToken);
    }
    public bool CanPeek => _channel.Reader.CanPeek;

    public async ValueTask<T> ReadAsync(CancellationToken cancellationToken = default)
    {
        return await _channel.Reader.ReadAsync(cancellationToken).ConfigureAwait(false);
    }

    public void Complete()
    {
        _channel.Writer.Complete();
    }
}
