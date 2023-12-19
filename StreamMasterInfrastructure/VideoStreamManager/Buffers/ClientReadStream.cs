using Microsoft.Extensions.Logging;

using StreamMasterApplication.Common.Interfaces;

namespace StreamMasterInfrastructure.VideoStreamManager.Buffers;

public sealed class ClientReadStream(Func<ICircularRingBuffer> bufferDelegate, ILogger<ClientReadStream> logger, IClientStreamerConfiguration config) : Stream, IClientReadStream
{
    private Func<ICircularRingBuffer> _bufferDelegate = bufferDelegate ?? throw new ArgumentNullException(nameof(bufferDelegate));
    private CancellationTokenSource _clientMasterToken = config.ClientMasterToken;
    private readonly SemaphoreSlim semaphore = new(1);
    private CancellationTokenSource? _readCancel;

    private bool IsCancelled { get; set; }
    private Guid ClientId { get; set; } = config.ClientId;
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

        if (_readCancel == null)
        {
            _readCancel = new CancellationTokenSource();
        }
        else
        {
            if (_readCancel.IsCancellationRequested)
                _readCancel = new CancellationTokenSource();

        }

        int bytesRead = 0;
        using CancellationTokenSource linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _readCancel!.Token, _clientMasterToken.Token);
        try
        {
            await semaphore.WaitAsync(_readCancel.Token);
            while (!linkedCts.IsCancellationRequested && bytesRead < buffer.Length)
            {
                int availableBytes = Buffer.GetAvailableBytes(ClientId);
                if (availableBytes == 0)
                {
                    // Wait for data to become available
                    await Buffer.WaitForDataAvailability(ClientId, _readCancel.Token);
                    continue;
                }

                int bytesToRead = Math.Min(buffer.Length - bytesRead, availableBytes);

                bytesRead += await Buffer.ReadChunkMemory(ClientId, buffer.Slice(bytesRead, bytesToRead), _readCancel.Token);
            }
        }
        catch (TaskCanceledException ex)
        {
            _readCancel = new CancellationTokenSource();
            logger.LogInformation(ex, "Task cancelled for ClientId: {ClientId}", ClientId);

        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error reading buffer for ClientId: {ClientId}", ClientId);
        }
        finally
        {
            semaphore.Release();
        }

        return bytesRead;

    }

    public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        if (IsCancelled)
        {
            return 0;
        }

        if (_readCancel == null)
        {
            _readCancel = new CancellationTokenSource();
        }
        else
        {
            if (_readCancel.IsCancellationRequested)
                _readCancel = new CancellationTokenSource();
        }

        int bytesRead = 0;
        using CancellationTokenSource linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _readCancel.Token, _clientMasterToken.Token);
        try
        {
            await semaphore.WaitAsync(_readCancel.Token);
            while (!linkedCts.IsCancellationRequested && bytesRead < count)
            {
                int availableBytes = Buffer.GetAvailableBytes(ClientId);
                if (availableBytes == 0)
                {
                    // Wait for data to become available
                    await Buffer.WaitForDataAvailability(ClientId, _readCancel.Token);
                    continue;
                }

                int bytesToRead = Math.Min(count - bytesRead, availableBytes);

                // Create a Memory<byte> slice from the buffer
                Memory<byte> bufferSlice = new(buffer, offset + bytesRead, bytesToRead);
                bytesRead += await Buffer.ReadChunkMemory(ClientId, bufferSlice, _readCancel.Token);
            }
        }
        catch (TaskCanceledException ex)
        {
            _readCancel = new CancellationTokenSource();
            logger.LogInformation(ex, "Task cancelled for ClientId: {ClientId}", ClientId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error reading buffer for ClientId: {ClientId}", ClientId);
        }
        finally
        {
            semaphore.Release();
        }

        return bytesRead;
    }



    public async Task SetBufferDelegate(Func<ICircularRingBuffer> bufferDelegate, IClientStreamerConfiguration config)
    {
        if (_readCancel == null)
        {
            _readCancel = new CancellationTokenSource();
        }
        else
        {
            _readCancel.Cancel();
        }

        ClientId = config.ClientId;
        _clientMasterToken = config.ClientMasterToken;

        try
        {
            await semaphore.WaitAsync();
            _bufferDelegate = bufferDelegate ?? throw new ArgumentNullException(nameof(bufferDelegate));
        }
        finally
        {
            semaphore.Release();
        }

        logger.LogInformation("Setting buffer delegate for Buffer.Id: {Id} Circular.Id: {Buffer.Id} {Name} ClientId: {ClientId}", Id, Buffer.Id, config.ChannelName, config.ClientId);
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