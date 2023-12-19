using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMasterApplication.Common.Interfaces;
using StreamMasterApplication.Common.Models;

using StreamMasterDomain.Cache;
using StreamMasterDomain.Common;
using StreamMasterDomain.Dto;

using System.Collections.Concurrent;

namespace StreamMasterInfrastructure.VideoStreamManager.Buffers;

/// <summary>
/// Represents a circular ring buffer for streaming data.
/// </summary>
public sealed class CircularRingBuffer : ICircularRingBuffer
{
    //public event EventHandler<Guid> DataAvailable;
    private readonly IStatisticsManager _statisticsManager;
    private readonly IInputStatisticsManager _inputStatisticsManager;
    private readonly IInputStreamingStatistics _inputStreamStatistics;
    //private readonly object _semaphoreLock = new();
    public readonly StreamInfo StreamInfo;
    private readonly Memory<byte> _buffer;
    private readonly int _bufferSize;
    private readonly ConcurrentDictionary<Guid, int> _clientReadIndexes = new();
    //private readonly Dictionary<Guid, SemaphoreSlim> _clientSemaphores = [];
    private readonly ConcurrentDictionary<Guid, TaskCompletionSource<bool>> _readWaitHandles = new();

    private readonly ILogger<ICircularRingBuffer> _logger;
    private int _oldestDataIndex;
    private readonly float _preBuffPercent;
    private int _writeIndex;
    //private readonly int _waitDelayMS;
    private readonly object _writeLock = new();
    private readonly ReaderWriterLockSlim _readWriteLock = new ReaderWriterLockSlim();
    private bool isBufferFull = false; // This should be maintained as part of your buffer state

    public CircularRingBuffer(VideoStreamDto videoStreamDto, string channelName, IStatisticsManager statisticsManager, IInputStatisticsManager inputStatisticsManager, IMemoryCache memoryCache, int rank, ILogger<ICircularRingBuffer> logger, int? waitDelayMS = null)
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

        _bufferSize = setting.RingBufferSizeMB * 1024 * 1000;
        _preBuffPercent = setting.PreloadPercentage;

        StreamInfo = new StreamInfo
        {
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

    public Guid Id { get; } = Guid.NewGuid();
    public int BufferSize => _buffer.Length;
    public string VideoStreamId => StreamInfo.VideoStreamId;
    private bool InternalIsPreBuffered { get; set; } = false;

    private void OnDataAvailable(Guid clientId)
    {
        if (_readWaitHandles.TryGetValue(clientId, out var tcs))
        {
            tcs.TrySetResult(true);
        }
    }

    public List<StreamStatisticsResult> GetAllStatisticsForAllUrls()
    {
        List<StreamStatisticsResult> allStatistics = [];

        IInputStreamingStatistics input = GetInputStreamStatistics();

        foreach (ClientStreamingStatistics stat in _statisticsManager.GetAllClientStatisticsByClientIds(_clientReadIndexes.Keys))
        {
            allStatistics.Add(new StreamStatisticsResult
            {
                Id = Id.ToString(),
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
        if (_clientReadIndexes.TryGetValue(clientId, out int readIndex))
        {
            return (_writeIndex - readIndex + _buffer.Length) % _buffer.Length;
        }
        return 0;
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

        while (!IsPreBuffered())
        {
            await Task.Delay(50, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
        }

        if (!_clientReadIndexes.TryGetValue(clientId, out int readIndex))
        {
            // Handle this case: either log an error or throw an exception
            return 0; // or throw new Exception($"Client {clientId} not found");
        }

        int bytesToRead = Math.Min(target.Length, GetAvailableBytes(clientId));
        int bytesRead = 0;

        _readWriteLock.EnterReadLock();
        try
        {
            while (bytesToRead > 0)
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
                cancellationToken.ThrowIfCancellationRequested();
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


    public async Task WaitForDataAvailability(Guid clientId, CancellationToken cancellationToken)
    {
        // Check if data is already available before waiting
        if (GetAvailableBytes(clientId) > 0)
        {
            return;
        }

        // Create or get an existing TaskCompletionSource for this client
        var tcs = _readWaitHandles.GetOrAdd(clientId, id => new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously));

        // Register cancellation token to set the TaskCompletionSource as canceled
        using (cancellationToken.Register(() => tcs.TrySetCanceled()))
        {
            // Wait for the task to complete, which indicates data is available
            await tcs.Task.ConfigureAwait(false);

            // Reset the TaskCompletionSource for the next wait
            _readWaitHandles[clientId] = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
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
        foreach (var clientId in _clientReadIndexes.Keys)
        {
            OnDataAvailable(clientId);
        }
    }

    public void UnRegisterClient(Guid clientId)
    {
        _ = _clientReadIndexes.TryRemove(clientId, out _);
        //_ = _clientSemaphores.Remove(clientId);
        _statisticsManager.UnRegisterClient(clientId);

        _logger.LogInformation("UnRegisterClient for clientId: {clientId}  {VideoStreamName}", clientId, StreamInfo.VideoStreamName);
    }

    public void UpdateReadIndex(Guid clientId, int newIndex)
    {
        _clientReadIndexes[clientId] = newIndex % _buffer.Length;
    }

    public int WriteChunk(Memory<byte> data)
    {
        _logger.LogDebug("Starting WriteChunk {VideoStreamId} with count: {count}", VideoStreamId, data.Length);

        int bytesWritten = 0;

        _readWriteLock.EnterWriteLock();
        try
        {
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
        }
        finally
        {
            _readWriteLock.ExitWriteLock();
        }

        _inputStreamStatistics.AddBytesWritten(bytesWritten);
        try
        {
            NotifyClients();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while notifying clients during WriteChunk for {VideoStreamId}.", VideoStreamId);
        }

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
            if (effectiveWriteEnd < _writeIndex)
            {
                return _oldestDataIndex <= effectiveWriteEnd || _oldestDataIndex > _writeIndex;
            }
            else
            {
                return effectiveWriteEnd > _oldestDataIndex;
            }
        }
    }

}
