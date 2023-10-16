using StreamMasterApplication.Common.Interfaces;
using StreamMasterApplication.Common.Models;

namespace StreamMasterInfrastructure.VideoStreamManager;

public class RingBufferReadStream(Func<ICircularRingBuffer> bufferDelegate, ClientStreamerConfiguration config) : Stream, IRingBufferReadStream
{
    private Func<ICircularRingBuffer> _bufferDelegate = bufferDelegate ?? throw new ArgumentNullException(nameof(bufferDelegate));
    private CancellationToken _cancellationSource = config.ClientHTTPRequestCancellationToken;
    private CancellationTokenSource _cancellationTokenSource = config.ClientCancellationTokenSource;
    private Guid _clientId = config.ClientId;

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
            buffer[offset + bytesRead] = Buffer.Read(_clientId, CancellationToken.None).Result;
            bytesRead++;
        }

        return bytesRead;
    }


    public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        int bytesRead = 0;
        int availableBytes;

        while (bytesRead < count)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _cancellationSource.ThrowIfCancellationRequested();
            _cancellationTokenSource.Token.ThrowIfCancellationRequested();

            availableBytes = Buffer.GetAvailableBytes(_clientId);

            if (availableBytes > 0)
            {
                int bytesToRead = Math.Min(count - bytesRead, availableBytes);
                bytesRead += await Buffer.ReadChunk(_clientId, buffer, offset, bytesToRead, cancellationToken);
                offset = (offset + bytesToRead) % buffer.Length;
            }
            else
            {
                await Buffer.WaitSemaphoreAsync(_clientId, cancellationToken);
            }
        }

        return bytesRead;
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        throw new NotSupportedException();
    }

    public void SetBufferDelegate(Func<ICircularRingBuffer> bufferDelegate, ClientStreamerConfiguration config)
    {
        _bufferDelegate = bufferDelegate ?? throw new ArgumentNullException(nameof(bufferDelegate));
        _clientId = config.ClientId;
        _cancellationSource = config.ClientHTTPRequestCancellationToken;
        _cancellationTokenSource = config.ClientCancellationTokenSource;
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
