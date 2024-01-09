using Microsoft.Extensions.Logging;

using System.Diagnostics;

namespace StreamMaster.Infrastructure.VideoStreamManager.Buffers;


/// <summary>
/// Represents a circular ring buffer for streaming data.
/// </summary>
public sealed partial class CircularRingBuffer : ICircularRingBuffer
{
    private TaskCompletionSource<bool> _writeSignal = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private TaskCompletionSource<bool> _pauseSignal = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private CancellationTokenRegistration _registration;

    public long GetNextReadIndex()
    {
        return WriteBytes;
    }

    private void SetupCancellation(CancellationToken token1, CancellationToken token2, CancellationToken token3)
    {
        CancellationToken linkedToken = CancellationTokenSource.CreateLinkedTokenSource(token1, token2, token3).Token;
        // Register a callback to complete the _pauseSignal when the token is canceled
        _registration = linkedToken.Register(() =>
        {
            if (!_pauseSignal.Task.IsCompleted)
            {
                bool a1 = token1.IsCancellationRequested;
                bool a2 = token2.IsCancellationRequested;
                bool a3 = token3.IsCancellationRequested;
                _pauseSignal.TrySetCanceled();
            }

            if (!_writeSignal.Task.IsCompleted)
            {
                bool a1 = token1.IsCancellationRequested;
                bool a2 = token2.IsCancellationRequested;
                bool a3 = token3.IsCancellationRequested;
                _writeSignal.TrySetCanceled();
            }
        });
    }

    public void UnregisterCancellation()
    {
        _registration.Unregister();
    }

    private int CalculateClientReadIndex(long readByteIndex)
    {
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

        CancellationTokenSource timeOutToken = new();
        timeOutToken.CancelAfter(TimeSpan.FromSeconds(10));

        Stopwatch stopwatch = Stopwatch.StartNew();
        CancellationToken linkedToken = CancellationTokenSource.CreateLinkedTokenSource(timeOutToken.Token, StopVideoStreamingToken.Token, cancellationToken).Token;


        int availableBytes = 0;

        if (clientReadIndex < 0)
        {
            logger.LogError("clientReadIndex < 0");
            _readLogger.LogDebug("ReadChunkMemory return 0");
            return 0;
        }

        try
        {
            while (!linkedToken.IsCancellationRequested && bytesRead < target.Length)
            {

                if (_pauseSignal.Task.IsCanceled)
                {
                    _readLogger.LogDebug("ReadChunkMemory _pauseSignal.Task.IsCanceled");
                    break;

                }
                await _pauseSignal.Task;// .WaitWithTimeoutAsync("PauseSignal", 10000, linkedToken);


                availableBytes = GetAvailableBytes(readIndex, correlationId);

                while (availableBytes == 0)
                {
                    await _writeSignal.Task;//.WaitWithTimeoutAsync("WriteSignal", 30000, linkedToken);
                    if (_writeSignal.Task.IsCanceled)
                    {
                        _readLogger.LogDebug("ReadChunkMemory _writeSignal.Task.IsCanceled");
                        break;
                    }
                    if (IsPaused())
                    {
                        if (bytesRead > 0)
                        {
                            _readLogger.LogDebug("ReadChunkMemory bytes Read > 0");
                            return bytesRead;
                        }
                        continue;
                    }
                    availableBytes = GetAvailableBytes(readIndex, correlationId);
                    if (availableBytes == 0)
                    {
                        int aa = 1;
                    }
                }

                availableBytes = Math.Min(availableBytes, target.Length - bytesRead);
                // Calculate the number of bytes to read before wrap-around
                bytesToRead = Math.Min(_buffer.Length - clientReadIndex, availableBytes); ;

                if (clientReadIndex + bytesToRead > _buffer.Length)
                {
                    int aaa = 1;
                }

                // Copy data to the target buffer
                Memory<byte> bufferSlice = _buffer.Slice(clientReadIndex, bytesToRead);
                bufferSlice.CopyTo(target[bytesRead..]);
                bytesRead += bytesToRead;

                // Update the client's read index, wrapping around if necessary
                clientReadIndex = (clientReadIndex + bytesToRead) % _buffer.Length;
                _readLogger.LogDebug("ReadChunkMemory {bytesRead} {bytesToRead} {clientReadIndex} {writeindex} {availableBytes} {target.Length}", bytesRead, bytesToRead, clientReadIndex, _writeIndex, availableBytes, target.Length);
            }
        }
        catch (TaskCanceledException ex)
        {
            _readLogger.LogDebug(ex, "ReadChunkMemory cancelled");
            _readLogger.LogDebug("ReadChunkMemory {timeOutToken.Token}", timeOutToken.Token.IsCancellationRequested);
            _readLogger.LogDebug("ReadChunkMemory {cancellationToken}", cancellationToken.IsCancellationRequested);
            _readLogger.LogDebug("ReadChunkMemory {StopVideoStreamingToken}", StopVideoStreamingToken.IsCancellationRequested);
            //bytesRead = -1;
        }
        catch (TimeoutException ex)
        {
            _readLogger.LogDebug(ex, "ReadChunkMemory timeout");
            _readLogger.LogDebug("ReadChunkMemory {timeOutToken.Token}", timeOutToken.Token.IsCancellationRequested);
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
            // Ensure that the registration is disposed of
            //UnregisterCancellation();
        }

        stopwatch.Stop();
        if (bytesRead < 1000)
        {
            logger.LogInformation("ReadChunkMemory bytesRead");
            bytesRead = 1;
        }
        return bytesRead;
    }

    public bool IsPaused()
    {
        return !_pauseSignal.Task.IsCompleted;
    }

    public void PauseReaders()
    {

        if (!_pauseSignal.Task.IsCompleted)
        {
            return;
        }
        // Reset the TaskCompletionSource to an uncompleted state
        _pauseSignal = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

    }

    public void UnPauseReaders()
    {

        // Complete the TaskCompletionSource
        _pauseSignal.TrySetResult(true);

    }

    public int WriteChunk(Memory<byte> data)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        int _bytesWritten = 0;

        try
        {

            while (data.Length > 0)
            {
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
            _writeErrorsCounter.WithLabels(Id.ToString(), StreamInfo.VideoStreamName).Inc();
            logger.LogError(ex, "WriteChunk error occurred while writing chunk for {VideoStreamName}.", VideoStreamName);
        }
        finally
        {
            stopwatch.Stop();

            _bytesWrittenCounter.WithLabels(Id.ToString(), StreamInfo.VideoStreamName).Inc(_bytesWritten);
            _writeMetric.RecordBytesProcessed(_bytesWritten);
            _bitsPerSecond.WithLabels(Id.ToString(), StreamInfo.VideoStreamName).Set(_writeMetric.GetBitsPerSecond());
            _inputStreamStatistics.AddBytesWritten(_bytesWritten);
            SignalReaders();
        }
        _writeLogger.LogDebug("WriteChunk {VideoStreamName} {bytesWritten} {elapsedMilliseconds}", VideoStreamName, _bytesWritten, stopwatch.ElapsedMilliseconds);
        return _bytesWritten;
    }

    private void SignalReaders()
    {
        if (!_writeSignal.Task.IsCompleted)
        {
            _writeSignal.TrySetResult(true);
            _writeSignal = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        }
    }
}
