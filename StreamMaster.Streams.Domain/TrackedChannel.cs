using StreamMaster.Streams.Domain.Helpers;

using System.Runtime.CompilerServices;
using System.Threading.Channels;

namespace StreamMaster.Streams.Domain;
public class TrackedChannel
{
    public TrackedChannel(int boundCapacity = ChannelHelper.DefaultChannelCapacity, BoundedChannelFullMode? fullMode = BoundedChannelFullMode.Wait)
    {
        InnerChannel = boundCapacity > 0
            ? Channel.CreateBounded<byte[]>(new BoundedChannelOptions(boundCapacity) { SingleReader = true, SingleWriter = true, FullMode = fullMode ?? BoundedChannelFullMode.Wait })
            : Channel.CreateUnbounded<byte[]>(new UnboundedChannelOptions { SingleReader = true, SingleWriter = true });
        TotalBytesInBuffer = 0;
    }

    /// <summary>
    /// Attempts to mark the channel as complete, indicating no more items will be written.
    /// </summary>
    public void TryComplete()
    {
        InnerChannel.Writer.TryComplete();
    }

    /// <summary>
    /// Reads all available data from the channel until it is complete or canceled.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token to observe while reading.</param>
    /// <returns>An asynchronous enumerable of byte arrays from the channel.</returns>
    public async IAsyncEnumerable<byte[]> ReadAllAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await foreach (byte[]? item in InnerChannel.Reader.ReadAllAsync(cancellationToken).ConfigureAwait(false))
        {
            TotalBytesInBuffer -= item.Length;
            yield return item;
        }
    }

    /// <summary>
    /// Waits for data to be available for reading from the channel.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token to observe while waiting.</param>
    /// <returns>A task that completes when data is available for reading.</returns>
    public ValueTask<bool> WaitToReadAsync(CancellationToken cancellationToken = default)
    {
        return InnerChannel.Reader.WaitToReadAsync(cancellationToken);
    }

    /// <summary>
    /// Tries to read data from the channel.
    /// </summary>
    /// <param name="data">The data read from the channel, if available.</param>
    /// <returns>True if data was successfully read; otherwise, false.</returns>
    public bool TryRead(out byte[]? data)
    {
        return InnerChannel.Reader.TryRead(out data);
    }

    /// <summary>
    /// Gets the total number of bytes currently in the channel's buffer.
    /// </summary>
    public long TotalBytesInBuffer { get; private set; }

    /// <summary>
    /// Writes data to the channel and updates the total byte count (synchronous version).
    /// </summary>
    /// <param name="data">The byte array to be written to the channel.</param>
    public void Write(byte[] data)
    {
        if (!InnerChannel.Writer.TryWrite(data))
        {
            throw new ChannelClosedException("The channel is closed and cannot accept writes.");
        }
        TotalBytesInBuffer += data.Length;
    }

    /// <summary>
    /// Writes data to the channel and updates the total byte count (asynchronous version).
    /// </summary>
    /// <param name="data">The byte array to be written to the channel.</param>
    public async ValueTask<bool> WriteAsync(byte[] data, CancellationToken cancellationToken = default)
    {
        if (await InnerChannel.Writer.WaitToWriteAsync(cancellationToken).ConfigureAwait(false))
        {
            if (InnerChannel.Writer.TryWrite(data))
            {
                TotalBytesInBuffer += data.Length;
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Reads data from the channel and updates the total byte count.
    /// </summary>
    public async ValueTask<byte[]> ReadAsync(CancellationToken cancellationToken = default)
    {
        if (await InnerChannel.Reader.WaitToReadAsync(cancellationToken).ConfigureAwait(false))
        {
            if (InnerChannel.Reader.TryRead(out byte[]? data))
            {
                TotalBytesInBuffer -= data.Length;
                return data;
            }
        }

        throw new InvalidOperationException("Failed to read from the channel.");
    }

    /// <summary>
    /// Gets the underlying channel for custom read/write operations.
    /// </summary>
    public Channel<byte[]> InnerChannel { get; }
}