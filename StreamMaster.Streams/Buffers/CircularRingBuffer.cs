using Microsoft.Extensions.Logging;

using StreamMaster.Domain.Cache;

using System.Diagnostics;

namespace StreamMaster.Infrastructure.VideoStreamManager.Buffers;


/// <summary>
/// Represents a circular ring buffer for streaming data.
/// </summary>
public sealed partial class CircularRingBuffer : ICircularRingBuffer
{
    private TaskCompletionSource<bool> _writeSignal = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private TaskCompletionSource<bool> _pauseSignal = new(TaskCreationOptions.RunContinuationsAsynchronously);

    private readonly CancellationTokenRegistration _registration;

    public long GetNextReadIndex()
    {
        return WriteBytes;
    }

    public void UnregisterCancellation()
    {
        _registration.Unregister();
    }

    private int CalculateClientReadIndex(long readByteIndex)
    {
        if (_buffer.Length == 0)
        {
            return 0;
        }
        // Calculate the byte difference
        long byteDifference = WriteBytes - readByteIndex;

        // Convert byte difference to buffer index difference
        int indexDifference = (int)(byteDifference % _buffer.Length);

        // Calculate client read index
        int clientReadIndex = _writeIndex - indexDifference;

        // Adjust for circular buffer wrap-around.
        if (clientReadIndex < 0)
        {
            clientReadIndex += _buffer.Length;
        }

        return clientReadIndex;
    }


    public async Task<int> ReadChunkMemory(long readIndex, Memory<byte> target, CancellationToken cancellationToken)
    {
        int bytesRead = 0;
        int bytesToRead = 0;
        int clientReadIndex = CalculateClientReadIndex(readIndex);

        Guid correlationId = Guid.NewGuid();

        Stopwatch stopwatch = Stopwatch.StartNew();
        CancellationToken linkedToken = CancellationTokenSource.CreateLinkedTokenSource(StopVideoStreamingToken.Token, cancellationToken).Token;

        int availableBytes = 0;

        if (clientReadIndex < 0)
        {
            logger.LogError("clientReadIndex < 0");
            _readLogger.LogDebug("ReadChunkMemory return 0");
            return 0;
        }

        try
        {
            _readLogger.LogDebug($"-------------------{VideoStreamName}-----------------------------");
            while (!linkedToken.IsCancellationRequested && bytesRead == 0)
            {

                if (_pauseSignal.Task.IsCanceled)
                {
                    _readLogger.LogDebug("ReadChunkMemory _pauseSignal.Task.IsCanceled");
                    break;

                }
                await _pauseSignal.Task;

                availableBytes = GetAvailableBytes(readIndex, correlationId);
                _readLogger.LogDebug("Start bytesRead: {bytesRead} bytesToRead: {bytesToRead} clientReadIndex: {clientReadIndex} writeindex: {writeindex} writebytes: {writebytes} availableBytes: {availableBytes} target.Length: {target.Length} readIndex: {readIndex}", bytesRead, bytesToRead, clientReadIndex, _writeIndex, WriteBytes, availableBytes, target.Length, readIndex);

                while (availableBytes == 0 && !cancellationToken.IsCancellationRequested)
                {
                    await _writeSignal.Task.WaitAsync(cancellationToken);//.WaitWithTimeoutAsync("WriteSignal", 30000, linkedToken);
                    if (_writeSignal.Task.IsCanceled || cancellationToken.IsCancellationRequested)
                    {
                        _readLogger.LogDebug("ReadChunkMemory _writeSignal.Task.IsCanceled");
                        break;
                    }
                    if (IsPaused)
                    {
                        continue;
                    }
                    availableBytes = GetAvailableBytes(readIndex, correlationId);
                    if (availableBytes < 1)
                    {
                        int aa = 1;
                    }
                }

                availableBytes = Math.Min(availableBytes, target.Length - bytesRead);
                // Calculate the number of bytes to read before wrap-around
                bytesToRead = Math.Min(_buffer.Length - clientReadIndex, availableBytes); ;
                if (bytesToRead <= 0)
                {
                    _readLogger.LogDebug("ReadChunkMemory bytesToRead == 0");
                    continue;
                }
                if (clientReadIndex + bytesToRead > _buffer.Length)
                {
                    int aaa = 1;
                }

                // Copy data to the target buffer
                Memory<byte> bufferSlice = _buffer.Slice(clientReadIndex, bytesToRead);
                bufferSlice.CopyTo(target[bytesRead..]);
                bytesRead += bytesToRead;

                // Update the client's read index, wrapping around if necessary
                readIndex += bytesToRead;
                clientReadIndex = CalculateClientReadIndex(readIndex);


                _readLogger.LogDebug("End bytesRead: {bytesRead} bytesToRead: {bytesToRead} clientReadIndex: {clientReadIndex} writeindex: {writeindex} writebytes: {writebytes} availableBytes: {availableBytes} target.Length: {target.Length} readIndex: {readIndex}", bytesRead, bytesToRead, clientReadIndex, _writeIndex, WriteBytes, availableBytes, target.Length, readIndex);
            }
        }
        catch (TaskCanceledException ex)
        {
            _readLogger.LogDebug(ex, "ReadChunkMemory cancelled");
            _readLogger.LogDebug("ReadChunkMemory {cancellationToken}", cancellationToken.IsCancellationRequested);
            _readLogger.LogDebug("ReadChunkMemory {StopVideoStreamingToken}", StopVideoStreamingToken.IsCancellationRequested);
        }
        catch (TimeoutException ex)
        {
            _readLogger.LogDebug(ex, "ReadChunkMemory timeout");
            _readLogger.LogDebug("ReadChunkMemory {cancellationToken}", cancellationToken.IsCancellationRequested);
            _readLogger.LogDebug("ReadChunkMemory {StopVideoStreamingToken}", StopVideoStreamingToken.IsCancellationRequested);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            int l = _buffer.Length;
            int a = clientReadIndex;
            int b = bytesToRead;
            int c = availableBytes;
            _readLogger.LogDebug(ex, "ReadChunkMemory arg exception _buffer.Length: {l} clientReadIndex: {a} bytesToRead: {b} availableBytes: {c}", l, a, b, c);
        }
        finally
        {

            _readLogger.LogDebug($"-------------------{VideoStreamName}-----------------------------");
        }

        stopwatch.Stop();

        return bytesRead;
    }

    public bool IsPaused => !_pauseSignal.Task.IsCompleted;

    public void PauseReaders()
    {
        _circularBufferLogger.LogDebug("Pause Readers");
        if (IsPaused)
        {
            return;
        }
        // Reset the TaskCompletionSource to an uncompleted state
        _pauseSignal = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

    }

    public void UnPauseReaders()
    {
        _circularBufferLogger.LogDebug("UnPause Readers");
        // Complete the TaskCompletionSource
        _pauseSignal.TrySetResult(true);

    }

    public int WriteChunk(Memory<byte> data)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        int _bytesWritten = 0;

        try
        {

            while (data.Length > 0 && _buffer.Length > 0)
            {
                if (data.Length < 10000)
                {
                    int aaaa = 1;
                }
                int availableSpace = _buffer.Length - _writeIndex;
                if (availableSpace <= 0)
                {
                    _writeIndex = 0;
                    availableSpace = _buffer.Length;
                }

                int lengthToWrite = Math.Min(data.Length, availableSpace);

                Memory<byte> bufferSlice = _buffer.Slice(_writeIndex, lengthToWrite);
                data[..lengthToWrite].CopyTo(bufferSlice);
                data = data[lengthToWrite..];

                _writeIndex = (_writeIndex + lengthToWrite) % _buffer.Length;
                _bytesWritten += lengthToWrite;
                WriteBytes += lengthToWrite;

            }
        }
        catch (Exception ex)
        {
            Setting setting = memoryCache.GetSetting();
            if (setting.EnablePrometheus)
            {
                _writeErrorsCounter.WithLabels(StreamInfo.VideoStreamName).Inc();
            }

            logger.LogError(ex, "WriteChunk error occurred while writing chunk for {VideoStreamName}.", VideoStreamName);
        }
        finally
        {
            stopwatch.Stop();
            SetMetrics(_bytesWritten);

            SignalReaders();
        }
        _writeLogger.LogDebug("WriteChunk {VideoStreamName} {bytesWritten} {_writeIndex} {elapsedMilliseconds}ms", VideoStreamName, _bytesWritten, _writeIndex, stopwatch.ElapsedMilliseconds);
        return _bytesWritten;
    }



    private void SignalReaders()
    {
        if (IsPaused)
        {
            return;
        }

        _circularBufferLogger.LogDebug("Signal Readers");
        if (!_writeSignal.Task.IsCompleted)
        {
            _writeSignal.TrySetResult(true);
            _writeSignal = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        }
    }
}
