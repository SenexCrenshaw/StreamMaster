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
public class CircularRingBuffer : ICircularRingBuffer
{
    private readonly IStatisticsManager _statisticsManager;
    private readonly IInputStatisticsManager _inputStatisticsManager;
    private readonly IInputStreamingStatistics _inputStreamStatistics;
    private readonly object _semaphoreLock = new();
    public readonly StreamInfo StreamInfo;
    private readonly Memory<byte> _buffer;
    private readonly int _bufferSize;
    private readonly ConcurrentDictionary<Guid, int> _clientReadIndexes = new();
    private readonly Dictionary<Guid, SemaphoreSlim> _clientSemaphores = new();
    private readonly ILogger<ICircularRingBuffer> _logger;
    private int _oldestDataIndex;
    private readonly float _preBuffPercent;
    private int _writeIndex;

    public CircularRingBuffer(ChildVideoStreamDto childVideoStreamDto, IStatisticsManager statisticsManager, IInputStatisticsManager inputStatisticsManager, IMemoryCache memoryCache, int rank, ILogger<ICircularRingBuffer> logger)
    {
        Setting setting = memoryCache.GetSetting();

        _statisticsManager = statisticsManager ?? throw new ArgumentNullException(nameof(statisticsManager));
        _inputStatisticsManager = inputStatisticsManager ?? throw new ArgumentNullException(nameof(inputStatisticsManager));
        _inputStreamStatistics = _inputStatisticsManager.RegisterReader(childVideoStreamDto.Id);

        _logger = logger;
        if (setting.PreloadPercentage < 0 || setting.PreloadPercentage > 100)
        {
            setting.PreloadPercentage = 0;
        }

        _bufferSize = setting.RingBufferSizeMB * 1024 * 1000;
        _preBuffPercent = setting.PreloadPercentage;

        StreamInfo = new StreamInfo
        {
            VideoStreamId = childVideoStreamDto.Id,
            VideoStreamName = childVideoStreamDto.User_Tvg_name,
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
    private bool InternalIsPreBuffered { get; set; } = false;

    public List<StreamStatisticsResult> GetAllStatisticsForAllUrls()
    {
        List<StreamStatisticsResult> allStatistics = new();

        IInputStreamingStatistics input = GetInputStreamStatistics();

        foreach (ClientStreamingStatistics stat in _statisticsManager.GetAllClientStatistics())
        {
            allStatistics.Add(new StreamStatisticsResult
            {
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
        _logger.LogDebug("Starting IsPreBuffered");

        if (InternalIsPreBuffered)
        {
            _logger.LogDebug("Finished IsPreBuffered with true (already pre-buffered)");
            return true;
        }

        int dataInBuffer = (_writeIndex - _oldestDataIndex + _buffer.Length) % _buffer.Length;
        float percentBuffered = (float)dataInBuffer / _buffer.Length * 100;

        InternalIsPreBuffered = percentBuffered >= _preBuffPercent;

        _logger.LogDebug("Finished IsPreBuffered with {isPreBuffered}", InternalIsPreBuffered);
        return InternalIsPreBuffered;
    }

    public async Task<byte> Read(Guid clientId, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Starting Read for clientId: {clientId}", clientId);

        while (!IsPreBuffered())
        {
            await Task.Delay(50, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
        }

        int readIndex = _clientReadIndexes[clientId];
        byte data = _buffer.Span[readIndex];
        _clientReadIndexes[clientId] = (readIndex + 1) % _buffer.Length;

        _statisticsManager.IncrementBytesRead(clientId);

        _logger.LogDebug("Finished Read for clientId: {clientId}", clientId);
        return data;
    }

    public async Task<int> ReadChunk(Guid clientId, byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Starting ReadChunk for clientId: {clientId}", clientId);

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

        _statisticsManager.AddBytesRead(clientId, count);
        _logger.LogDebug("Finished ReadChunk for clientId: {clientId}", clientId);

        return count;
    }

    public async Task<int> ReadChunkMemory(Guid clientId, Memory<byte> target, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Starting ReadChunkMemory for clientId: {clientId}", clientId);

        while (!IsPreBuffered())
        {
            await Task.Delay(100, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
        }

        if (!_clientReadIndexes.TryGetValue(clientId, out int readIndex))
        {
            // Handle this case: either log an error or throw an exception
            return 0; // or throw new Exception($"Client {clientId} not found");
        }

        int bytesToRead = Math.Min(target.Length, GetAvailableBytes(clientId));
        int bytesRead = 0;
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
        }

        _clientReadIndexes[clientId] = readIndex;

        _statisticsManager.AddBytesRead(clientId, target.Length);
        _logger.LogDebug("Finished ReadChunkMemory for clientId: {clientId}", clientId);

        return target.Length;
    }

    public void RegisterClient(Guid clientId, string clientAgent, string clientIPAddress)
    {
        _logger.LogDebug("Starting RegisterClient for clientId: {clientId} {_oldestDataIndex}", clientId, _oldestDataIndex);

        if (!_clientReadIndexes.ContainsKey(clientId))
        {
            _ = _clientReadIndexes.TryAdd(clientId, _oldestDataIndex);
            _ = _clientSemaphores.TryAdd(clientId, new SemaphoreSlim(0, 1));
            _statisticsManager.RegisterClient(clientId, clientAgent, clientIPAddress);
        }

        _logger.LogDebug("Finished RegisterClient for clientId: {clientId}", clientId);
    }

    private void ReleaseSemaphores()
    {
        lock (_semaphoreLock)
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
    }

    public void UnregisterClient(Guid clientId)
    {
        _logger.LogDebug("Starting UnregisterClient for clientId: {clientId}", clientId);

        _ = _clientReadIndexes.TryRemove(clientId, out _);
        _ = _clientSemaphores.Remove(clientId);
        _statisticsManager.UnregisterClient(clientId);

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
        _logger.LogDebug("Starting Write with data: {data}", data);

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
            ReleaseSemaphores();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while releasing semaphores during Write.");
        }

        _logger.LogDebug("Write completed with data: {data}", data);
    }

    public int WriteChunk(Memory<byte> data)
    {
        _logger.LogDebug("Starting WriteChunk with count: {count}", data.Length);

        int bytesWritten = 0;
        Span<byte> dataSpan = data.Span;

        for (int i = 0; i < data.Length; i++)
        {
            int nextWriteIndex = (_writeIndex + 1) % _buffer.Length;

            if (nextWriteIndex == _oldestDataIndex)
            {
                _oldestDataIndex = (_oldestDataIndex + 1) % _buffer.Length;
            }

            _buffer.Span[_writeIndex] = dataSpan[i];
            _writeIndex = nextWriteIndex;
            bytesWritten++;
        }

        _inputStreamStatistics.AddBytesWritten(bytesWritten);

        try
        {
            ReleaseSemaphores();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while releasing semaphores during WriteChunk.");
        }

        _logger.LogDebug("WriteChunk completed with count: {data.Length}", data.Length);

        return bytesWritten;
    }

    public int WriteChunk(byte[] data, int count)
    {
        _logger.LogDebug("Starting WriteChunk with count: {count}", count);

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
            ReleaseSemaphores();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while releasing semaphores during WriteChunk.");
        }

        _logger.LogDebug("WriteChunk completed with count: {count}", count);

        return bytesWritten;
    }

    public float GetBufferUtilization()
    {
        int dataInBuffer = (_writeIndex - _oldestDataIndex + _buffer.Length) % _buffer.Length;
        return (float)dataInBuffer / _buffer.Length * 100;
    }
}
