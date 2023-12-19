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

    private bool IsCancelled { get; set; }
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

    public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        if (IsCancelled)
        {
            return 0;
        }

        int bytesRead = 0;
        while (!cancellationToken.IsCancellationRequested && !_clientMasterToken.Token.IsCancellationRequested && bytesRead < buffer.Length)
        {
            int availableBytes = await Buffer.GetAvailableBytes(ClientId);
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
        if (bytesRead == 0)
        {
            var aaa = 1;
        }
        return bytesRead;
    }

    public void SetBufferDelegate(Func<ICircularRingBuffer> bufferDelegate, IClientStreamerConfiguration config)
    {
        ClientId = config.ClientId;
        _bufferDelegate = bufferDelegate ?? throw new ArgumentNullException(nameof(bufferDelegate));
        logger.LogInformation("Setting buffer delegate for Buffer.Id: {Id} Circular.Id: {Buffer.Id} {Name} ClientId: {ClientId}", Id, Buffer.Id, config.ChannelName, config.ClientId);
        _clientMasterToken = config.ClientMasterToken;
    }

    public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        return Task.FromResult(0);
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
        throw new NotSupportedException();
    }

    public void Cancel()
    {
        IsCancelled = true;
    }
}