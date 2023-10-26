using Microsoft.Extensions.Logging;

using StreamMasterApplication.Common.Interfaces;

namespace StreamMasterInfrastructure.VideoStreamManager.Buffers;

public class RingBufferReadStream(Func<ICircularRingBuffer> bufferDelegate, ILogger<RingBufferReadStream> logger, IClientStreamerConfiguration config) : Stream, IRingBufferReadStream
{
    private Func<ICircularRingBuffer> _bufferDelegate = bufferDelegate ?? throw new ArgumentNullException(nameof(bufferDelegate));
    private CancellationTokenSource _clientMasterToken = config.ClientMasterToken;
    private Guid _clientId { get; set; } = config.ClientId;
    public ICircularRingBuffer Buffer => _bufferDelegate();
    public Guid Id { get; } = Guid.NewGuid();
    public override bool CanRead => true;

    public override bool CanSeek => false;

    public override bool CanWrite => false;
    public override long Length => throw new NotSupportedException();

    public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

    public override void Flush()
    { }

    public override int Read(byte[] buffer, int offset, int count)
    {
        if (_clientMasterToken.IsCancellationRequested)
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

    //private DateTime _lastLogTime = new();

    public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        int bytesRead = 0;
        int availableBytes;
        //DateTime currentTime;

        while (!cancellationToken.IsCancellationRequested && !_clientMasterToken.Token.IsCancellationRequested && bytesRead < buffer.Length)
        {
            //currentTime = DateTime.UtcNow;
            //if ((currentTime - _lastLogTime).TotalSeconds >= 2)
            //{
            //    logger.LogInformation("Read for Buffer.Id: {Id} Circular.Id: {Buffer.Id} ClientId: {ClientId} VideoStreamId: {VideoStreamId}", Id, Buffer.Id, config.ClientId, config.ChannelVideoStreamId);

            //    _lastLogTime = currentTime;
            //}
            availableBytes = Buffer.GetAvailableBytes(_clientId);

            if (availableBytes == 0)
            {
                await Buffer.WaitSemaphoreAsync(_clientId, cancellationToken);
                availableBytes = Buffer.GetAvailableBytes(_clientId);
            }

            int bytesToRead = Math.Min(buffer.Length - bytesRead, availableBytes);
            if (bytesToRead > 0)
            {
                bytesRead += await Buffer.ReadChunkMemory(_clientId, buffer.Slice(bytesRead, bytesToRead), cancellationToken);
            }
        }

        return bytesRead;
    }

    public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        int bytesRead = 0;
        int availableBytes;

        while (!cancellationToken.IsCancellationRequested && !_clientMasterToken.Token.IsCancellationRequested && bytesRead < count)
        {
            availableBytes = Buffer.GetAvailableBytes(_clientId);

            if (availableBytes == 0)
            {
                await Buffer.WaitSemaphoreAsync(_clientId, cancellationToken);
                availableBytes = Buffer.GetAvailableBytes(_clientId);
            }

            int bytesToRead = Math.Min(count - bytesRead, availableBytes);
            if (bytesToRead > 0)
            {
                bytesRead += await Buffer.ReadChunk(_clientId, buffer, offset, bytesToRead, cancellationToken);
                offset = (offset + bytesToRead) % buffer.Length;
            }
        }

        return bytesRead;
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        throw new NotSupportedException();
    }

    public void SetBufferDelegate(Func<ICircularRingBuffer> bufferDelegate, IClientStreamerConfiguration config)
    {
        _clientId = config.ClientId;
        _bufferDelegate = bufferDelegate ?? throw new ArgumentNullException(nameof(bufferDelegate));
        logger.LogInformation("Setting buffer delegate for Buffer.Id: {Id} Circular.Id: {Buffer.Id} ClientId: {ClientId} VideoStreamId: {VideoStreamId}", Id, Buffer.Id, config.ClientId, config.ChannelVideoStreamId);
        _clientMasterToken = config.ClientMasterToken;
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