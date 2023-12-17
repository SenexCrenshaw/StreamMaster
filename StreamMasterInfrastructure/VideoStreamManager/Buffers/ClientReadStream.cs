using Microsoft.Extensions.Logging;

using StreamMasterApplication.Common.Interfaces;

namespace StreamMasterInfrastructure.VideoStreamManager.Buffers;

public sealed class ClientReadStream : Stream, IClientReadStream
{
    private Func<ICircularRingBuffer> _bufferDelegate;
    private CancellationTokenSource _clientMasterToken;
    private readonly ILogger<ClientReadStream> logger;

    public ClientReadStream(Func<ICircularRingBuffer> bufferDelegate, ILogger<ClientReadStream> logger, IClientStreamerConfiguration config)
    {
        this.logger = logger;
        _bufferDelegate = bufferDelegate ?? throw new ArgumentNullException(nameof(bufferDelegate));
        _clientMasterToken = config.ClientMasterToken;
        ClientId = config.ClientId;
    }

    private Guid ClientId { get; set; }
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
            int availableBytes = Buffer.GetAvailableBytes(ClientId);
            if (availableBytes == 0)
            {
                // Instead of a busy wait, consider waiting for a signal (like in ReadAsync)
                // This is a simplistic approach; adapt as needed for your scenario
                Thread.Sleep(100);
                continue;
            }

            int remainingBufferSpace = count - bytesRead;
            int bytesToRead = Math.Min(remainingBufferSpace, availableBytes);

            for (int i = 0; i < bytesToRead; i++)
            {
                buffer[offset + bytesRead] = Buffer.Read(ClientId, CancellationToken.None).Result;
                bytesRead++;
            }
        }

        return bytesRead;
    }


    public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        int bytesRead = 0;
        while (!cancellationToken.IsCancellationRequested && !_clientMasterToken.Token.IsCancellationRequested && bytesRead < buffer.Length)
        {
            int availableBytes = Buffer.GetAvailableBytes(ClientId);
            if (availableBytes == 0)
            {
                // Wait for data to become available
                await Buffer.WaitForDataAvailability(ClientId, cancellationToken);
                continue;
            }

            int bytesToRead = Math.Min(buffer.Length - bytesRead, availableBytes);

            bytesRead += await Buffer.ReadChunkMemory(ClientId, buffer.Slice(bytesRead, bytesToRead), cancellationToken);

            // Check for cancellation
            cancellationToken.ThrowIfCancellationRequested();

        }

        return bytesRead;
    }


    public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        int bytesRead = 0;
        while (!cancellationToken.IsCancellationRequested && bytesRead < count)
        {
            int availableBytes = Buffer.GetAvailableBytes(ClientId);
            if (availableBytes == 0)
            {
                // Wait for data to become available
                await Buffer.WaitForDataAvailability(ClientId, cancellationToken);
                continue;
            }

            // Calculate the maximum number of bytes that can be read
            int remainingBufferSpace = count - bytesRead;
            int bytesToRead = Math.Min(remainingBufferSpace, availableBytes);

            bytesRead += await Buffer.ReadChunk(ClientId, buffer, offset, bytesToRead, cancellationToken);
            offset += bytesToRead;

            // Check for cancellation
            cancellationToken.ThrowIfCancellationRequested();
        }

        return bytesRead;
    }


    public override long Seek(long offset, SeekOrigin origin)
    {
        throw new NotSupportedException();
    }

    public void SetBufferDelegate(Func<ICircularRingBuffer> bufferDelegate, IClientStreamerConfiguration config)
    {
        ClientId = config.ClientId;
        _bufferDelegate = bufferDelegate ?? throw new ArgumentNullException(nameof(bufferDelegate));
        logger.LogInformation("Setting buffer delegate for Buffer.Id: {Id} Circular.Id: {Buffer.Id} {Name} ClientId: {ClientId}", Id, Buffer.Id, config.ChannelName, config.ClientId);
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