using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using Prometheus;

using StreamMaster.Domain.Cache;
using StreamMaster.Domain.Common;
using StreamMaster.Domain.Dto;
using StreamMaster.Domain.Models;

using StreamMaster.Application.Common.Interfaces;
using StreamMaster.Application.Common.Models;

using System.Collections.Concurrent;
using System.Diagnostics;

namespace StreamMaster.Infrastructure.VideoStreamManager.Buffers;

/// <summary>
/// Represents a circular ring buffer for streaming data.
/// </summary>
public sealed class CircularRingBuffer : ICircularRingBuffer
{

    private static readonly Histogram WaitTimeHistogram = Metrics.CreateHistogram(
        "circular_buffer_wait_for_data_availability_duration_milliseconds",
        "Duration in milliseconds of waiting for data availability",
        new HistogramConfiguration
        {
            // Define buckets suitable for millisecond durations
            // Example: Buckets from 1 ms to 1000 ms in increments of 10 ms
            Buckets = Histogram.LinearBuckets(start: 1, width: 10, count: 100), // Adjust as needed
            LabelNames = ["circular_buffer_id", "client_id", "video_stream_name"]
        });

    private static readonly Gauge _bytesPerSecond = Metrics.CreateGauge(
"circular_buffer_read_stream_bytes_per_second",
"Bytes per second read from the input stream.",
new GaugeConfiguration
{
    LabelNames = ["circular_buffer_id", "video_stream_name"]
});


    private static readonly Histogram _bytesPerSecondHistogram = Metrics.CreateHistogram(
    "circular_buffer_read_stream_bytes_per_second_histogram",
    "Histogram of bytes per second read from the input stream.",
    new HistogramConfiguration
    {
        Buckets = Histogram.LinearBuckets(0, 20000000, 11),
        LabelNames = ["circular_buffer_id", "video_stream_name"]
    });

    private static readonly Histogram BytesWrittenHistogram = Metrics.CreateHistogram(
    "circular_buffer_bytes_written_histogram",
    "Histogram of bytes written.",
    new HistogramConfiguration
    {
        Buckets = Histogram.LinearBuckets(0, 20000000, 11),
        LabelNames = ["circular_buffer_id", "video_stream_name"]
    });

    private static readonly Histogram WriteDurationHistogram = Metrics.CreateHistogram(
       "circular_buffer_write_chunk_duration_milliseconds",
       "Histogram for the duration of the WriteChunk operation.",
       new HistogramConfiguration
       {
           Buckets = Histogram.LinearBuckets(start: 0.001, width: 0.01, count: 100),
           LabelNames = ["circular_buffer_id", "video_stream_name"]
       });


    private static readonly Counter BytesWrittenCounter = Metrics.CreateCounter(
        "circular_buffer_bytes_written_total",
        "Total number of bytes written.",
        new CounterConfiguration
        {
            LabelNames = ["circular_buffer_id", "video_stream_name"]
        });

    private static readonly Counter WriteErrorsCounter = Metrics.CreateCounter(
        "circular_buffer_write_errors_total",
        "Total number of write errors.",
         new CounterConfiguration
         {
             LabelNames = ["circular_buffer_id", "video_stream_name"] // Add the additional label here
         });

    private static readonly Histogram _dataArrivalHistogram = Metrics.CreateHistogram(
    "circular_buffer_arrival_time_milliseconds",
    "Histogram of data arrival times in milliseconds.",
        new HistogramConfiguration
        {
            Buckets = Histogram.LinearBuckets(start: 0.001, width: 0.01, count: 100), // Adjust the buckets to your needs
            LabelNames = ["circular_buffer_id", "video_stream_name"]
        });

    public event EventHandler<Guid> DataAvailable;
    private readonly ConcurrentDictionary<Guid, int> _clientLastReadBeforeOverwrite = new();

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
    private bool isBufferFull = false;

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

    public Memory<byte> GetBufferSlice(int length)
    {
        int bufferEnd = _oldestDataIndex + length;

        if (bufferEnd <= _bufferSize)
        {
            // No wrap-around needed
            return _buffer.Slice(_oldestDataIndex, length);
        }
        else
        {
            // Handle wrap-around
            int lengthToEnd = _bufferSize - _oldestDataIndex;
            int lengthFromStart = length - lengthToEnd;

            // Create a temporary array to hold the wrapped data
            byte[] result = new byte[length];

            // Copy from _oldestDataIndex to the end of the buffer
            _buffer.Slice(_oldestDataIndex, lengthToEnd).CopyTo(result);

            // Copy from start of the buffer to fill the remaining length
            _buffer[..lengthFromStart].CopyTo(result.AsMemory(lengthToEnd));

            return result;
        }
    }

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
        int a = _clientReadIndexes.TryGetValue(clientId, out int readIndex2) ? (_writeIndex - readIndex2 + _buffer.Length) % _buffer.Length : 0;
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

    public async Task<int> ReadChunkMemory(Guid clientId, Memory<byte> target, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Starting ReadChunkMemory for clientId: {clientId}", clientId);

        // Wait for data to become available initially
        while (GetAvailableBytes(clientId) == 0)
        {
            await WaitForDataAvailability(clientId, cancellationToken);
            if (cancellationToken.IsCancellationRequested)
            {
                return 0;
            }
        }

        int bytesRead = 0;
        int bufferLength = _buffer.Length; // Assuming _buffer is the circular buffer's underlying array

        while (!cancellationToken.IsCancellationRequested && bytesRead < target.Length)
        {

            int availableBytes = Math.Min(GetAvailableBytes(clientId), target.Length - bytesRead);
            if (availableBytes == 0)
            {
                if (bytesRead > ((target.Length - bytesRead) * .25))
                {
                    break;
                }
                await Task.Delay(20);
                continue;
            }

            int clientReadIndex = _clientReadIndexes[clientId];
            // Calculate the number of bytes to read before wrap-around
            int bytesToRead = Math.Min(bufferLength - clientReadIndex, availableBytes);

            // Copy data to the target buffer
            Memory<byte> bufferSlice = _buffer.Slice(clientReadIndex, bytesToRead);
            bufferSlice.CopyTo(target[bytesRead..]);
            bytesRead += bytesToRead;

            // Update the client's read index, wrapping around if necessary
            _clientReadIndexes[clientId] = (clientReadIndex + bytesToRead) % bufferLength;

            // Check if all requested data has been read
            if (bytesRead >= target.Length)
            {
                break;
            }

            //// Check and wait for more data if needed
            //if (GetAvailableBytes(clientId) == 0)
            //{
            //    await WaitForDataAvailability(clientId, cancellationToken);
            //    if (cancellationToken.IsCancellationRequested)
            //    {
            //        break;
            //    }
            //}
            _clientLastReadBeforeOverwrite[clientId] = _clientReadIndexes[clientId];
        }


        _statisticsManager.AddBytesRead(clientId, bytesRead);
        _logger.LogDebug("Finished ReadChunkMemory for clientId: {clientId}", clientId);

        return bytesRead;
    }

    private readonly object bufferLock = new();
    public void ResizeBuffer(int newSize)
    {
        lock (bufferLock) // Ensure exclusive access
        {
            Memory<byte> newBuffer = new byte[newSize];
            _buffer.CopyTo(newBuffer);

            //int dataLength = CalculateDataLength(); // Calculate the length of data in the old buffer

            //CopyDataToNewBuffer(newBuffer, dataLength); // Copy data to the new buffer

            // Update indices
            //_writeIndex = UpdateWriteIndex(dataLength);
            //foreach (Guid clientId in _clientReadIndexes.Keys)
            //{
            //    _clientReadIndexes[clientId] = UpdateReadIndex(clientId);
            //}

            // Replace old buffer with new buffer
            _buffer = newBuffer;
        }
    }

    public async Task WaitForDataAvailability(Guid clientId, CancellationToken cancellationToken)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        try
        {
            while (GetAvailableBytes(clientId) == 0 && !cancellationToken.IsCancellationRequested)
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
                //if (cancellationToken.IsCancellationRequested)
                //{
                //    _logger.LogWarning("Wait for data availability cancelled for client {ClientId}", clientId);
                //    break;
                //}
            }
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while waiting for data availability for client {ClientId}", clientId);
        }
        finally
        {
            stopwatch.Stop();

            // Assuming Prometheus metrics
            // Replace 'YourHistogram' with your actual Prometheus histogram object
            WaitTimeHistogram.WithLabels(Id.ToString(), clientId.ToString(), StreamInfo.VideoStreamName).Observe(stopwatch.Elapsed.TotalSeconds);
        }
    }


    public void RegisterClient(IClientStreamerConfiguration streamerConfiguration)
    {
        if (!_clientReadIndexes.ContainsKey(streamerConfiguration.ClientId))
        {
            _ = _clientReadIndexes.TryAdd(streamerConfiguration.ClientId, _oldestDataIndex);
            _statisticsManager.RegisterClient(streamerConfiguration.ClientId, streamerConfiguration.ClientUserAgent, streamerConfiguration.ClientIPAddress);
        }
        _logger.LogInformation("RegisterClient for ClientId: {ClientId} {VideoStreamName} {_oldestDataIndex}", streamerConfiguration.ClientId, StreamInfo.VideoStreamName, _oldestDataIndex);
    }

    private void NotifyClients()
    {
        DateTime now = DateTime.UtcNow;
        // Get the last notification time and update it to now
        DateTime lastNotificationTime = _lastNotificationTime;
        _lastNotificationTime = now;

        // Log the elapsed time if it's more than 500 milliseconds
        TimeSpan elapsed = now - lastNotificationTime;
        _dataArrivalHistogram.WithLabels(Id.ToString(), StreamInfo.VideoStreamName).Observe(elapsed.TotalMilliseconds);
        if (elapsed.TotalMilliseconds is > 15000 and < 60000000000000)
        {
            // Log the elapsed time here
            _logger.LogWarning($"Input stream is slow: {StreamInfo.VideoStreamName} {elapsed.TotalMilliseconds}ms elapsed since last set.");
        }

        foreach (Guid clientId in _clientReadIndexes.Keys)
        {
            OnDataAvailable(clientId);
        }
    }

    public void UnRegisterClient(Guid clientId)
    {
        _ = _clientReadIndexes.TryRemove(clientId, out _);
        _logger.LogInformation("UnRegisterClient for clientId: {clientId}  {VideoStreamName}", clientId, StreamInfo.VideoStreamName);
    }

    //public void UpdateReadIndex(Guid clientId, int newIndex)
    //{
    //    _clientReadIndexes[clientId] = newIndex % _buffer.Length;
    //}

    public int WriteChunk(Memory<byte> data)
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
                int availableSpace = _buffer.Length - _writeIndex;
                if (availableSpace == 0)
                {
                    _writeIndex = 0;
                    availableSpace = _buffer.Length;
                }

                int lengthToWrite = Math.Min(data.Length, availableSpace);

                Memory<byte> bufferSlice = _buffer.Slice(_writeIndex, lengthToWrite);
                data[..lengthToWrite].CopyTo(bufferSlice);

                if (!isBufferFull && _writeIndex + lengthToWrite >= _buffer.Length)
                {
                    isBufferFull = true;
                }

                _writeIndex = (_writeIndex + lengthToWrite) % _buffer.Length;
                bytesWritten += lengthToWrite;
                data = data[lengthToWrite..];

                // After updating _writeIndex
                if (HasOverwrittenOldestData(lengthToWrite))
                {
                    // Increment _oldestDataIndex to the next position after _writeIndex
                    _oldestDataIndex = (_writeIndex + 1) % _buffer.Length;
                }
                CheckAndReportClientOverwrites(initialWriteIndex, lengthToWrite);
            }
            BytesWrittenCounter.WithLabels(Id.ToString(), StreamInfo.VideoStreamName).Inc(bytesWritten);
            stopwatch.Stop();
            double seconds = stopwatch.Elapsed.TotalSeconds;
            double bps = bytesWritten / seconds;
            _bytesPerSecondHistogram.WithLabels(Id.ToString(), StreamInfo.VideoStreamName).Observe(bps);
            _bytesPerSecond.WithLabels(Id.ToString(), StreamInfo.VideoStreamName).Set(bps);
        }
        catch (Exception ex)
        {
            WriteErrorsCounter.WithLabels(Id.ToString(), StreamInfo.VideoStreamName).Inc(); // Increment write errors counter
            _logger.LogError(ex, "WriteChunk error occurred while writing chunk for {VideoStreamId}.", VideoStreamId);
        }
        finally
        {
            stopwatch.Stop();
            WriteDurationHistogram.WithLabels(Id.ToString(), StreamInfo.VideoStreamName).Observe(stopwatch.Elapsed.TotalMilliseconds);
        }

        _inputStreamStatistics.AddBytesWritten(bytesWritten);

        NotifyClients();

        _logger.LogDebug("WriteChunk completed with {VideoStreamId} count: {bytesWritten}", VideoStreamId, bytesWritten);

        return bytesWritten;
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
                ResizeBuffer();

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

    private int CalculateSafeReadIndex(int newWriteIndex)
    {
        // Implement logic to calculate a safe read index for the client
        // This might be the new write index, or some position before it, depending on your buffer's logic
        return newWriteIndex + 1;
    }

    private bool IsClientOverwritten(int clientReadIndex, int initialWriteIndex, int newWriteIndex)
    {

        // Check if the client's read index falls within the range of data that was overwritten
        if (initialWriteIndex <= newWriteIndex)
        {
            // No wrap-around
            bool a = clientReadIndex > initialWriteIndex;
            bool b = clientReadIndex <= newWriteIndex;
            return clientReadIndex > initialWriteIndex && clientReadIndex <= newWriteIndex;
        }
        else
        {
            // Wrap-around occurred
            bool a = clientReadIndex > initialWriteIndex;
            bool b = clientReadIndex <= newWriteIndex;
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

}
