using Microsoft.Extensions.Logging;

using StreamMasterApplication.Common.Interfaces;
using StreamMasterApplication.Common.Models;

using StreamMasterDomain.Common;
using StreamMasterDomain.Dto;

using System.Collections.Concurrent;

namespace StreamMasterInfrastructure.VideoStreamManager;

/// <summary>
/// Represents a circular ring buffer for streaming data.
/// </summary>
public class CircularRingBuffer : ICircularRingBuffer
{
    public readonly StreamInfo StreamInfo;
    private readonly Memory<byte> _buffer;
    private readonly int _bufferSize;
    private readonly ConcurrentDictionary<Guid, int> _clientReadIndexes = new();
    private readonly Dictionary<Guid, SemaphoreSlim> _clientSemaphores = new();
    private readonly Dictionary<Guid, StreamingStatistics> _clientStatistics = new();
    private readonly StreamingStatistics _inputStreamStatistics = new("Unknown");
    private readonly ILogger<CircularRingBuffer> _logger;
    private int _oldestDataIndex;
    private float _preBuffPercent;
    private int _writeIndex;

    public CircularRingBuffer(ChildVideoStreamDto childVideoStreamDto, string videoStreamId, string videoStreamName, int rank, ILogger<CircularRingBuffer> logger)
    {
        _logger = logger;
        if (setting.PreloadPercentage < 0 || setting.PreloadPercentage > 100)
            setting.PreloadPercentage = 0;

        _bufferSize = setting.RingBufferSizeMB * 1024 * 1000;
        _preBuffPercent = setting.PreloadPercentage;

        StreamInfo = new StreamInfo
        {
            VideoStreamId = videoStreamId,
            VideoStreamName = videoStreamName,
            M3UStreamId = childVideoStreamDto.Id,
            M3UStreamName = childVideoStreamDto.User_Tvg_name,
            Logo = childVideoStreamDto.User_Tvg_logo,
            StreamProxyType = childVideoStreamDto.StreamProxyType,
            StreamUrl = childVideoStreamDto.User_Url,
            Rank = rank
        };

        _buffer = new byte[_bufferSize];
        _writeIndex = 0;
        _oldestDataIndex = 0;
    }

    public int BufferSize => _buffer.Length;
    public string VideoStreamId => StreamInfo.VideoStreamId;
    private bool isPreBuffered { get; set; } = false;
    private Setting setting => FileUtil.GetSetting();

    public List<ClientStreamingStatistics> GetAllStatistics()
    {
        List<ClientStreamingStatistics> statisticsList = new();

        foreach (KeyValuePair<Guid, StreamingStatistics> entry in _clientStatistics)
        {
            statisticsList.Add(new ClientStreamingStatistics(entry.Value.ClientAgent)
            {
                ClientId = entry.Key,
                BytesRead = entry.Value.BytesRead,
                BytesWritten = entry.Value.BytesWritten,
                StartTime = entry.Value.StartTime,
            });
        }

        return statisticsList;
    }

    public List<StreamStatisticsResult> GetAllStatisticsForAllUrls()
    {
        List<StreamStatisticsResult> allStatistics = new();

        StreamingStatistics input = GetInputStreamStatistics();

        foreach (ClientStreamingStatistics stat in GetAllStatistics())
        {
            allStatistics.Add(new StreamStatisticsResult
            {
                VideoStreamId = StreamInfo.VideoStreamId,
                VideoStreamName = StreamInfo.VideoStreamName,
                M3UStreamId = StreamInfo.M3UStreamId,
                M3UStreamName = StreamInfo.M3UStreamName,
                M3UStreamProxyType = StreamInfo.StreamProxyType,
                Logo = StreamInfo.Logo,
                Rank = StreamInfo.Rank,
                InputBytesRead = input.BytesRead,
                InputBytesWritten = input.BytesWritten,
                InputBitsPerSecond = input.BitsPerSecond,
                InputStartTime = input.StartTime,

                StreamUrl = StreamInfo.StreamUrl,

                ClientBitsPerSecond = stat.BitsPerSecond,
                ClientBytesRead = stat.BytesRead,
                ClientBytesWritten = stat.BytesWritten,
                ClientId = stat.ClientId,
                ClientStartTime = stat.StartTime,
                ClientAgent = stat.ClientAgent,
            });
        }

        return allStatistics;
    }

    public int GetAvailableBytes(Guid clientId)
    {
        if (!_clientReadIndexes.ContainsKey(clientId))
        {
            return 0;
        }

        int readIndex = _clientReadIndexes[clientId];
        return (_writeIndex - readIndex + _buffer.Length) % _buffer.Length;
    }

    public ICollection<Guid> GetClientIds()
    {
        return _clientReadIndexes.Keys;
    }

    public StreamingStatistics? GetClientStatistics(Guid clientId)
    {
        return _clientStatistics.TryGetValue(clientId, out StreamingStatistics? clientStats) ? clientStats : null;
    }

    public StreamingStatistics GetInputStreamStatistics()
    {
        return _inputStreamStatistics;
    }

    public int GetReadIndex(Guid clientId)
    {
        return _clientReadIndexes[clientId];
    }

    public SingleStreamStatisticsResult GetSingleStreamStatisticsResult()
    {
        return new SingleStreamStatisticsResult
        {
            StreamUrl = StreamInfo.StreamUrl,
            ClientStatistics = GetAllStatistics()
        };
    }

    public bool IsPreBuffered()
    {
        _logger.LogDebug($"Starting IsPreBuffered");

        if (isPreBuffered)
        {
            _logger.LogDebug($"Finished IsPreBuffered with true (already pre-buffered)");
            return true;
        }

        int dataInBuffer = (_writeIndex - _oldestDataIndex + _buffer.Length) % _buffer.Length;
        float percentBuffered = (float)dataInBuffer / _buffer.Length * 100;

        isPreBuffered = percentBuffered >= _preBuffPercent;

        _logger.LogDebug($"Finished IsPreBuffered with {isPreBuffered}");
        return isPreBuffered;
    }

    public async Task<byte> Read(Guid clientId, CancellationToken cancellationToken)
    {
        _logger.LogDebug($"Starting Read for clientId: {clientId}");

        while (!IsPreBuffered())
        {
            await Task.Delay(50, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
        }

        int readIndex = _clientReadIndexes[clientId];
        byte data = _buffer.Span[readIndex];
        _clientReadIndexes[clientId] = (readIndex + 1) % _buffer.Length;

        if (_clientStatistics.TryGetValue(clientId, out StreamingStatistics? clientStats))
        {
            clientStats.IncrementBytesRead();
        }

        _logger.LogDebug($"Finished Read for clientId: {clientId}");
        return data;
    }

    public async Task<int> ReadChunk(Guid clientId, byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        _logger.LogDebug($"Starting ReadChunk for clientId: {clientId}");

        while (!IsPreBuffered())
        {
            await Task.Delay(100, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
        }

        int readIndex = _clientReadIndexes[clientId];

        for (int i = 0; i < count && !cancellationToken.IsCancellationRequested; i++)
        {
            buffer[offset + i] = _buffer.Span[readIndex];
            readIndex = (readIndex + 1) % _buffer.Length;
        }

        _clientReadIndexes[clientId] = readIndex;

        // Update client statistics
        if (_clientStatistics.TryGetValue(clientId, out StreamingStatistics? clientStats))
        {
            clientStats.AddBytesRead(count);
        }
        _logger.LogDebug("Finished ReadChunk for clientId: {clientId}", clientId);

        return count;
    }

    public void RegisterClient(Guid clientId, string clientAgent)
    {
        _logger.LogDebug("Starting RegisterClient for clientId: {clientId}", clientId);

        if (!_clientReadIndexes.ContainsKey(clientId))
        {
            _ = _clientReadIndexes.TryAdd(clientId, _oldestDataIndex);
            _ = _clientSemaphores.TryAdd(clientId, new SemaphoreSlim(0, 1));
            _ = _clientStatistics.TryAdd(clientId, new StreamingStatistics(clientAgent));
        }

        _logger.LogDebug("Finished RegisterClient for clientId: {clientId}", clientId);
    }

    public void ReleaseSemaphore(Guid clientId)
    {
        SemaphoreSlim semaphore = _clientSemaphores[clientId];
        _ = semaphore.Release();
    }

    public void UnregisterClient(Guid clientId)
    {
        _logger.LogDebug("Starting UnregisterClient for clientId: {clientId}", clientId);

        _ = _clientReadIndexes.TryRemove(clientId, out _);
        _ = _clientSemaphores.Remove(clientId);
        _ = _clientStatistics.Remove(clientId, out _);

        _logger.LogDebug("Finished UnregisterClient for clientId: {clientId}", clientId);
    }

    public void UpdateReadIndex(Guid clientId, int newIndex)
    {
        _clientReadIndexes[clientId] = newIndex % _buffer.Length;
    }

    public async Task WaitSemaphoreAsync(Guid clientId, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Starting WaitSemaphoreAsync for clientId: {clientId}", clientId);

        if (cancellationToken.IsCancellationRequested)
        {
            _logger.LogDebug("Exiting WaitSemaphoreAsync early due to CancellationToken cancellation request for clientId: {clientId}", clientId);
            return;
        }

        if (!_clientSemaphores.ContainsKey(clientId))
        {
            _logger.LogDebug("Exiting WaitSemaphoreAsync early due to clientId not registered: {clientId}", clientId);
            return;
        }

        SemaphoreSlim semaphore = _clientSemaphores[clientId];
        await semaphore.WaitAsync(50, cancellationToken);

        _logger.LogDebug("Exiting WaitSemaphoreAsync for clientId: {clientId}", clientId);
    }

    public void Write(byte data)
    {
        _logger.LogDebug($"Starting Write with data: {data}");

        int nextWriteIndex = (_writeIndex + 1) % _buffer.Length;

        if (nextWriteIndex == _oldestDataIndex)
        {
            _oldestDataIndex = (_oldestDataIndex + 1) % _buffer.Length;
        }

        _buffer.Span[_writeIndex] = data;
        _writeIndex = nextWriteIndex;

        _inputStreamStatistics.IncrementBytesWritten();

        try
        {
            foreach (KeyValuePair<Guid, SemaphoreSlim> kvp in _clientSemaphores)
            {
                SemaphoreSlim semaphore = kvp.Value;

                if (semaphore.CurrentCount == 0)
                {
                    semaphore.Release();
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while releasing semaphores during Write.");
        }

        _logger.LogDebug($"Write completed with data: {data}");
    }

    public int WriteChunk(byte[] data, int count)
    {
        _logger.LogDebug($"Starting WriteChunk with count: {count}");

        int bytesWritten = 0;

        for (int i = 0; i < count; i++)
        {
            int nextWriteIndex = (_writeIndex + 1) % _buffer.Length;

            if (nextWriteIndex == _oldestDataIndex)
            {
                _oldestDataIndex = (_oldestDataIndex + 1) % _buffer.Length;
            }

            _buffer.Span[_writeIndex] = data[i];
            _writeIndex = nextWriteIndex;
            bytesWritten++;
        }

        _inputStreamStatistics.AddBytesWritten(bytesWritten);

        try
        {
            foreach (KeyValuePair<Guid, SemaphoreSlim> kvp in _clientSemaphores)
            {
                SemaphoreSlim semaphore = kvp.Value;

                if (semaphore.CurrentCount == 0)
                {
                    semaphore.Release();
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while releasing semaphores during WriteChunk.");
        }

        _logger.LogDebug($"WriteChunk completed with count: {count}");

        return bytesWritten;
    }

    public float GetBufferUtilization()
    {
        int dataInBuffer = (_writeIndex - _oldestDataIndex + _buffer.Length) % _buffer.Length;
        return (float)dataInBuffer / _buffer.Length * 100;
    }
}
