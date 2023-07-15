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

    public CircularRingBuffer(ChildVideoStreamDto childVideoStreamDto, int tempbuffersize = 0)
    {
        if (setting.PreloadPercentage < 0 || setting.PreloadPercentage > 100)
            setting.PreloadPercentage = 0;

        _bufferSize = tempbuffersize > 0 ? tempbuffersize : setting.RingBufferSizeMB * 1024 * 1000;
        _preBuffPercent = setting.PreloadPercentage;

        StreamInfo = new StreamInfo
        {
            M3UStreamId = childVideoStreamDto.Id,
            M3UStreamName = childVideoStreamDto.User_Tvg_name,
            Logo = childVideoStreamDto.User_Tvg_logo,
            StreamProxyType = childVideoStreamDto.StreamProxyType,
            StreamUrl = childVideoStreamDto.User_Url,
        };

        _buffer = new byte[_bufferSize];
        _writeIndex = 0;
        _oldestDataIndex = 0;
    }

    public int BufferSize => _buffer.Length;
    private bool isPreBuffered { get; set; } = false;
    private Setting setting => FileUtil.GetSetting();

    /// <summary>
    /// Returns a List with all the clients' streaming statistics.
    /// </summary>
    /// <returns>A List with client statistics.</returns>
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
                M3UStreamId = StreamInfo.M3UStreamId,
                M3UStreamName = StreamInfo.M3UStreamName,
                M3UStreamProxyType = StreamInfo.StreamProxyType,
                Logo = StreamInfo.Logo,

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

    /// <summary>
    /// Returns the number of available bytes in the buffer for a given client.
    /// </summary>
    /// <param name="clientId">The ID of the client.</param>
    /// <returns>The number of available bytes.</returns>
    public int GetAvailableBytes(Guid clientId)
    {
        int readIndex = _clientReadIndexes[clientId];
        return (_writeIndex - readIndex + _buffer.Length) % _buffer.Length;
    }

    /// <summary>
    /// Returns a list of all client IDs registered with the buffer.
    /// </summary>
    /// <returns>A list of client IDs.</returns>
    public ICollection<Guid> GetClientIds()
    {
        return _clientReadIndexes.Keys;
    }

    /// <summary>
    /// Returns the streaming statistics for a given client.
    /// </summary>
    /// <param name="clientId">The ID of the client.</param>
    /// <returns>The client's streaming statistics.</returns>
    public StreamingStatistics? GetClientStatistics(Guid clientId)
    {
        return _clientStatistics.TryGetValue(clientId, out StreamingStatistics? clientStats) ? clientStats : null;
    }

    /// <summary>
    /// Returns the input stream statistics for the buffer.
    /// </summary>
    /// <returns>The input stream statistics.</returns>
    public StreamingStatistics GetInputStreamStatistics()
    {
        return _inputStreamStatistics;
    }

    /// <summary>
    /// Returns the current read index for a given client.
    /// </summary>
    /// <param name="clientId">The ID of the client.</param>
    /// <returns>The client's read index.</returns>
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

    /// <summary>
    /// Reads a single byte of data from the buffer for the specified client.
    /// </summary>
    /// <param name="clientId">The ID of the client.</param>
    /// <param name="cancellationToken">
    /// Cancellation token for cancelling the operation.
    /// </param>
    /// <returns>The read byte, or -1 if the read fails.</returns>
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

        // Update client statistics
        if (_clientStatistics.TryGetValue(clientId, out StreamingStatistics? clientStats))
        {
            clientStats.IncrementBytesRead();
        }

        return data;
    }

    /// <summary>
    /// Reads a chunk of data from the buffer for the specified client.
    /// </summary>
    /// <param name="clientId">The ID of the client.</param>
    /// <param name="buffer">The buffer to read data into.</param>
    /// <param name="offset">
    /// The offset within the buffer to start reading data.
    /// </param>
    /// <param name="count">The number of bytes to read.</param>
    /// <param name="cancellationToken">
    /// Cancellation token for cancelling the operation.
    /// </param>
    /// <returns>The number of bytes read.</returns>
    public async Task<int> ReadChunk(Guid clientId, byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        while (!IsPreBuffered())
        {
            await Task.Delay(100, cancellationToken);  // Wait for 100 milliseconds before checking again
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

    /// <summary>
    /// Registers a new client with the buffer.
    /// </summary>
    /// <param name="clientId">The ID of the client.</param>
    public void RegisterClient(Guid clientId, string clientAgent)
    {
        if (!_clientReadIndexes.ContainsKey(clientId))
        {
            _ = _clientReadIndexes.TryAdd(clientId, _oldestDataIndex);
            _ = _clientSemaphores.TryAdd(clientId, new SemaphoreSlim(0, 1));
            _ = _clientStatistics.TryAdd(clientId, new StreamingStatistics(clientAgent));
        }
    }

    /// <summary>
    /// Releases the semaphore for the specified client.
    /// </summary>
    /// <param name="clientId">The ID of the client.</param>
    public void ReleaseSemaphore(Guid clientId)
    {
        SemaphoreSlim semaphore = _clientSemaphores[clientId];
        _ = semaphore.Release();
    }

    /// <summary>
    /// Unregisters a client from the buffer.
    /// </summary>
    /// <param name="clientId">The ID of the client to unregister.</param>
    public void UnregisterClient(Guid clientId)
    {
        _ = _clientReadIndexes.TryRemove(clientId, out _);
        _ = _clientSemaphores.Remove(clientId);
        _ = _clientStatistics.Remove(clientId, out _);
    }

    /// <summary>
    /// Updates the read index for the specified client.
    /// </summary>
    /// <param name="clientId">The ID of the client.</param>
    /// <param name="newIndex">The new read index.</param>
    public void UpdateReadIndex(Guid clientId, int newIndex)
    {
        _clientReadIndexes[clientId] = newIndex % _buffer.Length;
    }

    /// <summary>
    /// Asynchronously waits for the semaphore for the specified client.
    /// </summary>
    /// <param name="clientId">The ID of the client.</param>
    /// <param name="cancellationToken">
    /// Cancellation token for cancelling the operation.
    /// </param>
    /// <returns>The task representing the asynchronous operation.</returns>
    public async Task WaitSemaphoreAsync(Guid clientId, CancellationToken cancellationToken)
    {
        SemaphoreSlim semaphore = _clientSemaphores[clientId];
        await semaphore.WaitAsync(50, cancellationToken);
    }

    /// <summary>
    /// Writes a single byte of data to the buffer.
    /// </summary>
    /// <param name="data">The byte of data to write.</param>
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

    /// <summary>
    /// Writes a chunk of data to the buffer.
    /// </summary>
    /// <param name="data">The data to write.</param>
    /// <param name="count">The number of bytes to write.</param>
    /// <returns>The number of bytes written.</returns>
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
