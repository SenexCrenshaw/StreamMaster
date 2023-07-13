using StreamMasterApplication.Common.Interfaces;

namespace StreamMasterInfrastructure.MiddleWare;

public class RingBufferReadStream : Stream, IRingBufferReadStream
{
    private readonly CancellationToken _cancellationTokenSource;
    private readonly Guid _clientId;
    private Func<ICircularRingBuffer> _bufferDelegate;

    public RingBufferReadStream(Func<CircularRingBuffer> bufferDelegate, Guid clientId, CancellationToken cancellationTokenSource)
    {
        _bufferDelegate = bufferDelegate;
        _clientId = clientId;
        _cancellationTokenSource = cancellationTokenSource;
    }

    public ICircularRingBuffer Buffer => _bufferDelegate();

    public override bool CanRead => true;

    public override bool CanSeek => false;

    public override bool CanWrite => false;

    public override long Length => throw new NotSupportedException();

    public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

    public override void Flush()
    { }

    public override int Read(byte[] buffer, int offset, int count)
    {
        if (_cancellationTokenSource.IsCancellationRequested)
        {
            return 0;
        }

        int bytesRead = 0;

        while (bytesRead < count)
        {
            buffer[offset + bytesRead] = Buffer.Read(_clientId, CancellationToken.None);
            bytesRead++;
        }

        return bytesRead;
    }

    public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        int bytesRead = 0;
        int availableBytes;

        while (bytesRead < count && !cancellationToken.IsCancellationRequested && !_cancellationTokenSource.IsCancellationRequested)
        {
            availableBytes = Buffer.GetAvailableBytes(_clientId);

            if (availableBytes > 0)
            {
                int bytesToRead = Math.Min(count - bytesRead, availableBytes);
                bytesRead += Buffer.ReadChunk(_clientId, buffer, offset, bytesToRead, cancellationToken);
                offset = (offset + bytesToRead) % buffer.Length;
            }
            else
            {
                // Wait for new data or delay
                if (Buffer.NewDataAvailable(_clientId))
                {
                    await Buffer.WaitSemaphoreAsync(_clientId, cancellationToken);
                }
                else
                {
                    await Task.Delay(5, cancellationToken);
                }
            }
        }

        return bytesRead;
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        throw new NotSupportedException();
    }

    public void SetBufferDelegate(Func<ICircularRingBuffer> newBufferDelegate)
    {
        _bufferDelegate = newBufferDelegate;
    }

    public override void SetLength(long value)
    {
        throw new NotSupportedException();
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        throw new NotSupportedException();
    }
}
