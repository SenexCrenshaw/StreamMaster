using Microsoft.Extensions.Logging;

using System.Text.Json;

namespace StreamMaster.Infrastructure.VideoStreamManager.Buffers;


/// <summary>
/// Represents a circular ring buffer for streaming data.
/// </summary>
public sealed partial class CircularRingBuffer : ICircularRingBuffer
{
    private void LogDynamicWaitTime(Guid correlationId, int throttleThreshold, int closestReadIndexDistance, int waitTime)
    {
        var logData = new
        {
            Event = "CalculateDynamicWaitTime",
            CorrelationId = correlationId,
            VideoStreamName,
            ThrottleThreshold = throttleThreshold,
            ClosestReadIndexDistance = closestReadIndexDistance,
            CalculatedWaitTime = waitTime
        };
        _waitLogger.LogDebug(System.Text.Json.JsonSerializer.Serialize(logData));
    }

    private void LogCalculateDistance(Guid correlationId, int readIndex, int distance, string message)
    {
        var logData = new
        {
            Event = "CalculateDistance",
            CorrelationId = correlationId,
            VideoStreamName,
            ReadIndex = readIndex,
            WriteIndex = _writeIndex,
            CalculatedDistance = distance,
            Message = message
        };
        _distanceLogger.LogDebug(System.Text.Json.JsonSerializer.Serialize(logData));
    }

    private int GetAvailableBytes(long readIndex, Guid correlationId)
    {
        long availableBytes = _writeBytes - readIndex;
        if (availableBytes == 0)
        {
            _readLogger.LogDebug(JsonSerializer.Serialize(new
            {
                Event = "CheckAvailableBytes",
                CorrelationId = correlationId,
                //ClientId = clientId,
                VideoStreamName,
                ReadIndex = readIndex,
                WriteIndex = _writeIndex,
                AvailableBytes = availableBytes
            }));
        }
        return (int)availableBytes;
    }

    public bool IsPreBuffered()
    {
        if (InternalIsPreBuffered || HasBufferFlipped)
        {
            //_logger.LogDebug("Finished IsPreBuffered with true  {VideoStreamName}", VideoStreamName);
            return true;
        }

        long dataInBuffer = _writeIndex % _buffer.Length;
        float percentBuffered = (float)dataInBuffer / _buffer.Length * 100;

        InternalIsPreBuffered = percentBuffered >= _preBuffPercent;
        if (InternalIsPreBuffered)
        {
            _logger.LogInformation("Finished IsPreBuffered {VideoStreamName}", VideoStreamName);
        }

        //_logger.LogDebug($"IsPreBuffered check  with true dataInBuffer: {dataInBuffer} percentBuffered: {percentBuffered} preBuffPercent: {_preBuffPercent}");
        return InternalIsPreBuffered;
    }

}
