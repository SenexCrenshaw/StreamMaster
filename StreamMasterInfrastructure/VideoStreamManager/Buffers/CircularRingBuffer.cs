using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using Prometheus;

using StreamMasterApplication.Common.Interfaces;
using StreamMasterApplication.Common.Models;

using StreamMasterDomain.Cache;
using StreamMasterDomain.Common;
using StreamMasterDomain.Dto;

using System.Collections.Concurrent;
using System.Diagnostics;

namespace StreamMasterInfrastructure.VideoStreamManager.Buffers;

/// <summary>
/// Represents a circular ring buffer for streaming data.
/// </summary>
public sealed class CircularRingBuffer : ICircularRingBuffer
{
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

    private readonly Histogram BytesWrittenHistogram = Metrics.CreateHistogram(
    "circular_buffer_bytes_written_histogram",
    "Histogram of bytes written.",
    new HistogramConfiguration
    {
        Buckets = Histogram.LinearBuckets(0, 20000000, 11),
        LabelNames = ["circular_buffer_id", "video_stream_name"]
    });

    private readonly Histogram WriteDurationHistogram = Metrics.CreateHistogram(
       "circular_buffer_write_chunk_duration_milliseconds",
       "Histogram for the duration of the WriteChunk operation.",
       new HistogramConfiguration
       {
           Buckets = Histogram.LinearBuckets(0, 100, 100),
           LabelNames = ["circular_buffer_id", "video_stream_name"]
       });



    private readonly Counter BytesWrittenCounter = Metrics.CreateCounter(
        "circular_buffer_bytes_written_total",
        "Total number of bytes written.",
        new CounterConfiguration
        {
            LabelNames = ["circular_buffer_id", "video_stream_name"]
        });

    private readonly Counter WriteErrorsCounter = Metrics.CreateCounter(
        "circular_buffer_write_errors_total",
        "Total number of write errors.",
         new CounterConfiguration
         {
             LabelNames = ["circular_buffer_id", "video_stream_name"] // Add the additional label here
         });

    private readonly Histogram _dataArrivalHistogram = Metrics.CreateHistogram(
    "circular_buffer_arrival_time_milliseconds",
    "Histogram of data arrival times in milliseconds.",
        new HistogramConfiguration
        {
            Buckets = Histogram.LinearBuckets(start: 0.01, width: 0.01, count: 500), // Adjust the buckets to your needs
            LabelNames = ["circular_buffer_id", "video_stream_name"]
        });

    private readonly IStatisticsManager _statisticsManager;
    private readonly IInputStatisticsManager _inputStatisticsManager;
    private readonly IInputStreamingStatistics _inputStreamStatistics;
    public readonly StreamInfo StreamInfo;
    private readonly Memory<byte> _buffer;
    private readonly int _bufferSize;
    private readonly ConcurrentDictionary<Guid, int> _clientReadIndexes = new();

    private readonly ConcurrentDictionary<Guid, ManualResetEventSlim> _readWaitHandles = new();
    private DateTime _lastNotificationTime = new();

    public VideoInfo? VideoInfo { get; set; } = null;

    private readonly ILogger<ICircularRingBuffer> _logger;
    private int _oldestDataIndex;
    private readonly float _preBuffPercent;
    private int _writeIndex;
    private readonly ReaderWriterLockSlim _readWriteLock = new();
    private bool isBufferFull = false;

    public CircularRingBuffer(VideoStreamDto videoStreamDto, string channelId, string channelName, IStatisticsManager statisticsManager, IInputStatisticsManager inputStatisticsManager, IMemoryCache memoryCache, int rank, ILogger<ICircularRingBuffer> logger)
    {
        Setting setting = memoryCache.GetSetting();

        //_waitDelayMS = waitDelayMS != null ? (int)waitDelayMS : (setting.MaxConnectRetry + 1) * setting.MaxConnectRetryTimeMS;

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
            StreamProxyType = videoStreamDto.StreamProxyType,
            StreamUrl = videoStreamDto.User_Url,

            Rank = rank
        };

        _buffer = new byte[_bufferSize];
        _writeIndex = 0;
        _oldestDataIndex = 0;
        logger.LogInformation("New Circular Buffer {Id} for stream {videoStreamId} {name}", Id, videoStreamDto.Id, videoStreamDto.User_Tvg_name);
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
                M3UStreamProxyType = StreamInfo.StreamProxyType,
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

    //public static double CalculateConsumptionTime(int packetSizeKB, double bitrateMbps)
    //{
    //    // Convert bitrate from Mbps to kilobytes per second (KB/s)
    //    // Note: 1 byte = 8 bits, 1 megabit = 1000 kilobits
    //    double bitrateKBps = bitrateMbps * 1000 / 8;

    //    // Calculate the time to consume the packet in seconds
    //    double timeInSeconds = packetSizeKB / bitrateKBps;

    //    // Convert the time to milliseconds
    //    return timeInSeconds * 1000;
    //}

    //private readonly int waitMs = 50;

    public async Task<int> ReadChunkMemory(Guid clientId, Memory<byte> target, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Starting ReadChunkMemory for clientId: {clientId}", clientId);

        while (!IsPreBuffered() && !cancellationToken.IsCancellationRequested)
        {
            await Task.Delay(10, cancellationToken);
        }

        if (!_clientReadIndexes.TryGetValue(clientId, out int readIndex))
        {
            // Handle this case: either log an error or throw an exception
            return 0; // or throw new Exception($"Client {clientId} not found");
        }

        int bytesToRead = Math.Min(target.Length, GetAvailableBytes(clientId));
        int bytesRead = 0;


        try
        {
            _readWriteLock.EnterReadLock();
            while (!cancellationToken.IsCancellationRequested && bytesToRead > 0)
            {
                // Calculate how much we can read before we have to wrap
                int canRead = Math.Min(bytesToRead, _buffer.Length - readIndex);

                // Create a slice from the readIndex to the end of what we can read
                Memory<byte> slice = _buffer.Slice(readIndex, canRead);

                // Copy to the target buffer
                slice.CopyTo(target.Slice(bytesRead, canRead));

                // Update readIndex and bytesToRead
                readIndex = (readIndex + canRead) % _buffer.Length;
                bytesRead += canRead;
                bytesToRead -= canRead;
            }

            _clientReadIndexes[clientId] = readIndex;
        }
        finally
        {
            _readWriteLock.ExitReadLock();
        }

        _statisticsManager.AddBytesRead(clientId, bytesRead);
        _logger.LogDebug("Finished ReadChunkMemory for clientId: {clientId}", clientId);

        return bytesRead;
    }

    public void WaitForDataAvailability(Guid clientId, CancellationToken cancellationToken)
    {
        ManualResetEventSlim waitHandle = _readWaitHandles.GetOrAdd(clientId, _ => new ManualResetEventSlim(false));

        using (CancellationTokenSource linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
        {
            linkedCts.CancelAfter(TimeSpan.FromSeconds(5));

            try
            {
                while (GetAvailableBytes(clientId) == 0 && !linkedCts.Token.IsCancellationRequested)
                {
                    // Wait directly on the ManualResetEventSlim
                    if (waitHandle.WaitHandle.WaitOne(TimeSpan.FromMilliseconds(25)))
                    {
                        break; // Exit the loop if the waitHandle is set
                    }
                }
            }
            catch (OperationCanceledException)
            {
                if (linkedCts.Token.IsCancellationRequested)
                {
                    _logger.LogError("WaitForDataAvailability timed out for client ID {clientId}", clientId);
                }
                else if (cancellationToken.IsCancellationRequested)
                {
                    _logger.LogError("WaitForDataAvailability was canceled for client ID {clientId}", clientId);
                }
                // Additional handling if necessary
            }
        }

        waitHandle.Reset();
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

        foreach (Guid clientId in _readWaitHandles.Keys)
        {
            if (_readWaitHandles.TryGetValue(clientId, out ManualResetEventSlim? waitHandle))
            {
                waitHandle.Set();
            }
        }
    }

    public void UnRegisterClient(Guid clientId)
    {
        _ = _clientReadIndexes.TryRemove(clientId, out _);
        _logger.LogInformation("UnRegisterClient for clientId: {clientId}  {VideoStreamName}", clientId, StreamInfo.VideoStreamName);
    }

    public void UpdateReadIndex(Guid clientId, int newIndex)
    {
        _clientReadIndexes[clientId] = newIndex % _buffer.Length;
    }

    public int WriteChunk(Memory<byte> data)
    {
        Stopwatch stopwatch = new();
        stopwatch.Start();

        _logger.LogDebug("Starting WriteChunk {VideoStreamId} with count: {count}", VideoStreamId, data.Length);

        int bytesWritten = 0;


        try
        {
            _readWriteLock.EnterWriteLock();
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
            WriteDurationHistogram.WithLabels(Id.ToString(), StreamInfo.VideoStreamName).Observe(stopwatch.Elapsed.TotalMilliseconds); // Record the duration
            _readWriteLock.ExitWriteLock();
        }

        _inputStreamStatistics.AddBytesWritten(bytesWritten);

        NotifyClients();

        _logger.LogDebug("WriteChunk completed with {VideoStreamId} count: {bytesWritten}", VideoStreamId, bytesWritten);

        return bytesWritten;
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
