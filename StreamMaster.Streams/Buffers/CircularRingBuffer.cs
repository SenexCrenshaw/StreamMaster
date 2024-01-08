using Microsoft.Extensions.Logging;

using System.Diagnostics;

namespace StreamMaster.Infrastructure.VideoStreamManager.Buffers;


/// <summary>
/// Represents a circular ring buffer for streaming data.
/// </summary>
public sealed partial class CircularRingBuffer : ICircularRingBuffer
{
    private TaskCompletionSource<bool> _writeSignal = new(TaskCreationOptions.RunContinuationsAsynchronously);

    public long GetNextReadIndex()
    {
        return WriteBytes;
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
        Guid correlationId = Guid.NewGuid();

        Stopwatch stopwatch = Stopwatch.StartNew();
        CancellationToken linkedToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, StopVideoStreamingToken.Token).Token;

        int bytesRead = 0;
        int bufferLength = _buffer.Length;

        int clientReadIndex = CalculateClientReadIndex(readIndex);

        if (clientReadIndex < 0)
        {
            _logger.LogError("clientReadIndex < 0 ");
            return 0;
        }

        while (!linkedToken.IsCancellationRequested && bytesRead < target.Length)
        {
            int availableBytes = GetAvailableBytes(readIndex, correlationId);

            while (availableBytes == 0)
            {
                await _writeSignal.Task;
                availableBytes = GetAvailableBytes(readIndex, correlationId);
            }

            availableBytes = Math.Min(availableBytes, target.Length - bytesRead);
            // Calculate the number of bytes to read before wrap-around
            int bytesToRead = Math.Min(bufferLength - clientReadIndex, availableBytes); ;

            // Copy data to the target buffer
            Memory<byte> bufferSlice = _buffer.Slice(clientReadIndex, bytesToRead);
            bufferSlice.CopyTo(target[bytesRead..]);
            bytesRead += bytesToRead;

            // Update the client's read index, wrapping around if necessary
            clientReadIndex = (clientReadIndex + bytesToRead) % bufferLength;
        }

        stopwatch.Stop();
        return bytesRead;
    }

    public async Task<int> WriteChunk(Memory<byte> data, CancellationToken cancellationToken)
    {
        Guid correlationId = Guid.NewGuid();
        Stopwatch stopwatch = Stopwatch.StartNew();
        int _bytesWritten = 0;
        int bytesToWrite = data.Length;

        try
        {

            while (data.Length > 0)
            {

                if (cancellationToken.IsCancellationRequested)
                {
                    _writeLogger.LogDebug("WriteChunkAsync was cancelled during wait.");
                    return _bytesWritten; // Or handle the cancellation in an appropriate manner
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

                _writeIndex = (_writeIndex + lengthToWrite) % _buffer.Length;
                WriteBytes += lengthToWrite;
                _bytesWritten += lengthToWrite;

                data = data[lengthToWrite..];
            }
            var logData = new
            {
                Event = "WriteChunk",
                CorrelationId = correlationId,
                VideoStreamName,
                WriteIndex = _writeIndex,
                BytesToWrite = bytesToWrite,
                BytesWritten = _bytesWritten,
                WriteDurationMs = stopwatch.ElapsedMilliseconds // Add timing information
            };
            _writeLogger.LogDebug(System.Text.Json.JsonSerializer.Serialize(logData));
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
            SignalReadersAfterWrite();
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

    /// <summary>
    /// Signals all waiting readers that new data has been written.
    /// </summary>
    private void SignalReadersAfterWrite()
    {
        SignalReaders();
    }

}
