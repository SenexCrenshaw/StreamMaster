using StreamMasterApplication.Common.Interfaces;
using StreamMasterApplication.Common.Models;

using StreamMasterDomain.Common;
using StreamMasterDomain.Dto;

using System.Collections.Concurrent;

namespace StreamMasterInfrastructure.MiddleWare;

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
    private int _oldestDataIndex;
    private float _preBuffPercent;
    private int _writeIndex;

    public CircularRingBuffer(ChildVideoStreamDto childVideoStreamDto, int videoStreamId, string videoStreamName, int rank, int tempbuffersize = 0)
    {
        if (setting.PreloadPercentage < 0 || setting.PreloadPercentage > 100)
            setting.PreloadPercentage = 0;

        _bufferSize = tempbuffersize > 0 ? tempbuffersize : setting.RingBufferSizeMB * 1024 * 1000;
        _preBuffPercent = setting.PreloadPercentage;

        StreamInfo = new StreamInfo
        {
            VideoStreamId = videoStreamId,
            VideoStreamName= videoStreamName,
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
    public int VideoStreamId => StreamInfo.VideoStreamId;
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
        if (isPreBuffered)
        {
            return true;
        }

        int dataInBuffer = (_writeIndex - _oldestDataIndex + _buffer.Length) % _buffer.Length;
        float percentBuffered = ((float)dataInBuffer / _buffer.Length) * 100;

        isPreBuffered = percentBuffered >= _preBuffPercent;
        return isPreBuffered;
    }

    public async Task<byte> Read(Guid clientId, CancellationToken cancellationToken)
    {
        while (!IsPreBuffered())
        {
            await Task.Delay(100, cancellationToken);  // Wait for 100 milliseconds before checking again
            cancellationToken.ThrowIfCancellationRequested();
        }

        int readIndex = _clientReadIndexes[clientId];
        byte data = _buffer.Span[readIndex];
        _clientReadIndexes[clientId] = (readIndex + 1) % _buffer.Length;

        if (_clientStatistics.TryGetValue(clientId, out StreamingStatistics? clientStats))
        {
            clientStats.IncrementBytesRead();
        }

        return data;
    }

    public async Task<int> ReadChunk(Guid clientId, byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
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

        return count;
    }

    public void RegisterClient(Guid clientId, string clientAgent)
    {
        if (!_clientReadIndexes.ContainsKey(clientId))
        {
            _ = _clientReadIndexes.TryAdd(clientId, _oldestDataIndex);
            _ = _clientSemaphores.TryAdd(clientId, new SemaphoreSlim(0, 1));
            _ = _clientStatistics.TryAdd(clientId, new StreamingStatistics(clientAgent));
        }
    }

    public void ReleaseSemaphore(Guid clientId)
    {
        SemaphoreSlim semaphore = _clientSemaphores[clientId];
        _ = semaphore.Release();
    }

    public void UnregisterClient(Guid clientId)
    {
        _ = _clientReadIndexes.TryRemove(clientId, out _);
        _ = _clientSemaphores.Remove(clientId);
        _ = _clientStatistics.Remove(clientId, out _);
    }

    public void UpdateReadIndex(Guid clientId, int newIndex)
    {
        _clientReadIndexes[clientId] = newIndex % _buffer.Length;
    }

    public async Task WaitSemaphoreAsync(Guid clientId, CancellationToken cancellationToken)
    {
        SemaphoreSlim semaphore = _clientSemaphores[clientId];
        await semaphore.WaitAsync(50, cancellationToken);
    }

    public void Write(byte data)
    {
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
                    _ = semaphore.Release();
                }
            }
        }
        catch (Exception ex)
        {
            // Consider logging the exception here
            //_logger.LogError(ex, "An error occurred while releasing semaphores.");
        }
    }

    public int WriteChunk(byte[] data, int count)
    {
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
            // Consider logging the exception here
            //_logger.LogError(ex, "An error occurred while releasing semaphores.");
        }

        return bytesWritten;
    }
}
