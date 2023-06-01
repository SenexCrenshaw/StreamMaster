using MediatR;

using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using StreamMasterApplication.Common.Interfaces;
using StreamMasterApplication.Common.Models;
using StreamMasterApplication.Hubs;
using StreamMasterApplication.VideoStreams.Queries;

using StreamMasterDomain.Common;

using System.Collections.Concurrent;

namespace StreamMasterInfrastructure.MiddleWare;

public class RingBufferManager : IDisposable, IRingBufferManager
{
    private readonly Timer _broadcastTimer;

    private readonly ConcurrentDictionary<Guid, CancellationTokenSource> _clientCancellationTokenSources;
    private readonly ConcurrentDictionary<string, CancellationTokenSource> _handlerTokens;
    private readonly IHubContext<StreamMasterHub, IStreamMasterHub> _hub;
    private readonly ILogger<RingBufferManager> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly ConcurrentDictionary<Guid, RingBufferReadStream> _streamReads;
    private readonly ConcurrentDictionary<string, StreamStreamInfo> _streamStreamInfos;
    private readonly Setting setting;
    public RingBufferManager(ILogger<RingBufferManager> logger, IServiceProvider serviceProvider, IHubContext<StreamMasterHub, IStreamMasterHub> hub, ISender sender)
    {
        _logger = logger;
        _hub = hub;
        _serviceProvider = serviceProvider;
        _handlerTokens = new ConcurrentDictionary<string, CancellationTokenSource>();
        _clientCancellationTokenSources = new ConcurrentDictionary<Guid, CancellationTokenSource>();
        _streamReads = new ConcurrentDictionary<Guid, RingBufferReadStream>();
        _streamStreamInfos = new();
         setting = FileUtil.GetSetting();
        _broadcastTimer = new Timer(BroadcastMessage, null, 3 * 1000, 3 * 1000);
    }

    public void Dispose()
    {
        _broadcastTimer?.Dispose();

        foreach (StreamStreamInfo _streamStreamInfo in _streamStreamInfos.Values)
        {
            _streamStreamInfo.Dispose();
        }
        _streamStreamInfos.Clear();

        foreach (CancellationTokenSource cancellationTokenSource in _clientCancellationTokenSources.Values)
        {
            cancellationTokenSource.Dispose();
        }
        _clientCancellationTokenSources.Clear();
    }

    /// <summary>
    /// Retrieves statistics for the specified stream URL managed by the RingBufferManager.
    /// </summary>
    /// <param name="streamUrl">
    /// The URL of the stream to get the statistics for.
    /// </param>
    /// <returns>
    /// A SingleStreamStatisticsResult object containing the statistics for the
    /// specified stream URL or null if the stream URL is not found.
    /// </returns>
    public SingleStreamStatisticsResult GetAllStatistics(string streamUrl)
    {
        _logger.LogInformation("Retrieving statistics for stream: {StreamUrl}", setting.CleanURLs ? "url removed": streamUrl);

        CircularRingBuffer? buffer = GetBuffer(streamUrl);
        if (buffer != null)
        {
            _logger.LogInformation("Retrieving statistics for stream: {StreamUrl}", setting.CleanURLs ? "url removed" : streamUrl);
            return new SingleStreamStatisticsResult
            {
                StreamUrl = streamUrl,
                ClientStatistics = buffer.GetAllStatistics()
            };
        }
        _logger.LogWarning("Stream not found when retrieving statistics: {StreamUrl}", setting.CleanURLs ? "url removed" : streamUrl);
        return new SingleStreamStatisticsResult();
    }

    /// <summary>
    /// Retrieves statistics for all stream URLs managed by the RingBufferManager.
    /// </summary>
    /// <returns>
    /// A list of StreamStatisticsResult objects containing the statistics for
    /// each stream URL.
    /// </returns>
    public List<StreamStatisticsResult> GetAllStatisticsForAllUrls()
    {
        // _logger.LogInformation("Retrieving statistics for all stream URLs");
        List<StreamStatisticsResult> allStatistics = new();

        foreach (string streamUrl in _streamStreamInfos.Keys)
        {
            CircularRingBuffer? bufferEntry = _streamStreamInfos[streamUrl].RingBuffer;

            if (bufferEntry is null)
            {
                continue;
            }

            StreamingStatistics input = bufferEntry.GetInputStreamStatistics();
            foreach (ClientStreamingStatistics stat in bufferEntry.GetAllStatistics())
            {
                allStatistics.Add(new StreamStatisticsResult
                {
                    M3UStreamId = bufferEntry.StreamInfo.M3UStreamId,
                    M3UStreamName = bufferEntry.StreamInfo.M3UStreamName,
                    M3UStreamProxyType = bufferEntry.StreamInfo.StreamProxyType,
                    Logo = bufferEntry.StreamInfo.Logo,

                    InputBytesRead = input.BytesRead,
                    InputBytesWritten = input.BytesWritten,
                    InputBitsPerSecond = input.BitsPerSecond,
                    InputStartTime = input.StartTime,

                    //InputStreamStatistics = bufferEntry.Value.GetInputStreamStatistics(),
                    StreamUrl = streamUrl,

                    ClientBitsPerSecond = stat.BitsPerSecond,
                    ClientBytesRead = stat.BytesRead,
                    ClientBytesWritten = stat.BytesWritten,
                    ClientId = stat.ClientId,
                    ClientStartTime = stat.StartTime,

                    // ClientStatistics = bufferEntry.Value.GetAllStatistics()
                });
            }
        }
        // _logger.LogInformation("Retrieved statistics for all stream URLs");

        return allStatistics;
    }

    /// Gets the stream associated with the specified stream URL or creates a
    /// new one if it doesn't exist. </summary> <param name="config">The
    /// streamer configuration containing the client and stream details.</param>
    /// <returns>A tuple containing the stream and the client ID.</returns>
    public (Stream? stream, Guid clientId, ProxyStreamError? error) GetStream(StreamerConfiguration config)
    {
        string? streamUrl = ChannelUtils.ChannelManager(config);
        if (streamUrl == null)
        {
            ProxyStreamError error = new() { ErrorCode = ProxyStreamErrorCode.ChannelManagerFinished, Message = "Channel Manager Called Exit" };

            return (null, config.ClientId, error);
        }

        StreamStreamInfo? streamStreamInfo = GetOrCreateBuffer(streamUrl, config);

        if (streamStreamInfo is null || streamStreamInfo.RingBuffer is null)
        {
            _logger.LogCritical("Could not create buffer for client {ClientId} registered for stream: {StreamUrl}", config.ClientId, setting.CleanURLs ? "url removed" : streamUrl);
            //throw new ApplicationException($"Could not create buffer for client {config.ClientId} registered for stream: {streamUrl}");
            return (null, config.ClientId, null);
        }

        RingBufferReadStream streamRead = new(() => streamStreamInfo.RingBuffer, config.ClientId, config.CancellationToken);
        _ = _streamReads.TryAdd(config.ClientId, streamRead);

        streamStreamInfo.RingBuffer.RegisterClient(config.ClientId);
        IncrementClientCounter(streamUrl);

        _logger.LogInformation("Client {ClientId} registered for stream: {StreamUrl}", config.ClientId, setting.CleanURLs ? "url removed" : streamUrl);
        return (streamRead, config.ClientId, null);
    }

    /// <summary>
    /// Removes a client from the buffer associated with the specified stream URL.
    /// </summary>
    /// <param name="config">
    /// The streamer configuration containing the client and stream details.
    /// </param>
    public void RemoveClient(StreamerConfiguration config)
    {
        CircularRingBuffer? buffer = GetBuffer(config.CurentVideoStream.User_Url);

        if (buffer != null)
        {
            buffer.UnregisterClient(config.ClientId);
            _logger.LogInformation("Client {ClientId} unregistered for stream: {StreamUrl}", config.ClientId,  setting.CleanURLs ? "url removed" : config.CurentVideoStream.User_Url);

            _ = _clientCancellationTokenSources.TryRemove(config.ClientId, out _);
            if (_clientCancellationTokenSources.TryGetValue(config.ClientId, out CancellationTokenSource? token))
            {
                token.Cancel();
            }
            DecrementClientCounter(config.CurentVideoStream.User_Url);
            _logger.LogInformation("Client removed: {ClientId}", config.ClientId);
        }
        else
        {
            _logger.LogWarning("Unable to remove client: {ClientId}. Stream not found: {StreamUrl}", config.ClientId, setting.CleanURLs ? "url removed" : config.CurentVideoStream.User_Url);
        }
    }

    /// <summary>
    /// Simulates a stream failure for the specified stream URL by canceling the
    /// associated streaming task.
    /// </summary>
    /// <param name="streamUrl">
    /// The URL of the stream to simulate the failure for.
    /// </param>
    public void SimulateStreamFailure(string streamUrl)
    {
        if (string.IsNullOrEmpty(streamUrl))
        {
            _logger.LogWarning("Stream URL is empty or null, cannot simulate stream failure");
            return;
        }

        if (_streamStreamInfos.TryGetValue(streamUrl, out StreamStreamInfo? _streamStreamInfo))
        {
            if (_streamStreamInfo.StreamerCancellationToken is not null)
            {
                _streamStreamInfo.StreamerCancellationToken.Cancel();
            }
            _logger.LogInformation("Simulating stream failure for: {StreamUrl}", setting.CleanURLs ? "url removed" : streamUrl);
        }
        else
        {
            _logger.LogWarning("Stream not found, cannot simulate stream failure: {StreamUrl}", setting.CleanURLs ? "url removed" : streamUrl);
        }
    }

    private void BroadcastMessage(object? state)
    {
        _ = _hub.Clients.All.StreamStatisticsResultsUpdate(GetAllStatisticsForAllUrls());
        // _logger.LogInformation("Broadcast message sent for all streams");
    }

    //private bool CheckMaxStreamsForUrl(string streamUrl, StreamerConfiguration clientInfo)
    //{
    //    using IServiceScope scope = _serviceProvider.CreateScope();
    //    ISender _sender = scope.ServiceProvider.GetRequiredService<ISender>();

    // var m3uFileTask = _sender.Send(new
    // GetM3UFileIdMaxStreamFromUrl(streamUrl)); m3uFileTask.Wait();

    // var m3uFileIdMaxStream = m3uFileTask.Result; if (m3uFileIdMaxStream ==
    // null) { _logger.LogError("M3UFile not found for stream: {StreamUrl}",
    // streamUrl); return false; }

    // clientInfo.CurrentM3UFileId= m3uFileIdMaxStream.M3UFileId;

    // var currentStreamCount =
    // _m3uFileIdMaxStreams.GetOrAdd(m3uFileIdMaxStream.M3UFileId, _ => { return
    // 0; });

    // if (currentStreamCount >= m3uFileIdMaxStream.MaxStreams) { return false; }

    //    return true;
    //}

    /// <summary>
    /// Creates a new buffer and starts the video streaming for the specified
    /// stream URL and client configuration.
    /// </summary>
    /// <param name="streamUrl">
    /// The URL of the stream to create and start the buffer for.
    /// </param>
    /// <param name="clientInfo">The client configuration for the stream.</param>
    /// <returns>A new instance of the CircularRingBuffer.</returns>
    private StreamStreamInfo CreateAndStartBuffer(string streamUrl, StreamerConfiguration clientInfo)
    {
        _logger.LogInformation("Creating and starting buffer for stream: {StreamUrl}", setting.CleanURLs ? "url removed" : streamUrl);

        CircularRingBuffer buffer = new(clientInfo);

        CancellationTokenSource cancellationTokenSource = new();
        Task streamingTask = StartVideoStreaming(streamUrl, clientInfo, buffer, cancellationTokenSource);

        using IServiceScope scope = _serviceProvider.CreateScope();
        ISender _sender = scope.ServiceProvider.GetRequiredService<ISender>();

        var m3uFileTask = _sender.Send(new GetM3UFileIdMaxStreamFromUrl(streamUrl));
        m3uFileTask.Wait();

        var m3uFileIdMaxStream = m3uFileTask.Result;
        if (m3uFileIdMaxStream == null)
        {
            _logger.LogError("M3UFile not found for stream: {StreamUrl}", setting.CleanURLs ? "url removed" : streamUrl);
            m3uFileIdMaxStream = new M3UFileIdMaxStream { M3UFileId = 0, MaxStreams = 999 };
        }

        StreamStreamInfo streamStreamInfo = new(streamUrl, buffer, streamingTask, m3uFileIdMaxStream.M3UFileId, m3uFileIdMaxStream.MaxStreams, cancellationTokenSource);

        _ = _streamStreamInfos.TryAdd(streamUrl, streamStreamInfo);

        // Add a handler to handle for the stream if one doesnt exist
        if (!_handlerTokens.TryGetValue(streamUrl, out _))
        {
            CancellationTokenSource handlerCancellationTokenSource = new();
            _ = HandleFailover(clientInfo, handlerCancellationTokenSource.Token);
            _ = _handlerTokens.TryAdd(streamUrl, handlerCancellationTokenSource);
        }

        _logger.LogInformation("Buffer created and streaming started for: {StreamUrl}", setting.CleanURLs ? "url removed" : streamUrl);

        return streamStreamInfo;
    }

    /// <summary>
    /// Decrements the client counter for the specified stream URL. If the
    /// counter reaches zero, it removes the buffer and streaming task
    /// associated with the stream URL.
    /// </summary>
    /// <param name="streamUrl">
    /// The URL of the stream to decrement the client counter for.
    /// </param>
    private void DecrementClientCounter(string streamUrl)
    {
        if (_streamStreamInfos.TryGetValue(streamUrl, out StreamStreamInfo? _streamStreamInfo))
        {
            if (_streamStreamInfo.ClientCounter > 0)
            {
                --_streamStreamInfo.ClientCounter;
            }

            if (_streamStreamInfo.ClientCounter == 0)
            {                                
                if (_streamStreamInfos.TryRemove(streamUrl, out _))
                {
                    _logger.LogInformation("Buffer removed for stream: {StreamUrl}", setting.CleanURLs ? "url removed" : streamUrl);
                    _streamStreamInfo.Stop();
                }                
            }
            else
            {
                _logger.LogInformation("Client counter decremented for stream: {StreamUrl}. New count: {ClientCount}", setting.CleanURLs ? "url removed" : streamUrl, _streamStreamInfo.ClientCounter);
            }
        }
    }

    private CircularRingBuffer? GetBuffer(string streamUrl)
    {
        return _streamStreamInfos.TryGetValue(streamUrl, out StreamStreamInfo? _streamStreamInfo) ? _streamStreamInfo.RingBuffer : null;
    }

    /// <summary>
    /// Gets the existing buffer associated with the specified stream URL or
    /// creates a new one if it doesn't exist.
    /// </summary>
    /// <param name="streamUrl">
    /// The URL of the stream to get or create the buffer for.
    /// </param>
    /// <param name="config">The client configuration for the stream.</param>
    /// <returns>The existing or newly created buffer.</returns>
    private StreamStreamInfo? GetOrCreateBuffer(string streamUrl, StreamerConfiguration config)
    {
        _logger.LogInformation("Getting or creating buffer for stream: {StreamUrl}", setting.CleanURLs ? "url removed" : streamUrl);
        var si = _streamStreamInfos.GetOrAdd(streamUrl, _ =>
        {
            _logger.LogInformation("Creating and starting a new buffer for stream: {StreamUrl}", setting.CleanURLs ? "url removed" : streamUrl);

            return CreateAndStartBuffer(streamUrl, config);
        });

        var allStreamsCount = _streamStreamInfos.Where(x => x.Value.M3UFileId == si.M3UFileId).Sum(a => a.Value.ClientCounter);

        if (allStreamsCount >= si.MaxStreams)
        {
            _logger.LogInformation("Max stream count reached for stream: {StreamUrl}", setting.CleanURLs ? "url removed" : streamUrl);
            return null;
        }

        return si;
    }

    /// <summary>
    /// Handles failover for the specified stream URL by monitoring the
    /// streaming task and switching to a new stream URL in case of failure.
    /// </summary>
    /// <param name="clientInfo">The client configuration for the stream.</param>
    /// <param name="cancellationToken">
    /// The cancellation token to cancel the operation.
    /// </param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    private async Task HandleFailover(StreamerConfiguration clientInfo, CancellationToken cancellationToken)
    {
        string streamUrl = clientInfo.CurentVideoStream.User_Url;
        _logger.LogInformation("Failover handling watching stream: {StreamUrl}", setting.CleanURLs ? "url removed" : streamUrl);

        await Task.Delay(clientInfo.FailoverCheckInterval, cancellationToken);

        if (!_streamStreamInfos.TryGetValue(streamUrl, out StreamStreamInfo? _streamStreamInfo))
        {
            _logger.LogWarning("HandleFailover streamer info not found for: {StreamUrl}", setting.CleanURLs ? "url removed" : streamUrl);
            return;
        }

        bool quit = false;
        while (!cancellationToken.IsCancellationRequested && !quit)
        {
            if (!_streamStreamInfos.TryGetValue(streamUrl, out _streamStreamInfo))
            {
                _logger.LogWarning("HandleFailover Stream stopped: {StreamUrl}", setting.CleanURLs ? "url removed" : streamUrl);

                return;
            }

            if (_streamStreamInfo.ClientCounter == 0 || _streamStreamInfo.StreamerCancellationToken.IsCancellationRequested || _streamStreamInfo.StreamingTask.IsFaulted || _streamStreamInfo.StreamingTask.IsCanceled)
            {
                if (_streamStreamInfo.FailoverInProgress)
                {
                    continue;
                }

                if (_streamStreamInfo.ClientCounter == 0)
                {
                    if (!_streamStreamInfo.StreamerCancellationToken.IsCancellationRequested)
                    {
                        _streamStreamInfo.StreamerCancellationToken.Cancel();
                    }
                    _logger.LogWarning("No more clients for : {StreamUrl}", setting.CleanURLs ? "url removed" : streamUrl);
                    quit = true;
                    continue;
                }

                _streamStreamInfo.FailoverInProgress = true;

                await Task.Delay(200, cancellationToken);

                _logger.LogWarning("Stream failure detected for stream: {StreamUrl}", setting.CleanURLs ? "url removed" : streamUrl);

                _streamStreamInfo.Stop();

                IReadOnlyList<Guid> clientIds = _streamStreamInfo.RingBuffer.GetClientIds();

                _ = _streamStreamInfos.TryRemove(streamUrl, out _);

                string? newStreamUrl = ChannelUtils.ChannelManager(clientInfo);
                if (newStreamUrl == null)
                {
                    break;
                }

                StreamStreamInfo? streamStreamInfo = GetOrCreateBuffer(newStreamUrl, clientInfo);
                if (streamStreamInfo == null)
                {
                    break;
                }
                foreach (Guid clientdId in clientIds)
                {
                    streamStreamInfo.RingBuffer.RegisterClient(clientdId);
                    if (_streamReads.TryGetValue(clientdId, out RingBufferReadStream? streamRead))
                    {
                        streamRead.SetBufferDelegate(() => streamStreamInfo.RingBuffer);
                    }
                }

                _logger.LogInformation("Failover handled, switched to new stream URL: {NewStreamUrl}", setting.CleanURLs ? "url removed" :  newStreamUrl);

                // Update the stream URL
                streamUrl = newStreamUrl;

                _streamStreamInfo.FailoverInProgress = false;
            }

            await Task.Delay(clientInfo.FailoverCheckInterval, cancellationToken);
        }

        //_streamingTasks.TryRemove(streamUrl, out _);
        _logger.LogInformation("Failover handling stopped for stream: {StreamUrl}", setting.CleanURLs ? "url removed" : streamUrl);
    }

    private void IncrementClientCounter(string streamUrl)
    {
        if (_streamStreamInfos.TryGetValue(streamUrl, out StreamStreamInfo? _streamStreamInfo))
        {
            _streamStreamInfo.ClientCounter++;
        }
    }

    /// <summary>
    /// Starts video streaming for the specified stream URL and writes the data
    /// into the provided buffer.
    /// </summary>
    /// <param name="streamUrl">
    /// The URL of the stream to start video streaming for.
    /// </param>
    /// <param name="clientInfo">The client configuration for the stream.</param>
    /// <param name="buffer">The buffer to write the streamed data into.</param>
    /// <param name="cancellationToken">
    /// The cancellation token to cancel the operation.
    /// </param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    private async Task StartVideoStreaming(string streamUrl, StreamerConfiguration clientInfo, CircularRingBuffer buffer, CancellationTokenSource cancellationToken)
    {
        _logger.LogInformation("Starting video read streaming for stream: {StreamUrl}", setting.CleanURLs ? "url removed" : streamUrl);

        (Stream? stream, ProxyStreamError? error) = await StreamingProxies.GetProxyStream(streamUrl);
        if (stream == null || error != null)
        {
            _logger.LogError("Error getting proxy stream for {streamUrl}: {error?.Message}", setting.CleanURLs ? "url removed" : streamUrl, error?.Message);

            return;
        }

        byte[] bufferChunk = new byte[clientInfo.BufferChunkSize];
        int bytesRead;

        int retryCount = 0;
        int maxRetries = clientInfo.MaxConnectRetries > 0 ? clientInfo.MaxConnectRetries : 3;
        int waitTime = clientInfo.MaxConnectRetryTime > 0 ? clientInfo.MaxConnectRetryTime : 50;

        while (!cancellationToken.IsCancellationRequested)
        {
            bytesRead = await stream.ReadAsync(bufferChunk, cancellationToken.Token);
            if (bytesRead == 0)
            {
                retryCount++;
                if (retryCount >= maxRetries)
                {
                    break;
                }
                _logger.LogInformation("Input read stream received 0 bytes for stream: {StreamUrl} Retry {retryCount}/{maxRetries}", setting.CleanURLs ? "url removed" : streamUrl, retryCount, maxRetries);
                await Task.Delay(waitTime); // wait before retrying
            }
            else
            {
                _ = buffer.WriteChunk(bufferChunk, bytesRead);

                retryCount = 0; // reset retry count on successful read
            }
        }

        _logger.LogInformation("Input read stream stopped for stream: {StreamUrl}", setting.CleanURLs ? "url removed" : streamUrl);
        cancellationToken.Cancel();
    }
}
