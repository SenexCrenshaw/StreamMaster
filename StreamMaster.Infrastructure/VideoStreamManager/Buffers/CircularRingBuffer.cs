using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using Prometheus;

using StreamMaster.Application.Common.Interfaces;
using StreamMaster.Application.Common.Models;
using StreamMaster.Domain.Cache;
using StreamMaster.Domain.Common;
using StreamMaster.Domain.Dto;
using StreamMaster.Domain.Metrics;

using System.Collections.Concurrent;
using System.Diagnostics;

namespace StreamMaster.Infrastructure.VideoStreamManager.Buffers;

/// <summary>
/// Represents a circular ring buffer for streaming data.
/// </summary>
public sealed class CircularRingBuffer : ICircularRingBuffer
{
    private static readonly Gauge _waitTime = Metrics.CreateGauge(
  "sm_circular_buffer_read_wait_for_data_availability_duration_milliseconds",
        "Client waiting duration in milliseconds for data availability",
new GaugeConfiguration
{
    LabelNames = ["circular_buffer_id", "client_id", "video_stream_name"]

});

    private static readonly Gauge _bitsPerSecond = Metrics.CreateGauge(
"sm_circular_buffer_read_stream_bits_per_second",
"Bits per second read from the input stream.",
new GaugeConfiguration
{
    LabelNames = ["circular_buffer_id", "video_stream_name"]
});

    private static readonly Counter _bytesWrittenCounter = Metrics.CreateCounter(
        "sm_circular_buffer_bytes_written_total",
        "Total number of bytes written.",
        new CounterConfiguration
        {
            LabelNames = ["circular_buffer_id", "video_stream_name"]
        });

    private static readonly Counter _writeErrorsCounter = Metrics.CreateCounter(
        "sm_circular_buffer_write_errors_total",
        "Total number of write errors.",
         new CounterConfiguration
         {
             LabelNames = ["circular_buffer_id", "video_stream_name"]
         });

    private static readonly Gauge _dataArrival = Metrics.CreateGauge(
"sm_circular_buffer_arrival_time_milliseconds",
    "Data arrival times in milliseconds.",
new GaugeConfiguration
{
    LabelNames = ["circular_buffer_id", "video_stream_name"]
});

    private const int maxWaitTimeMs = 250;

    public event EventHandler<Guid> DataAvailable;

    private readonly ConcurrentDictionary<Guid, PerformanceBpsMetrics> _performanceMetrics = new();
    private readonly ConcurrentDictionary<Guid, int> _clientLastReadBeforeOverwrite = new();
    private readonly PerformanceBpsMetrics _writeMetric = new();
    private readonly IStatisticsManager _statisticsManager;
    private readonly IInputStatisticsManager _inputStatisticsManager;
    private readonly IInputStreamingStatistics _inputStreamStatistics;
    public readonly StreamInfo StreamInfo;
    private Memory<byte> _buffer;
    private readonly int _bufferSize;
    private readonly int _originalBufferSize;
    private readonly ConcurrentDictionary<Guid, int> _clientReadIndexes = new();
    private DateTime _lastNotificationTime = new();
    public VideoInfo? VideoInfo { get; set; } = null;
    private readonly ILogger<ICircularRingBuffer> _logger;
    private int _oldestDataIndex;
    private readonly float _preBuffPercent;
    private int _writeIndex;
    //private bool isBufferFull = false;

    public CancellationTokenSource StopVideoStreamingToken { get; set; }

    public CircularRingBuffer(VideoStreamDto videoStreamDto, string channelId, string channelName, IStatisticsManager statisticsManager, IInputStatisticsManager inputStatisticsManager, IMemoryCache memoryCache, int rank, ILogger<ICircularRingBuffer> logger)
    {
        Setting setting = memoryCache.GetSetting();
        _statisticsManager = statisticsManager ?? throw new ArgumentNullException(nameof(statisticsManager));
        _inputStatisticsManager = inputStatisticsManager ?? throw new ArgumentNullException(nameof(inputStatisticsManager));
        _inputStreamStatistics = _inputStatisticsManager.RegisterReader(videoStreamDto.Id);

        _logger = logger;
        if (setting.PreloadPercentage is < 0 or > 100)
        {
            setting.PreloadPercentage = 0;
        }

        if (setting.RingBufferSizeMB is < 1 or > 10)
        {
            setting.RingBufferSizeMB = 1;
        }

        _bufferSize = setting.RingBufferSizeMB * 1024 * 1000;
        _preBuffPercent = setting.PreloadPercentage;

        StreamInfo = new StreamInfo
        {
            ChannelId = channelId,
            ChannelName = channelName,
            VideoStreamId = videoStreamDto.Id,
            VideoStreamName = videoStreamDto.User_Tvg_name,
            Logo = videoStreamDto.User_Tvg_logo,
            StreamingProxyType = videoStreamDto.StreamingProxyType,
            StreamUrl = videoStreamDto.User_Url,

            Rank = rank
        };

        _buffer = new byte[_bufferSize];
        _originalBufferSize = _bufferSize;
        _writeIndex = 0;
        _oldestDataIndex = 0;

        logger.LogInformation("New Circular Buffer {Id} for stream {videoStreamId} {name}", Id, videoStreamDto.Id, videoStreamDto.User_Tvg_name);
    }
    private void OnDataAvailable(Guid clientId)
    {
        DataAvailable?.Invoke(this, clientId);
    }

    public string VideoStreamName => StreamInfo.VideoStreamName;

    //public Memory<byte> GetBufferSlice(int length)
    //{
    //    int bufferEnd = _oldestDataIndex + length;

    //    if (bufferEnd <= _bufferSize)
    //    {
    //        // No wrap-around needed
    //        return _buffer.Slice(_oldestDataIndex, length);
    //    }
    //    else
    //    {
    //        // Handle wrap-around
    //        int lengthToEnd = _bufferSize - _oldestDataIndex;
    //        int lengthFromStart = length - lengthToEnd;

    //        // Create a temporary array to hold the wrapped data
    //        byte[] result = new byte[length];

    //        // Copy from _oldestDataIndex to the end of the buffer
    //        _buffer.Slice(_oldestDataIndex, lengthToEnd).CopyTo(result);

    //        // Copy from start of the buffer to fill the remaining length
    //        _buffer[..lengthFromStart].CopyTo(result.AsMemory(lengthToEnd));

    //        return result;
    //    }
    //}

    public Guid Id { get; } = Guid.NewGuid();
    public int BufferSize => _buffer.Length;
    public string VideoStreamId => StreamInfo.VideoStreamId;
    private bool InternalIsPreBuffered { get; set; } = false;

    public List<StreamStatisticsResult> GetAllStatisticsForAllUrls()
    {
        List<StreamStatisticsResult> allStatistics = [];

        IInputStreamingStatistics input = GetInputStreamStatistics();

        foreach (ClientStreamingStatistics stat in _statisticsManager.GetAllClientStatisticsByClientIds(_clientReadIndexes.Keys))
        {
            allStatistics.Add(new StreamStatisticsResult
            {
                Id = Guid.NewGuid().ToString(),
                CircularBufferId = Id.ToString(),
                ChannelId = StreamInfo.ChannelId,
                ChannelName = StreamInfo.ChannelName,
                VideoStreamId = StreamInfo.VideoStreamId,
                VideoStreamName = StreamInfo.VideoStreamName,
                M3UStreamingProxyType = StreamInfo.StreamingProxyType,
                Logo = StreamInfo.Logo,
                Rank = StreamInfo.Rank,

                InputBytesRead = input.BytesRead,
                InputBytesWritten = input.BytesWritten,
                InputBitsPerSecond = input.BitsPerSecond,
                InputStartTime = input.StartTime,

                StreamUrl = StreamInfo.StreamUrl,

                ClientBitsPerSecond = stat.ReadBitsPerSecond,
                ClientBytesRead = stat.BytesRead,
                ClientId = stat.ClientId,
                ClientStartTime = stat.StartTime,
                ClientAgent = stat.ClientAgent,
                ClientIPAddress = stat.ClientIPAddress
            });
        }

        return allStatistics;
    }

    public int GetAvailableBytes(Guid clientId)
    {
        return _clientReadIndexes.TryGetValue(clientId, out int readIndex) ? (_writeIndex - readIndex + _buffer.Length) % _buffer.Length : 0;
    }

    public ICollection<Guid> GetClientIds()
    {
        return _clientReadIndexes.Keys;
    }

    private IInputStreamingStatistics GetInputStreamStatistics()
    {
        return _inputStreamStatistics;
    }

    public int GetReadIndex(Guid clientId)
    {
        return _clientReadIndexes[clientId];
    }
    public bool IsPreBuffered()
    {
        if (InternalIsPreBuffered)
        {
            _logger.LogDebug("Finished IsPreBuffered with true (already pre-buffered) {VideoStreamId}", VideoStreamId);
            return true;
        }

        int dataInBuffer = (_writeIndex - _oldestDataIndex + _buffer.Length) % _buffer.Length;
        float percentBuffered = (float)dataInBuffer / _buffer.Length * 100;

        InternalIsPreBuffered = percentBuffered >= _preBuffPercent;

        _logger.LogDebug("Finished IsPreBuffered with {isPreBuffered} {VideoStreamId}", InternalIsPreBuffered, VideoStreamId);
        return InternalIsPreBuffered;
    }

    public async Task<int> ReadChunkMemory(Guid ClientId, Memory<byte> target, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Starting ReadChunkMemory for clientId: {clientId}", ClientId);

        PerformanceBpsMetrics metrics = _performanceMetrics.GetOrAdd(ClientId, key => new PerformanceBpsMetrics());


        var linkedToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, StopVideoStreamingToken.Token).Token;
        // Wait for data to become available initially
        while (GetAvailableBytes(ClientId) == 0)
        {
            await WaitForDataAvailability(ClientId, linkedToken);
            if (linkedToken.IsCancellationRequested)
            {
                return 0;
            }
        }

        int bytesRead = 0;
        int bufferLength = _buffer.Length; // Assuming _buffer is the circular buffer's underlying array

        while (!linkedToken.IsCancellationRequested && bytesRead < target.Length)
        {

            int availableBytes = Math.Min(GetAvailableBytes(ClientId), target.Length - bytesRead);
            if (availableBytes == 0)
            {
                if (bytesRead > ((target.Length - bytesRead) * .25))
                {
                    break;
                }
                await Task.Delay(20, linkedToken);
                continue;
            }

            int clientReadIndex = _clientReadIndexes[ClientId];
            // Calculate the number of bytes to read before wrap-around
            int bytesToRead = Math.Min(bufferLength - clientReadIndex, availableBytes);

            // Copy data to the target buffer
            Memory<byte> bufferSlice = _buffer.Slice(clientReadIndex, bytesToRead);
            bufferSlice.CopyTo(target[bytesRead..]);
            bytesRead += bytesToRead;

            // Update the client's read index, wrapping around if necessary
            _clientReadIndexes[ClientId] = (clientReadIndex + bytesToRead) % bufferLength;

            // Check if all requested data has been read
            if (bytesRead >= target.Length)
            {
                break;
            }

            _clientLastReadBeforeOverwrite[ClientId] = _clientReadIndexes[ClientId];
        }

        metrics.RecordBytesProcessed(bytesRead);
        _statisticsManager.AddBytesRead(ClientId, bytesRead);
        _logger.LogDebug("Finished ReadChunkMemory for clientId: {clientId}", ClientId);

        return bytesRead;
    }

    private readonly object bufferLock = new();
    public void ResizeBuffer(int newSize)
    {
        lock (bufferLock) // Ensure exclusive access
        {
            Memory<byte> newBuffer = new byte[newSize];
            _buffer.CopyTo(newBuffer);
            _buffer = newBuffer;
        }
    }

    public async Task WaitForDataAvailability(Guid clientId, CancellationToken cancellationToken)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        try
        {
            while ((!IsPreBuffered() || GetAvailableBytes(clientId) == 0) && !cancellationToken.IsCancellationRequested)
            {
                TaskCompletionSource<bool> tcs = new();

                void handler(object? sender, Guid id)
                {
                    if (id == clientId)
                    {
                        tcs.TrySetResult(true);
                        DataAvailable -= handler;
                    }
                }

                DataAvailable += handler;

                await Task.WhenAny(tcs.Task, Task.Delay(15000, cancellationToken));

            }
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while waiting for data availability for client {ClientId}", clientId);
        }
        finally
        {
            stopwatch.Stop();
            _waitTime.WithLabels(Id.ToString(), clientId.ToString(), StreamInfo.VideoStreamName).Set(stopwatch.Elapsed.TotalMilliseconds);
        }
    }


    public void RegisterClient(IClientStreamerConfiguration streamerConfiguration)
    {
        //int bufferAdvancePercentage = 10; // 10% of the buffer size
        //int maxBufferAdvance = (int)(_buffer.Length * (bufferAdvancePercentage / 100.0));
        ////int availableData = CalculateDistance(_oldestDataIndex, _writeIndex);

        //// Adjust buffer advance based on available data
        ////int actualBufferAdvance = Math.Min(maxBufferAdvance, availableData);
        //int clientReadIndex = (_oldestDataIndex) % _buffer.Length;

        if (_clientReadIndexes.TryAdd(streamerConfiguration.ClientId, _oldestDataIndex))
        {
            _statisticsManager.RegisterClient(streamerConfiguration);
            _logger.LogInformation("Registered new client {ClientId} with read index {ReadIndex}", streamerConfiguration.ClientId, _oldestDataIndex);
        }
        else
        {
            _logger.LogWarning("Failed to add new client {ClientId} to read indexes.", streamerConfiguration.ClientId);
            // Handle the case if adding the client fails
        }
    }


    private void NotifyClients()
    {
        DateTime now = DateTime.UtcNow;
        // Get the last notification time and update it to now
        DateTime lastNotificationTime = _lastNotificationTime;
        _lastNotificationTime = now;

        // Log the elapsed time if it's more than 500 milliseconds
        TimeSpan elapsed = now - lastNotificationTime;
        _dataArrival.WithLabels(Id.ToString(), StreamInfo.VideoStreamName).Set(elapsed.TotalMilliseconds);

        if (elapsed.TotalMilliseconds is > 15000 and < 60000000000000)
        {
            // Log the elapsed time here
            _logger.LogWarning($"Input stream is slow: {StreamInfo.VideoStreamName} {elapsed.TotalMilliseconds}ms elapsed since last set. Cancelling Stream");
            StopVideoStreamingToken.Cancel();
        }

        foreach (Guid clientId in _clientReadIndexes.Keys)
        {
            OnDataAvailable(clientId);
        }
    }

    public void UnRegisterClient(Guid clientId)
    {
        _ = _clientReadIndexes.TryRemove(clientId, out _);
        _statisticsManager.UnRegisterClient(clientId);

        _logger.LogInformation("UnRegisterClient for clientId: {clientId}  {VideoStreamName}", clientId, StreamInfo.VideoStreamName);
    }

    public async Task<int> WriteChunk(Memory<byte> data, CancellationToken cancellationToken)
    {
        //_writePending = true;
        Stopwatch stopwatch = new();
        stopwatch.Start();

        _logger.LogDebug("Starting WriteChunk {VideoStreamId} with count: {count}", VideoStreamId, data.Length);

        int bytesWritten = 0;
        try
        {
            int initialWriteIndex = _writeIndex;
            while (data.Length > 0)
            {
                int waitTime;

                if (IsPreBuffered())
                {
                    waitTime = CalculateDynamicWaitTime();
                    if (waitTime > 0)
                    {
                        await Task.Delay(waitTime, cancellationToken);
                        _logger.LogWarning($"Dynamically adjusting write wait: {StreamInfo.VideoStreamName} waiting {waitTime}ms. Is Pre Buffered: {IsPreBuffered()}");
                    }
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    _logger.LogDebug("WriteChunkAsync was cancelled during wait.");
                    return bytesWritten; // Or handle the cancellation in an appropriate manner
                }

                int availableSpace = _buffer.Length - _writeIndex;
                if (availableSpace == 0)
                {
                    _writeIndex = 0;
                    availableSpace = _buffer.Length;
                }

                int lengthToWrite = Math.Min(data.Length, availableSpace);
                CheckAndReportClientOverwrites(initialWriteIndex, lengthToWrite);

                Memory<byte> bufferSlice = _buffer.Slice(_writeIndex, lengthToWrite);
                data[..lengthToWrite].CopyTo(bufferSlice);

                if (!isBufferFull && _writeIndex + lengthToWrite >= _buffer.Length)
                {
                    isBufferFull = true;
                }

                _writeIndex = (_writeIndex + lengthToWrite) % _buffer.Length;
                bytesWritten += lengthToWrite;
                data = data[lengthToWrite..];


                // Increment _oldestDataIndex to the next position after _writeIndex
                if (isBufferFull)
                {
                    _oldestDataIndex = (_writeIndex + 1) % _buffer.Length;
                }

            }

            stopwatch.Stop();

            _bytesWrittenCounter.WithLabels(Id.ToString(), StreamInfo.VideoStreamName).Inc(bytesWritten);
            _writeMetric.RecordBytesProcessed(bytesWritten);
            _bitsPerSecond.WithLabels(Id.ToString(), StreamInfo.VideoStreamName).Set(_writeMetric.GetBitsPerSecond());
        }
        catch (Exception ex)
        {
            _writeErrorsCounter.WithLabels(Id.ToString(), StreamInfo.VideoStreamName).Inc();
            _logger.LogError(ex, "WriteChunk error occurred while writing chunk for {VideoStreamId}.", VideoStreamId);
        }
        finally
        {
            stopwatch.Stop();

        }

        _inputStreamStatistics.AddBytesWritten(bytesWritten);

        NotifyClients();

        _logger.LogDebug("WriteChunk completed with {VideoStreamId} count: {bytesWritten}", VideoStreamId, bytesWritten);

        return bytesWritten;
    }

    private int CalculateDynamicWaitTime()
    {
        int closestReadIndexDistance = CalculateClosestReadIndexDistance();
        int throttleThreshold = (int)(_buffer.Length * 0.10);

        if (closestReadIndexDistance > throttleThreshold)
        {
            return 0; // No wait required
        }


        if (closestReadIndexDistance == 0)
        {
            return maxWaitTimeMs; // If the read index is very close, wait for the maximum time
        }

        double proximity = (double)closestReadIndexDistance / throttleThreshold;
        int calculatedWaitTime = (int)(-maxWaitTimeMs * Math.Log(proximity));

        return Math.Min(calculatedWaitTime, maxWaitTimeMs); // Ensure wait time doesn't exceed maxWaitTime
    }


    private int CalculateClosestReadIndexDistance()
    {
        int closestDistance = _buffer.Length;
        foreach (var readIndex in _clientReadIndexes.Values)
        {
            int distance = CalculateDistance(_oldestDataIndex, readIndex);
            if (distance < closestDistance)
            {
                closestDistance = distance;
            }
        }
        return closestDistance;
    }

    private int CalculateDistance(int startIndex, int endIndex)
    {
        if (startIndex <= endIndex)
        {
            return endIndex - startIndex;
        }
        else
        {
            if (!isBufferFull)
            {
                // If the buffer hasn't looped, treat this as a direct distance without wrap-around
                return _oldestDataIndex;
            }
            // Handle wrap-around case
            return (_buffer.Length - startIndex) + endIndex;
        }
    }

    private void CheckAndReportClientOverwrites(int initialWriteIndex, int lengthToWrite)
    {
        int newWriteIndex = (_writeIndex + lengthToWrite) % _buffer.Length;
        foreach (KeyValuePair<Guid, int> clientReadIndexEntry in _clientReadIndexes)
        {
            int clientReadIndex = clientReadIndexEntry.Value;
            if (IsClientOverwritten(clientReadIndex, initialWriteIndex, newWriteIndex))
            {
                Guid clientId = clientReadIndexEntry.Key;
                // Report the overwrite
                int lastReadBeforeOverwrite = _clientLastReadBeforeOverwrite.TryGetValue(clientId, out int lastRead) ? lastRead : -1;

                _logger.LogWarning($"Client {clientId}'s read index {clientReadIndex} (last read before overwrite: {lastReadBeforeOverwrite}) was overwritten by a write operation. Write started at {initialWriteIndex} and ended at {newWriteIndex} in {VideoStreamName}");

                _clientReadIndexes[clientId] = CalculateSafeReadIndex(newWriteIndex);
                // ResizeBuffer();
            }
        }
    }

    private void ResizeBuffer()
    {
        int currentSize = _buffer.Length;

        int increaseBy = (int)(currentSize * 0.20); // 20% increase
        int maxSize = _originalBufferSize * 4; // Maximum size, e.g., 4 times the original size

        // Calculate the new size, ensuring it's not greater than the max
        int newSize = Math.Min(currentSize + increaseBy, maxSize);

        // Check if new size is actually larger than current size
        if (newSize > currentSize)
        {
            _logger.LogInformation($"Resizing buffer from {currentSize} to {newSize}");
            try
            {
                ResizeBuffer(newSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while resizing the buffer");
                // Handle or rethrow the exception as needed
            }
        }
        else
        {
            //_logger.LogInformation("Buffer resize not required or maximum size reached.");
        }
    }

    private static int CalculateSafeReadIndex(int newWriteIndex)
    {
        // Implement logic to calculate a safe read index for the client
        // This might be the new write index, or some position before it, depending on your buffer's logic
        return newWriteIndex + 1;
    }

    private static bool IsClientOverwritten(int clientReadIndex, int initialWriteIndex, int newWriteIndex)
    {

        // Check if the client's read index falls within the range of data that was overwritten
        if (initialWriteIndex <= newWriteIndex)
        {
            // No wrap-around
            return clientReadIndex > initialWriteIndex && clientReadIndex <= newWriteIndex;
        }
        else
        {
            // Wrap-around occurred
            return clientReadIndex > initialWriteIndex || clientReadIndex <= newWriteIndex;
        }
    }

    private bool HasOverwrittenOldestData(int lengthToWrite)
    {
        if (!isBufferFull)
        {
            return false;
        }

        if (_writeIndex >= _oldestDataIndex)
        {
            return true;
        }
        else
        {
            int effectiveWriteEnd = (_writeIndex + lengthToWrite) % _buffer.Length;
            return effectiveWriteEnd < _writeIndex
                ? _oldestDataIndex <= effectiveWriteEnd || _oldestDataIndex > _writeIndex
                : effectiveWriteEnd > _oldestDataIndex;
        }
    }

    private bool _disposed = false; // To track whether Dispose has been called
    private bool isBufferFull;

    private void DoDispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _waitTime.GetAllLabelValues().ToList().ForEach(x => _waitTime.RemoveLabelled(x[0], x[1], x[2]));
                _bitsPerSecond.RemoveLabelled(Id.ToString(), StreamInfo.VideoStreamName);
                _bytesWrittenCounter.RemoveLabelled(Id.ToString(), StreamInfo.VideoStreamName);
                _writeErrorsCounter.RemoveLabelled(Id.ToString(), StreamInfo.VideoStreamName);
                _dataArrival.RemoveLabelled(Id.ToString(), StreamInfo.VideoStreamName);
                foreach (var item in _clientReadIndexes)
                {
                    _statisticsManager.UnRegisterClient(item.Key);
                }
                _clientReadIndexes.Clear();
                _clientLastReadBeforeOverwrite.Clear();
                _performanceMetrics.Clear();
            }

            // Dispose unmanaged resources here if any

            _disposed = true;
        }


    }

    // Public implementation of Dispose pattern callable by consumers
    public void Dispose()
    {
        DoDispose(true);
        GC.SuppressFinalize(this);
    }

    // Finalizer in case Dispose wasn't called
    ~CircularRingBuffer()
    {
        DoDispose(false);
    }

}
