using StreamMasterApplication.Common.Interfaces;
using StreamMasterApplication.Common.Models;

namespace StreamMasterInfrastructure.MiddleWare;

public class RingBufferReadStream : Stream, IRingBufferReadStream
{
    private readonly CancellationToken _cancellationTokenSource;
    private readonly Guid _clientId;
    private Func<ICircularRingBuffer> _bufferDelegate;

    public RingBufferReadStream(Func<ICircularRingBuffer> bufferDelegate, StreamerConfiguration config)
    {
        _bufferDelegate = bufferDelegate ?? throw new ArgumentNullException(nameof(bufferDelegate));
        _clientId = config.ClientId;
        _cancellationTokenSource = config.CancellationToken;
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

        var ringBuffer = Buffer;
        int bytesRead = 0;

        while (bytesRead < count)
        {
            buffer[offset + bytesRead] = ringBuffer.Read(_clientId, CancellationToken.None).Result;
            bytesRead++;
        }

        return bytesRead;
    }

    public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        int bytesRead = 0;
        int availableBytes;

        var ringBuffer = Buffer;

        while (bytesRead < count)
        {
            if (cancellationToken.IsCancellationRequested || _cancellationTokenSource.IsCancellationRequested)
            {
                break;
            }

            availableBytes = ringBuffer.GetAvailableBytes(_clientId);

            if (availableBytes > 0)
            {
                int bytesToRead = Math.Min(count - bytesRead, availableBytes);
                bytesRead += await ringBuffer.ReadChunk(_clientId, buffer, offset, bytesToRead, cancellationToken);
                offset = (offset + bytesToRead) % buffer.Length;
            }
            else
            {
                await ringBuffer.WaitSemaphoreAsync(_clientId, cancellationToken);
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
