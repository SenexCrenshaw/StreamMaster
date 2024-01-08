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
        return _writeBytes;
    }

    private int CalculateClientReadIndex(long readByteIndex)
    {
        // Calculate the preliminary client read index.
        int clientReadIndex = _writeIndex - (int)(_writeBytes - readByteIndex);

        // Adjust for circular buffer wrap-around.
        if (clientReadIndex < 0)
        {
            clientReadIndex += _buffer.Length;
        }

        // Ensure the index is within the bounds of the buffer.
        clientReadIndex %= _buffer.Length;

        return clientReadIndex;
    }


    public async Task<int> ReadChunkMemory(long readIndex, Memory<byte> target, CancellationToken cancellationToken)
    {
        Guid correlationId = Guid.NewGuid();

        Stopwatch stopwatch = Stopwatch.StartNew();

        //PerformanceBpsMetrics metrics = _performanceMetrics.GetOrAdd(ClientId, key => new PerformanceBpsMetrics());

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

        //metrics.RecordBytesProcessed(bytesRead);
        //_statisticsManager.AddBytesRead(ClientId, bytesRead);

        //if (_clientReadIndexes.TryGetValue(ClientId, out int value))
        //{
        //    // Enhanced structured logging
        //    var logData = new
        //    {
        //        Event = "ReadChunkMemory",
        //        CorrelationId = correlationId,
        //        ClientId,
        //        VideoStreamName,
        //        ClientReadIndex = value,
        //        BytesRead = bytesRead,
        //        ReadDurationMs = stopwatch.ElapsedMilliseconds,
        //        BufferOccupancy = CalculateBufferOccupancy(),
        //        ActiveReaders = _clientReadIndexes.Count,
        //        DistanceToOldestReader = CalculateDistanceToOldestReader(correlationId)
        //    };
        //    _readLogger.LogDebug(JsonSerializer.Serialize(logData));
        //}

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
                _writeBytes += lengthToWrite;
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

    /// <summary>
    /// Calculates the amount of data currently stored in the buffer.
    /// This method determines how much of the buffer is occupied with data
    /// that has been written but not necessarily read by all clients.
    /// </summary>
    /// <returns>The total number of bytes occupied in the buffer.</returns>
    //private int CalculateBufferOccupancy()
    //{
    //    // If the buffer hasn't flipped (i.e., write index hasn't reached the end of the buffer),
    //    // occupancy is simply the current position of the write index.
    //    if (!HasBufferFlipped)
    //    {
    //        return _writeIndex;
    //    }

    //    // When the buffer has flipped, it means the write operation has wrapped around to the start
    //    // and may be overwriting older data. In this case, the method needs to find the oldest 
    //    // (furthest behind) read index among all clients. This index helps in understanding
    //    // the part of the buffer that has data which might not have been read yet.
    //    int oldestReadIndex = _clientReadIndexes.Values.DefaultIfEmpty(_buffer.Length).Min();

    //    // If there are no readers, the whole buffer is considered occupied.
    //    if (oldestReadIndex == _buffer.Length)
    //    {
    //        return _buffer.Length;
    //    }

    //    // Calculate the occupied space based on the distance between the write index and the oldest read index.
    //    // This helps determine how much new data can be accepted before overwriting unread data.
    //    return _writeIndex >= oldestReadIndex ?
    //        _writeIndex - oldestReadIndex : // Write index is ahead of the oldest read index
    //        _buffer.Length - oldestReadIndex + _writeIndex; // Write index has wrapped around past the oldest read index
    //}



    /// <summary>
    /// Calculates the maximum distance from the write index to any read index.
    /// This represents the furthest (oldest) unread data in the buffer.
    /// </summary>
    /// <param name="correlationId">Correlation identifier for logging purposes.</param>
    /// <returns>The maximum distance to the oldest reader in the buffer.</returns>
    //private int CalculateDistanceToOldestReader(Guid correlationId)
    //{
    //    int maxDistance = 0;
    //    foreach (KeyValuePair<Guid, int> clientReadIndexEntry in _clientReadIndexes)
    //    {
    //        int distance = CalculateDistance(clientReadIndexEntry.Value, correlationId);
    //        if (distance > maxDistance)
    //        {
    //            maxDistance = distance;
    //        }
    //    }

    //    // Structured logging
    //    var logData = new
    //    {
    //        Event = "CalculateDistanceToOldestReader",
    //        CorrelationId = correlationId,
    //        VideoStreamName,
    //        MaxDistanceToOldestReader = maxDistance
    //    };
    //    _distanceLogger.LogDebug(System.Text.Json.JsonSerializer.Serialize(logData));

    //    return maxDistance;
    //}

    /// <summary>
    /// Finds the closest read index to the write index, indicating the nearest reader.
    /// </summary>
    /// <param name="correlationId">Correlation identifier for logging purposes.</param>
    /// <returns>The shortest distance to the nearest reader in the buffer.</returns>
    //private int CalculateClosestReadIndexDistance(Guid correlationId)
    //{
    //    int closestDistance = _buffer.Length;

    //    foreach (KeyValuePair<Guid, int> clientReadIndexEntry in _clientReadIndexes)
    //    {
    //        int distance = CalculateDistance(clientReadIndexEntry.Value, correlationId);
    //        if (distance < closestDistance)
    //        {
    //            closestDistance = distance;
    //        }
    //    }

    //    // Structured logging
    //    var logData = new
    //    {
    //        Event = "CalculateClosestReadIndexDistance",
    //        CorrelationId = correlationId,
    //        ClosestDistance = closestDistance,
    //        ActiveReaders = _clientReadIndexes.Count
    //    };
    //    _distanceLogger.LogDebug(JsonSerializer.Serialize(logData));

    //    return closestDistance;
    //}

    /// <summary>
    /// Calculates the distance between a given read index and the current write index.
    /// This helps determine how far behind a reader is in the buffer.
    /// </summary>
    /// <param name="readIndex">The read index to calculate the distance from.</param>
    /// <param name="correlationId">Correlation identifier for logging purposes.</param>
    /// <returns>The calculated distance from the read index to the write index.</returns>
    private int CalculateDistance(int readIndex, Guid correlationId)
    {
        if (!HasBufferFlipped)
        {
            // Log when the buffer hasn't flipped
            LogCalculateDistance(correlationId, readIndex, _buffer.Length, "Buffer not flipped");
            return _buffer.Length;
        }

        // Handle wrap-around case
        int ret = _buffer.Length - _writeIndex + readIndex;

        // Structured logging
        LogCalculateDistance(correlationId, readIndex, ret, "Calculated distance");

        return ret;
    }




    //private void CheckAndReportClientOverwrites(int lengthToWrite, Guid correlationId)
    //{

    //    int newWriteIndex = (_writeIndex + lengthToWrite) % _buffer.Length;
    //    foreach (KeyValuePair<Guid, int> clientReadIndexEntry in _clientReadIndexes)
    //    {
    //        int clientReadIndex = clientReadIndexEntry.Value;

    //        _overwriteLogger.LogDebug($"CheckAndReportClientOverwrites {_buffer.Length} {clientReadIndex} {_writeIndex} {newWriteIndex} {lengthToWrite}");

    //        if (IsClientOverwritten(clientReadIndex, _writeIndex, newWriteIndex))
    //        {
    //            Guid clientId = clientReadIndexEntry.Key;
    //            _overwriteLogger.LogWarning(System.Text.Json.JsonSerializer.Serialize(new
    //            {
    //                Event = "ClientOverwrite",
    //                CorrelationId = correlationId,
    //                ClientId = clientId,
    //                VideoStreamName,
    //                ClientReadIndex = clientReadIndexEntry.Value,
    //                WriteIndex = _writeIndex,
    //                NewWriteIndex = newWriteIndex
    //            }));
    //            _logger.LogWarning($"Client {clientId}'s read index {clientReadIndex} newWriteIndex {newWriteIndex} in {VideoStreamName} ");

    //            _clientReadIndexes[clientId] = CalculateSafeReadIndex(newWriteIndex);
    //            // ResizeBuffer();
    //        }
    //    }
    //}

    //private void CheckAndReportClientOverwrites(int lengthToWrite, Guid correlationId)
    //{
    //    int newWriteIndex = (_writeIndex + lengthToWrite) % _buffer.Length;

    //    foreach (KeyValuePair<Guid, int> clientReadIndexEntry in _clientReadIndexes)
    //    {
    //        int clientReadIndex = clientReadIndexEntry.Value;

    //        if (IsClientOverwritten(clientReadIndex, _writeIndex, newWriteIndex))
    //        {
    //            AdjustClientReadIndex(clientReadIndexEntry.Key, newWriteIndex, correlationId);
    //        }
    //    }
    //}


    /// <summary>
    /// Adjusts the read index of a client to a safe position to avoid overwrites.
    /// </summary>
    //private void AdjustClientReadIndex(Guid clientId, int newWriteIndex, Guid correlationId)
    //{
    //    int safeReadIndex = CalculateSafeReadIndex(newWriteIndex);
    //    _clientReadIndexes[clientId] = safeReadIndex;

    //    // Log the adjustment for debugging and monitoring
    //    _overwriteLogger.LogWarning(JsonSerializer.Serialize(new
    //    {
    //        Event = "AdjustClientReadIndex",
    //        CorrelationId = correlationId,
    //        ClientId = clientId,
    //        SafeReadIndex = safeReadIndex,
    //        OriginalReadIndex = _clientReadIndexes[clientId]
    //    }));
    //}

    //private int CalculateThrottleWaitTime(Guid correlationId)
    //{
    //    int throttleThreshold = (int)(_buffer.Length * 0.10); // Threshold for throttling (10% of buffer size)
    //    int closestReadIndexDistance = CalculateClosestReadIndexDistance(correlationId);

    //    if (closestReadIndexDistance > throttleThreshold)
    //    {
    //        return 0; // No throttling required
    //    }

    //    if (closestReadIndexDistance == 0)
    //    {
    //        return maxDynamicWaitTimeMs; // Maximum wait if a client is immediately behind the write index
    //    }

    //    // Calculate proportional wait time based on proximity to the closest reader
    //    double proximity = (double)closestReadIndexDistance / throttleThreshold;
    //    int calculatedWaitTime = (int)(-maxDynamicWaitTimeMs * Math.Log(proximity));

    //    // Log the calculated wait time for monitoring
    //    LogDynamicWaitTime(correlationId, throttleThreshold, closestReadIndexDistance, calculatedWaitTime);

    //    return Math.Min(calculatedWaitTime, maxDynamicWaitTimeMs); // Cap the wait time to a maximum value
    //}


    //private int CalculateSafeReadIndex(int newWriteIndex)
    //{
    //    int bufferLength = _buffer.Length;

    //    newWriteIndex += ((int)(bufferLength * 0.10)) % bufferLength;

    //    // Implement logic to calculate a safe read index for the client
    //    // This might be the new write index, or some position before it, depending on your buffer's logic
    //    return newWriteIndex;
    //}

    //private bool IsClientOverwritten(int clientReadIndex, int initialWriteIndex, int newWriteIndex)
    //{
    //    _overwriteLogger.LogDebug($"IsClientOverwritten {clientReadIndex} {initialWriteIndex} {newWriteIndex}");

    //    if (clientReadIndex == initialWriteIndex) // Client is waiting for data
    //    {
    //        return false;
    //    }


    //    //Write doesnt wrap
    //    if (initialWriteIndex <= newWriteIndex)
    //    {
    //        //In the middle of the write
    //        bool test = clientReadIndex > initialWriteIndex && clientReadIndex <= newWriteIndex;
    //        _overwriteLogger.LogDebug($"IsClientOverwritten No wrap {test}");
    //        return test;
    //    }
    //    // Write wraps around

    //    // Check if clientReadIndex is outside the range of newWriteIndex to initialWriteIndex
    //    bool isOverwritten = !(clientReadIndex > newWriteIndex && clientReadIndex <= initialWriteIndex);
    //    _overwriteLogger.LogDebug($"IsClientOverwritten wrap {isOverwritten}");
    //    return isOverwritten;
    //}

}
