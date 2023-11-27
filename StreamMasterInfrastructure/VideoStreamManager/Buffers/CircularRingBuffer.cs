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
    private readonly Dictionary<Guid, SemaphoreSlim> _clientSemaphores = [];
    private readonly ILogger<ICircularRingBuffer> _logger;
    private int _oldestDataIndex;
    private readonly float _preBuffPercent;
    private int _writeIndex;

    public CircularRingBuffer(VideoStreamDto videoStreamDto, string channelName, IStatisticsManager statisticsManager, IInputStatisticsManager inputStatisticsManager, IMemoryCache memoryCache, int rank, ILogger<ICircularRingBuffer> logger)
    {
        Setting setting = memoryCache.GetSetting();

        _statisticsManager = statisticsManager ?? throw new ArgumentNullException(nameof(statisticsManager));
        _inputStatisticsManager = inputStatisticsManager ?? throw new ArgumentNullException(nameof(inputStatisticsManager));
        _inputStreamStatistics = _inputStatisticsManager.RegisterReader(videoStreamDto.Id);

        _logger = logger;
        if (setting.PreloadPercentage < 0 || setting.PreloadPercentage > 100)
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

    public async Task<byte> Read(Guid clientId, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Starting Read for {VideoStreamId} clientId: {clientId}", VideoStreamId, clientId);

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
            await Task.Delay(100, cancellationToken).ConfigureAwait(false);
            cancellationToken.ThrowIfCancellationRequested();
        }

        if (!_clientReadIndexes.TryGetValue(clientId, out int readIndex))
        {
            _logger.LogWarning("Client {clientId} not found in ReadChunk", clientId);
            return 0; // Or throw an exception
        }

        int bytesRead = 0;
        while (bytesRead < count)
        {
            cancellationToken.ThrowIfCancellationRequested();

            int availableToRead = Math.Min(count - bytesRead, _buffer.Length - readIndex);
            if (availableToRead == 0)
            {
                // Handle wrap-around
                readIndex = 0;
                continue;
            }

            _buffer.Slice(readIndex, availableToRead).Span.CopyTo(buffer.AsSpan(offset + bytesRead));
            bytesRead += availableToRead;
            readIndex = (readIndex + availableToRead) % _buffer.Length;
        }

        _clientReadIndexes[clientId] = readIndex;

        _statisticsManager.AddBytesRead(clientId, bytesRead);
        _logger.LogDebug("Finished ReadChunk for clientId: {clientId} with bytes read: {bytesRead}", clientId, bytesRead);

        return bytesRead;
    }

    public async Task<int> ReadChunkMemory(Guid clientId, Memory<byte> target, CancellationToken cancellationToken)
    {
        // _logger.LogInformation("Starting ReadChunkMemory for clientId: {clientId}", clientId);

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
        while (bytesToRead > 0)
        {
            cancellationToken.ThrowIfCancellationRequested();
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
        if (!_clientReadIndexes.ContainsKey(clientId))
        {
            _ = _clientReadIndexes.TryAdd(clientId, _oldestDataIndex);
            _ = _clientSemaphores.TryAdd(clientId, new SemaphoreSlim(0, 1));
            _statisticsManager.RegisterClient(clientId, clientAgent, clientIPAddress);
        }
        _logger.LogInformation("RegisterClient for Circular.Id: {Id} ClientId: {ClientId} {VideoStreamName} {_oldestDataIndex}", Id, clientId, StreamInfo.VideoStreamName, _oldestDataIndex);
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

    public void UnRegisterClient(Guid clientId)
    {
        _ = _clientReadIndexes.TryRemove(clientId, out _);
        _ = _clientSemaphores.Remove(clientId);
        _statisticsManager.UnRegisterClient(clientId);

        _logger.LogInformation("UnRegisterClient for clientId: {clientId}  {VideoStreamName}", clientId, StreamInfo.VideoStreamName);
    }

    public void UpdateReadIndex(Guid clientId, int newIndex)
    {
        _clientReadIndexes[clientId] = newIndex % _buffer.Length;
    }

    public async Task WaitSemaphoreAsync(Guid clientId, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            _logger.LogDebug("Exiting WaitSemaphoreAsync early due to CancellationToken cancellation request for clientId: {clientId}", clientId);
            return;
        }

        if (_clientSemaphores.TryGetValue(clientId, out SemaphoreSlim? semaphore))
        {
            await semaphore.WaitAsync(1000, cancellationToken);
            _logger.LogDebug("WaitSemaphoreAsync for clientId: {clientId}", clientId);
        }
        else
        {
            _logger.LogDebug("Exiting WaitSemaphoreAsync early due to clientId not registered: {clientId}", clientId);
        }
    }

    public void Write(byte data)
    {
        _logger.LogDebug("Starting Write {VideoStreamId}", VideoStreamId);

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
            _logger.LogError(ex, "An error occurred while releasing semaphores during Write {VideoStreamId}", VideoStreamId);
        }

        _logger.LogDebug("Starting WriteChunk {VideoStreamId} with count: {count}", VideoStreamId, data);
    }

    public int WriteChunk(Memory<byte> data)
    {
        _logger.LogDebug("Starting WriteChunk {VideoStreamId} with count: {count}", VideoStreamId, data.Length);

        int bytesWritten = 0;

        while (data.Length > 0)
        {
            int availableSpace = _buffer.Length - _writeIndex;
            if (availableSpace == 0)
            {
                // Handle buffer wrap around
                _writeIndex = 0;
                availableSpace = _buffer.Length;
            }

            int lengthToWrite = Math.Min(data.Length, availableSpace);

            try
            {
                Memory<byte> bufferSlice = _buffer.Slice(_writeIndex, lengthToWrite);
                data[..lengthToWrite].CopyTo(bufferSlice);
                _writeIndex = (_writeIndex + lengthToWrite) % _buffer.Length;
                bytesWritten += lengthToWrite;
                data = data[lengthToWrite..];
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error writing chunk in WriteChunk for {VideoStreamId}.", VideoStreamId);
                throw;
            }
        }

        _inputStreamStatistics.AddBytesWritten(bytesWritten);

        try
        {
            ReleaseSemaphores();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while releasing semaphores during WriteChunk for {VideoStreamId}.", VideoStreamId);
        }

        _logger.LogDebug("WriteChunk completed with {VideoStreamId} count: {data.Length}", VideoStreamId, data.Length);

        return bytesWritten;
    }

    public int WriteChunk(byte[] data, int count)
    {
        _logger.LogDebug("Starting WriteChunk {VideoStreamId} with count: {count}", VideoStreamId, count);

        int bytesWritten = 0;
        int dataIndex = 0;

        while (dataIndex < count)
        {
            int nextWriteIndex = (_writeIndex + 1) % _buffer.Length;

            if (nextWriteIndex == _oldestDataIndex)
            {
                _oldestDataIndex = (_oldestDataIndex + 1) % _buffer.Length;
            }

            int availableSpace = _buffer.Length - _writeIndex;
            if (availableSpace == 0)
            {
                // Handle buffer wrap around
                _writeIndex = 0;
                availableSpace = _buffer.Length;
            }

            int lengthToWrite = Math.Min(count - dataIndex, availableSpace);

            Span<byte> bufferSpan = _buffer.Span.Slice(_writeIndex, lengthToWrite);
            Span<byte> dataSpan = new(data, dataIndex, lengthToWrite);
            dataSpan.CopyTo(bufferSpan);

            _writeIndex = (_writeIndex + lengthToWrite) % _buffer.Length;
            dataIndex += lengthToWrite;
            bytesWritten += lengthToWrite;
        }

        _inputStreamStatistics.AddBytesWritten(bytesWritten);

        try
        {
            ReleaseSemaphores();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while releasing semaphores during WriteChunk for {VideoStreamId}.", VideoStreamId);
        }

        _logger.LogDebug("WriteChunk completed with {VideoStreamId} count: {data.Length}", VideoStreamId, data.Length);

        return bytesWritten;
    }
    public float GetBufferUtilization()
    {
        int dataInBuffer = (_writeIndex - _oldestDataIndex + _buffer.Length) % _buffer.Length;
        return (float)dataInBuffer / _buffer.Length * 100;
    }
}
