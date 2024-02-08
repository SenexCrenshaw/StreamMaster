using Microsoft.Extensions.Logging;

using System.Diagnostics;

namespace StreamMaster.Streams.Buffers;

public sealed partial class ClientReadStream : Stream, IClientReadStream
{
    private readonly SemaphoreSlim _bufferSwitchSemaphore = new(1, 1);
    private CancellationTokenSource _readCancel = new();

    public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken)
    {
        if (IsCancelled)
        {
            return 0;
        }
        Stopwatch stopWatch = Stopwatch.StartNew();

        int bytesRead = 0;
        try
        {
            while (true)
            {
                await _bufferSwitchSemaphore.WaitAsync(_readCancel.Token);

                CancellationTokenSource timedToken = new(TimeSpan.FromSeconds(30));

                using CancellationTokenSource readLinked = CancellationTokenSource.CreateLinkedTokenSource(_readCancel.Token, timedToken.Token);


                if (timedToken.IsCancellationRequested)
                {
                    logger.LogWarning("ReadAsync timedToken cancelled for ClientId: {ClientId}", ClientId);
                    _bufferSwitchSemaphore.Release();
                    break;
                }

                using CancellationTokenSource linkedCts = CancellationTokenSource.CreateLinkedTokenSource(_readCancel.Token, timedToken.Token, cancellationToken);

                try
                {
                    bytesRead = !linkedCts.IsCancellationRequested ? await Buffer.ReadChunkMemory(_lastReadIndex, buffer, linkedCts.Token) : 0;
                }
                catch (TaskCanceledException)
                {
                    logger.LogWarning("ReadAsync cancellationToken {cancellationToken}", cancellationToken.IsCancellationRequested);
                    logger.LogWarning("ReadAsync _readCancel {_readCancel}", _readCancel.IsCancellationRequested);

                }
                finally
                {
                    linkedCts.Dispose();
                    try
                    {
                        _bufferSwitchSemaphore.Release();
                    }
                    catch (ObjectDisposedException)
                    {
                    }

                }

                if (timedToken.IsCancellationRequested)
                {
                    logger.LogWarning("ReadAsync timedToken cancelled for ClientId: {ClientId}", ClientId);
                    break;
                }

                if (bytesRead != 0)
                {
                    accumulatedBytesRead += bytesRead;
                    metrics.RecordBytesProcessed(bytesRead);
                    _lastReadIndex += bytesRead;
                }

                if (!IsPaused && !Buffer.IsPaused)
                {
                    break;
                }

                await Task.Delay(5, cancellationToken);
            }
        }
        catch (TaskCanceledException ex)
        {
            logger.LogInformation(ex, "ReadAsync cancelled ended for ClientId: {ClientId}", ClientId);
            logger.LogInformation("ReadAsync {_readCancel.Token}", _readCancel.Token.IsCancellationRequested);
            logger.LogInformation("ReadAsync {cancellationToken}", cancellationToken.IsCancellationRequested);
            bytesRead = 1;
        }
        finally
        {
            stopWatch.Stop();

            SetMetrics(bytesRead);

            if (bytesRead == 0)
            {
                logger.LogDebug("Read 0 bytes for ClientId: {ClientId}", ClientId);
            }
        }

        if (_bufferSwitchSemaphore.CurrentCount == 0)
        {
            _bufferSwitchSemaphore.Release();
        }

        _statisticsManager.AddBytesRead(ClientId, bytesRead);
        return bytesRead;
    }

    public async Task SetBufferDelegate(Func<ICircularRingBuffer> bufferDelegate, IClientStreamerConfiguration config)
    {
        if (_readCancel.IsCancellationRequested)
        {
            logger.LogError("SetBufferDelegate called on a cancelled stream for ClientId: {ClientId}", ClientId);
            _readCancel.Dispose();
            _readCancel = new CancellationTokenSource();
        }

        _readCancel.Cancel();
        await _bufferSwitchSemaphore.WaitAsync();
        Pause();
        try
        {
            ClientId = config.ClientId;
            _clientMasterToken = config.ClientMasterToken;
            _bufferDelegate = bufferDelegate;
            _lastReadIndex = bufferDelegate().GetNextReadIndex();
        }
        finally
        {
            UnPause();
            _readCancel = new CancellationTokenSource();
            _bufferSwitchSemaphore.Release();
        }

        logger.LogInformation("Setting buffer delegate for Buffer.Id: {Id} Circular.Id: {Buffer.Id} {Name} ClientId: {ClientId}", Id, Buffer.Id, config.ChannelName, config.ClientId);
    }

}