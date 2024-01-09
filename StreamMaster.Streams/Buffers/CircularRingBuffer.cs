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

    public long GetNextReadIndex()
    {
        return WriteBytes;
    }

    private void SetupCancellation(CancellationToken token)
    {
        // Register a callback to complete the _pauseSignal when the token is canceled
        token.Register(() =>
        {
            if (!_pauseSignal.Task.IsCompleted)
            {
                _pauseSignal.TrySetCanceled();
            }

            if (!_writeSignal.Task.IsCompleted)
            {
                _writeSignal.TrySetCanceled();
            }
        });
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

    private int clientReadIndex;
    private int bytesToRead;
    public async Task<int> ReadChunkMemory(long readIndex, Memory<byte> target, CancellationToken cancellationToken)
    {
        Guid correlationId = Guid.NewGuid();

        StopVideoStreamingToken.CancelAfter(TimeSpan.FromSeconds(10));

        Stopwatch stopwatch = Stopwatch.StartNew();
        CancellationToken linkedToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, StopVideoStreamingToken.Token).Token;

        int bytesRead = 0;

        clientReadIndex = CalculateClientReadIndex(readIndex);

        SetupCancellation(linkedToken);

        if (clientReadIndex < 0)
        {
            _logger.LogError("clientReadIndex < 0 ");
            return 0;
        }
        try
        {
            while (!linkedToken.IsCancellationRequested && bytesRead < target.Length)
            {
                await _pauseSignal.Task;

                int availableBytes = GetAvailableBytes(readIndex, correlationId);

                while (availableBytes == 0)
                {
                    await _writeSignal.Task;
                    if (IsPaused())
                    {
                        if (bytesRead > 0)
                        {
                            return bytesRead;
                        }
                        continue;
                    }
                    availableBytes = GetAvailableBytes(readIndex, correlationId);
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
            }
        }
        catch (ArgumentOutOfRangeException ex)
        {
            int l = _buffer.Length;
            int a = clientReadIndex;
            int b = bytesToRead;
        }

        stopwatch.Stop();
        return bytesRead;
    }

    private bool IsPaused()
    {
        return !_pauseSignal.Task.IsCompleted;
    }

    public void PauseReaders(bool pause)
    {
        if (pause)
        {
            if (!_pauseSignal.Task.IsCompleted)
            {
                return;
            }
            // Reset the TaskCompletionSource to an uncompleted state
            _pauseSignal = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        }

        else
        {
            // Complete the TaskCompletionSource
            _pauseSignal.TrySetResult(true);
        }
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
            _logger.LogError(ex, "WriteChunk error occurred while writing chunk for {VideoStreamName}.", VideoStreamName);
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
