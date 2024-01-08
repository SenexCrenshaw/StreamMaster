using Microsoft.Extensions.Logging;

using System.Text.Json;

namespace StreamMaster.Infrastructure.VideoStreamManager.Buffers;

public sealed partial class CircularRingBuffer : ICircularRingBuffer
{
    private readonly Timer _bufferHealthLogger;

    private void LogBufferHealth(object state)
    {
        var bufferHealthData = new
        {
            Event = "BufferHealthCheck",
            WriteIndex = _writeIndex,
            //ClientBufferStatuses = _clientReadIndexes.Select(clientReadIndex =>
            //    new
            //    {
            //        ClientId = clientReadIndex.Key,
            //        ReadIndex = clientReadIndex.Value,
            //        DistanceToWriteIndex = CalculateDistanceToWriteIndex(clientReadIndex.Value),
            //        BufferHealth = CalculateBufferHealth(clientReadIndex.Value)
            //    }
            //).ToList()
        };

        _statsLogger.LogDebug(JsonSerializer.Serialize(bufferHealthData));
    }

    /// <summary>
    /// Calculates the distance between a specified read index and the current write index.
    /// This distance indicates the amount of unread data for a client in the buffer.
    /// If the buffer has wrapped around (i.e., the write index has looped back to the start of the buffer),
    /// calculate the distance by adding the space from the read index to the end of the buffer
    /// and from the start of the buffer to the write index.
    /// </summary>
    /// <param name="readIndex">The read index from which to calculate the distance.</param>
    /// <returns>The number of bytes from the read index to the write index.</returns>
    private int CalculateDistanceToWriteIndex(int readIndex)
    {
        // If the read index is behind or at the write index, simply subtract to find the distance.
        return readIndex <= _writeIndex ? _writeIndex - readIndex : _buffer.Length - readIndex + _writeIndex;
    }

    /// <summary>
    /// Calculates the buffer health for a specific client based on their current read index.
    /// Buffer health is a measure of how much of the buffer a client has already read,
    /// represented as a percentage of the total buffer length.
    /// </summary>
    /// <param name="readIndex">The current read index for the client.</param>
    /// <returns>The buffer health as a percentage, indicating how much of the buffer the client has read.</returns>
    private double CalculateBufferHealth(int readIndex)
    {
        // Calculate the distance from the client's current read index to the write index.
        // This distance represents the amount of data that the client has not yet read.
        int distanceToWriteIndex = CalculateDistanceToWriteIndex(readIndex);

        // Calculate the buffer health as a percentage. A lower percentage means the client has
        // read most of the buffer and is close to the write index, indicating good health.
        // A higher percentage indicates the client is lagging behind the write index,
        // which may be a cause for concern if it gets too high.
        return 100 - ((double)distanceToWriteIndex / _buffer.Length * 100);
    }

}
