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
    private readonly IStatisticsManager _statisticsManager;
    private readonly IInputStatisticsManager _inputStatisticsManager;
    private readonly IInputStreamingStatistics _inputStreamStatistics;
    public readonly StreamInfo StreamInfo;
    private readonly Memory<byte> _buffer;
    private readonly int _bufferSize;
    private readonly ConcurrentDictionary<Guid, int> _clientReadIndexes = new();

    //private readonly ConcurrentDictionary<Guid, TaskCompletionSource<bool>> _readWaitHandles = new();

    private readonly ConcurrentDictionary<Guid, ManualResetEventSlim> _readWaitHandles = new();
    private readonly ConcurrentDictionary<Guid, DateTime> _lastNotificationTime = new();


    private readonly ILogger<ICircularRingBuffer> _logger;
    private int _oldestDataIndex;
    private readonly float _preBuffPercent;
    private int _writeIndex;
    private readonly ReaderWriterLockSlim _readWriteLock = new();
    private bool isBufferFull = false; // This should be maintained as part of your buffer state

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

        if (setting.RingBufferSizeMB < 1 || setting.RingBufferSizeMB > 10)
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

    //private void OnDataAvailable(Guid clientId)
    //{
    //    if (_readWaitHandles.TryGetValue(clientId, out TaskCompletionSource<bool>? tcs))
    //    {
    //        tcs.TrySetResult(true);
    //    }
    //}

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

    public async Task<int> ReadChunkMemory(Guid clientId, Memory<byte> target, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Starting ReadChunkMemory for clientId: {clientId}", clientId);

        while (!IsPreBuffered() && !cancellationToken.IsCancellationRequested)
        {
            await Task.Delay(50, cancellationToken);
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
    public async Task WaitForDataAvailability(Guid clientId, CancellationToken cancellationToken)
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
                    if (waitHandle.WaitHandle.WaitOne(TimeSpan.FromMilliseconds(100)))
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


    //public async Task WaitForDataAvailability(Guid clientId, CancellationToken cancellationToken)
    //{
    //    ManualResetEventSlim waitHandle = _readWaitHandles.GetOrAdd(clientId, _ => new ManualResetEventSlim(false));

    //    // Create a linked token source that will cancel either on the original token's cancellation or after the timeout
    //    using (CancellationTokenSource linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
    //    {
    //        linkedCts.CancelAfter(TimeSpan.FromSeconds(5));

    //        try
    //        {
    //            while (GetAvailableBytes(clientId) == 0 && !linkedCts.Token.IsCancellationRequested)
    //            {
    //                // Wait asynchronously on the ManualResetEventSlim with a delay
    //                Task waitTask = Task.Run(() => waitHandle.Wait(linkedCts.Token), cancellationToken);
    //                Task delayTask = Task.Delay(TimeSpan.FromMilliseconds(100), cancellationToken); // Introduce a 100ms delay

    //                // Wait for either the wait handle to be set or the delay to complete
    //                await Task.WhenAny(waitTask, delayTask);
    //            }
    //        }
    //        catch (OperationCanceledException)
    //        {
    //            _logger.LogError("WaitForDataAvailability timed out for client ID {clientId}", clientId);
    //            // Handle the cancellation (timeout or external cancellation)
    //            // Log or perform necessary actions on timeout or cancellation
    //            // This catch block will handle both the linked token (timeout) and the original cancellationToken
    //        }
    //    }

    //    // Reset the event for the next wait
    //    waitHandle.Reset();
    //}




    //public async Task WaitForDataAvailability(Guid clientId, CancellationToken cancellationToken)
    //{
    //    try
    //    {
    //        // Check if data is already available before waiting
    //        if (GetAvailableBytes(clientId) > 0)
    //        {
    //            return;
    //        }

    //        // Create or get an existing TaskCompletionSource for this client
    //        TaskCompletionSource<bool> tcs = _readWaitHandles.GetOrAdd(clientId, id => new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously));

    //        // Register cancellation token to set the TaskCompletionSource as canceled
    //        using (cancellationToken.Register(() => tcs.TrySetCanceled()))
    //        {
    //            // Wait for the task to complete, which indicates data is available
    //            await tcs.Task.ConfigureAwait(false);

    //            // Reset the TaskCompletionSource for the next wait
    //            _readWaitHandles[clientId] = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
    //        }
    //    }
    //    catch (TaskCanceledException)
    //    {

    //    }
    //}

    public void RegisterClient(IClientStreamerConfiguration streamerConfiguration)
    {
        if (!_clientReadIndexes.ContainsKey(streamerConfiguration.ClientId))
        {
            _ = _clientReadIndexes.TryAdd(streamerConfiguration.ClientId, _oldestDataIndex);
            _statisticsManager.RegisterClient(streamerConfiguration.ClientId, streamerConfiguration.ClientUserAgent, streamerConfiguration.ClientIPAddress);
        }
        _logger.LogInformation("RegisterClient for ClientId: {ClientId} {VideoStreamName} {_oldestDataIndex}", streamerConfiguration.ClientId, StreamInfo.VideoStreamName, _oldestDataIndex);
    }


    //private void NotifyClients()
    //{
    //    foreach (Guid clientId in _clientReadIndexes.Keys)
    //    {
    //        if (_readWaitHandles.TryGetValue(clientId, out ManualResetEventSlim? waitHandle))
    //        {
    //            waitHandle.Set();
    //        }
    //    }
    //}

    private void NotifyClients()
    {
        DateTime now = DateTime.UtcNow;

        foreach (Guid clientId in _readWaitHandles.Keys)
        {
            if (_readWaitHandles.TryGetValue(clientId, out ManualResetEventSlim? waitHandle))
            {
                // Get the last notification time and update it to now
                DateTime lastNotificationTime = _lastNotificationTime.GetOrAdd(clientId, now);
                _lastNotificationTime[clientId] = now;

                // Log the elapsed time if it's more than 500 milliseconds
                TimeSpan elapsed = now - lastNotificationTime;
                if (elapsed.TotalMilliseconds > 500)
                {
                    // Log the elapsed time here
                    _logger.LogInformation($"Client {clientId}: {elapsed.TotalMilliseconds}ms elapsed since last set.");
                }

                waitHandle.Set();
            }
        }
    }

    public void UnRegisterClient(Guid clientId)
    {
        _ = _clientReadIndexes.TryRemove(clientId, out _);
        //_ = _clientSemaphores.Remove(clientId);
        //_statisticsManager.UnRegisterClient(clientId);

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
            return effectiveWriteEnd < _writeIndex
                ? _oldestDataIndex <= effectiveWriteEnd || _oldestDataIndex > _writeIndex
                : effectiveWriteEnd > _oldestDataIndex;
        }
    }

}
