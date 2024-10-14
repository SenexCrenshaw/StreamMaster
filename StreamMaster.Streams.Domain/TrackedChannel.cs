using StreamMaster.Streams.Domain.Helpers;

using System.Runtime.CompilerServices;
using System.Threading.Channels;

public class TrackedChannel : IDisposable
{
    private readonly bool _trackBytes;
    private bool _disposed;
    private readonly CancellationTokenSource _cancellationTokenSource = new();

    public TrackedChannel(int boundCapacity = ChannelHelper.DefaultChannelCapacity, BoundedChannelFullMode? fullMode = BoundedChannelFullMode.Wait, bool trackBytes = false)
    {
        InnerChannel = boundCapacity > 0
            ? Channel.CreateBounded<byte[]>(new BoundedChannelOptions(boundCapacity) { SingleReader = true, SingleWriter = true, FullMode = fullMode ?? BoundedChannelFullMode.Wait })
            : Channel.CreateUnbounded<byte[]>(new UnboundedChannelOptions { SingleReader = true, SingleWriter = true });

        _trackBytes = trackBytes;
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
        await foreach (byte[]? item in InnerChannel.Reader.ReadAllAsync(_cancellationTokenSource.Token).ConfigureAwait(false))
        {
            if (_trackBytes)
            {
                TotalBytesInBuffer -= item.Length;
            }
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
        return InnerChannel.Reader.WaitToReadAsync(_cancellationTokenSource.Token);
    }

    /// <summary>
    /// Tries to read data from the channel.
    /// </summary>
    /// <param name="data">The data read from the channel, if available.</param>
    /// <returns>True if data was successfully read; otherwise, false.</returns>
    public bool TryRead(out byte[]? data)
    {
        bool result = InnerChannel.Reader.TryRead(out data);
        if (result && _trackBytes && data != null)
        {
            TotalBytesInBuffer -= data.Length;
        }
        return result;
    }

    /// <summary>
    /// Gets the total number of bytes currently in the channel's buffer (only if tracking is enabled).
    /// </summary>
    public long TotalBytesInBuffer { get; private set; }

    /// <summary>
    /// Writes data to the channel and updates the total byte count (synchronous version).
    /// </summary>
    /// <param name="data">The byte array to be written to the channel.</param>
    public bool Write(byte[] data)
    {
        if (!InnerChannel.Writer.TryWrite(data))
        {
            throw new ChannelClosedException("The channel is closed and cannot accept writes.");
        }

        if (_trackBytes)
        {
            TotalBytesInBuffer += data.Length;
        }
        return true;
    }

    /// <summary>
    /// Writes data to the channel and updates the total byte count (asynchronous version).
    /// </summary>
    /// <param name="data">The byte array to be written to the channel.</param>
    public async ValueTask<bool> WriteAsync(byte[] data, CancellationToken cancellationToken = default)
    {
        if (await InnerChannel.Writer.WaitToWriteAsync(_cancellationTokenSource.Token).ConfigureAwait(false))
        {
            if (InnerChannel.Writer.TryWrite(data))
            {
                if (_trackBytes)
                {
                    TotalBytesInBuffer += data.Length;
                }
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
        if (await InnerChannel.Reader.WaitToReadAsync(_cancellationTokenSource.Token).ConfigureAwait(false))
        {
            if (InnerChannel.Reader.TryRead(out byte[]? data))
            {
                if (_trackBytes && data != null)
                {
                    TotalBytesInBuffer -= data.Length;
                }
                return data;
            }
        }

        throw new InvalidOperationException("Failed to read from the channel.");
    }

    /// <summary>
    /// Gets the underlying channel for custom read/write operations.
    /// </summary>
    public Channel<byte[]> InnerChannel { get; }

    /// <summary>
    /// Disposes the channel, ensuring proper resource cleanup.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Protected Dispose method to handle resource cleanup.
    /// </summary>
    /// <param name="disposing">Whether the method is called from Dispose() or the finalizer.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // Signal to cancel any pending operations
                _cancellationTokenSource.Cancel();

                // Mark the channel as complete to stop further writes
                TryComplete();

                // Optionally: Drain the channel to ensure no items remain (optional based on your usage)
                DrainChannel();

                // Cleanup the cancellation token source
                _cancellationTokenSource.Dispose();
            }

            _disposed = true;
        }
    }

    private void DrainChannel()
    {
        // Drain the channel if there are any remaining unread items (optional, depending on your use case)
        while (InnerChannel.Reader.TryRead(out _))
        {
            // Just reading and discarding the remaining items
        }
    }

    ~TrackedChannel()
    {
        Dispose(false);
    }
}
