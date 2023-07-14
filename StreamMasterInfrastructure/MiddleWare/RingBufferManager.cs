using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using StreamMasterApplication.Common.Interfaces;
using StreamMasterApplication.Common.Models;
using StreamMasterApplication.Hubs;

using StreamMasterDomain.Common;
using StreamMasterDomain.Enums;

using System.Collections.Concurrent;

namespace StreamMasterInfrastructure.MiddleWare;

public class RingBufferManager : IDisposable, IRingBufferManager
{
    private readonly Timer _broadcastTimer;

    private readonly ConcurrentDictionary<string, CancellationTokenSource> _handlerTokens;
    private readonly IHubContext<StreamMasterHub, IStreamMasterHub> _hub;
    private readonly ILogger<RingBufferManager> _logger;
    private readonly IServiceProvider _serviceProvider;

    private StreamManager _streamManager;

    public RingBufferManager(
        ILogger<RingBufferManager> logger,

        IServiceProvider serviceProvider,
        IHubContext<StreamMasterHub, IStreamMasterHub> hub)
    {
        _logger = logger;
        _hub = hub;
        _serviceProvider = serviceProvider;
        _handlerTokens = new ConcurrentDictionary<string, CancellationTokenSource>();
        _streamManager = new StreamManager(_logger);
        _broadcastTimer = new Timer(BroadcastMessage, null, 1000, 1000);
    }

    public Setting setting
    {
        get
        {
            return FileUtil.GetSetting();
        }
    }

    public void Dispose()
    {
        _broadcastTimer?.Dispose();
        _streamManager.Dispose();
    }

    public SingleStreamStatisticsResult GetAllStatistics(string streamUrl)
    {
        _logger.LogInformation("Retrieving statistics for stream: {StreamUrl}", setting.CleanURLs ? "url removed" : streamUrl);

        var buffer = _streamManager.GetBufferFromStreamUrl(streamUrl);
        if (buffer != null)
        {
            _logger.LogInformation("Retrieving statistics for stream: {StreamUrl}", setting.CleanURLs ? "url removed" : streamUrl);
            return buffer.GetSingleStreamStatisticsResult();
        }
        _logger.LogWarning("Stream not found when retrieving statistics: {StreamUrl}", setting.CleanURLs ? "url removed" : streamUrl);
        return new SingleStreamStatisticsResult();
    }

    public List<StreamStatisticsResult> GetAllStatisticsForAllUrls()
    {
        List<StreamStatisticsResult> allStatistics = new();

        var infos = _streamManager.GetStreamInformations();
        foreach (var info in infos.Where(a => a.RingBuffer != null))
        {
            allStatistics.AddRange(info.RingBuffer.GetAllStatisticsForAllUrls());
        }

        return allStatistics;
    }

    public (Stream? stream, Guid clientId, ProxyStreamError? error) GetStream(StreamerConfiguration config)
    {
        string? streamUrl = ChannelUtils.ChannelManager(config);
        if (streamUrl == null)
        {
            ProxyStreamError error = new() { ErrorCode = ProxyStreamErrorCode.ChannelManagerFinished, Message = "Channel Manager Called Exit" };

            return (null, config.ClientId, error);
        }

        StreamInformation? streamStreamInfo = GetOrCreateBuffer(streamUrl, config);

        if (streamStreamInfo is null || streamStreamInfo.RingBuffer is null)
        {
            _logger.LogCritical("Could not create buffer for client {ClientId} registered for stream: {StreamUrl}", config.ClientId, setting.CleanURLs ? "url removed" : streamUrl);

            return (null, config.ClientId, null);
        }

        RingBufferReadStream streamRead = new RingBufferReadStream(() => streamStreamInfo.RingBuffer, config);
        config.ReadBuffer = streamRead;

        RegisterClientToNewStream(config.ClientId, streamStreamInfo);

        _logger.LogInformation("Client {ClientId} registered for stream: {StreamUrl}", config.ClientId, setting.CleanURLs ? "url removed" : streamUrl);
        return (streamRead, config.ClientId, null);
    }

    public void RemoveClient(StreamerConfiguration config)
    {
        var buffer = _streamManager.GetBufferFromStreamUrl(config.CurentVideoStream.User_Url);

        if (buffer != null)
        {
            buffer.UnregisterClient(config.ClientId);
            _logger.LogInformation("Client {ClientId} unregistered for stream: {StreamUrl}", config.ClientId, setting.CleanURLs ? "url removed" : config.CurentVideoStream.User_Url);

            DecrementClientCounter(config);
        }
        else
        {
            _logger.LogWarning("Unable to remove client: {ClientId}. Stream not found: {StreamUrl}", config.ClientId, setting.CleanURLs ? "url removed" : config.CurentVideoStream.User_Url);
        }
    }

    public void SimulateStreamFailure(string streamUrl)
    {
        if (string.IsNullOrEmpty(streamUrl))
        {
            _logger.LogWarning("Stream URL is empty or null, cannot simulate stream failure");
            return;
        }

        StreamInformation? _streamInformation = _streamManager.GetStreamInformationFromStreamUrl(streamUrl);

        if (_streamInformation is not null)
        {
            if (_streamInformation.StreamerCancellationToken is not null)
            {
                _streamInformation.StreamerCancellationToken.Cancel();
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
    }

    private StreamInformation? CreateAndStartBuffer(string streamUrl, StreamerConfiguration clientInfo)
    {
        return CreateAndStartBufferAsyncInternal(streamUrl, clientInfo).GetAwaiter().GetResult();
    }

    private async Task<StreamInformation?> CreateAndStartBufferAsyncInternal(string streamUrl, StreamerConfiguration clientInfo)
    {
        _logger.LogInformation("Creating and starting buffer for stream: {StreamUrl}", setting.CleanURLs ? "url removed" : streamUrl);

        CircularRingBuffer buffer = new(clientInfo);
        CancellationTokenSource cancellationTokenSource = new();

        Stream? stream;
        ProxyStreamError? error;
        int processId;

        if (setting.StreamingProxyType == StreamingProxyTypes.FFMpeg)
        {
            (stream, processId, error) = await StreamingProxies.GetFFMpegStream(streamUrl);
            if (processId == -1)
            {
                _logger.LogError("Error getting proxy stream for {StreamUrl}: {Error?.Message}", setting.CleanURLs ? "url removed" : streamUrl, error?.Message);
            }
        }
        else
        {
            (stream, processId, error) = await StreamingProxies.GetProxyStream(streamUrl, cancellationTokenSource.Token);
        }

        if (stream == null || error != null)
        {
            _logger.LogError("Error getting proxy stream for {StreamUrl}: {Error?.Message}", setting.CleanURLs ? "url removed" : streamUrl, error?.Message);
            return null;
        }

        Task streamingTask = StartVideoStreaming(stream, streamUrl, clientInfo, buffer, cancellationTokenSource);

        using IServiceScope scope = _serviceProvider.CreateScope();
        var _context = scope.ServiceProvider.GetRequiredService<IAppDbContext>();

        var m3uFileIdMaxStream = _context.GetM3UFileIdMaxStreamFromUrl(streamUrl);
        if (m3uFileIdMaxStream == null)
        {
            _logger.LogError("M3UFile not found for stream: {StreamUrl}", setting.CleanURLs ? "url removed" : streamUrl);
            return null;
        }

        StreamInformation streamStreamInfo = new(streamUrl, buffer, streamingTask, m3uFileIdMaxStream.M3UFileId, m3uFileIdMaxStream.MaxStreams, processId, cancellationTokenSource)
        {
            M3UStream = clientInfo.M3UStream
        };

        _streamManager.AddStreamInfo(streamStreamInfo);

        // Add a handler to handle the stream if one doesn't exist
        if (!_handlerTokens.ContainsKey(streamUrl))
        {
            CancellationTokenSource handlerCancellationTokenSource = new();
            _ = HandleFailover(clientInfo, handlerCancellationTokenSource.Token);
            _handlerTokens.TryAdd(streamUrl, handlerCancellationTokenSource);
        }

        _logger.LogInformation("Buffer created and streaming started for: {StreamUrl}", setting.CleanURLs ? "url removed" : streamUrl);

        return streamStreamInfo;
    }

    private void DecrementClientCounter(StreamerConfiguration config)
    {
        _streamManager.DecrementClientCounter(config);
    }

    private async Task DelayWithCancellation(int milliseconds, CancellationToken cancellationToken)
    {
        try
        {
            await Task.Delay(milliseconds, cancellationToken);
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogInformation("Task was cancelled");
            throw;
        }
    }

    private StreamInformation? GetOrCreateBuffer(string streamUrl, StreamerConfiguration config)
    {
        _logger.LogInformation("Getting or creating buffer for stream: {StreamUrl}", setting.CleanURLs ? "url removed" : streamUrl);
        StreamInformation? si = _streamManager.GetOrAdd(streamUrl, _ =>
        {
            _logger.LogInformation("Creating and starting a new buffer for stream: {StreamUrl}", setting.CleanURLs ? "url removed" : streamUrl);

            return CreateAndStartBuffer(streamUrl, config);
        });

        if (si == null)
        {
            return null;
        }

        var allStreamsCount = _streamManager.GetStreamsCountForM3UFile(si.M3UFileId);

        if (allStreamsCount > si.MaxStreams)
        {
            _logger.LogInformation("Max stream count reached for stream: {StreamUrl}", setting.CleanURLs ? "url removed" : streamUrl);
            return null;
        }

        _streamManager.IncrementClientCounter(config);

        return si;
    }

    private async Task HandleFailover(StreamerConfiguration clientInfo, CancellationToken cancellationToken)
    {
        string streamUrl = clientInfo.CurentVideoStream.User_Url;
        try
        {
            _logger.LogInformation("Failover watcher starting for stream: {StreamUrl}", setting.CleanURLs ? "url removed" : streamUrl);

            await DelayWithCancellation(clientInfo.FailoverCheckInterval, cancellationToken);
            if (!TryGetStreamInfo(streamUrl, out StreamInformation? _streamInformation))
            {
                _logger.LogWarning("HandleFailover streamer info not found for: {StreamUrl}", setting.CleanURLs ? "url removed" : streamUrl);

                return;
            }
            bool quit = false;
            while (!cancellationToken.IsCancellationRequested && !quit)
            {
                quit = ProcessStreamFailure(streamUrl, clientInfo, cancellationToken, _streamInformation);
                await DelayWithCancellation(clientInfo.FailoverCheckInterval, cancellationToken);
            }

            _logger.LogInformation("Failover watcher stopped for stream: {StreamUrl}", setting.CleanURLs ? "url removed" : streamUrl);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while handling failover for stream {StreamUrl}", setting.CleanURLs ? "url removed" : streamUrl);
        }
    }

    private void HandleStreamFailover(StreamerConfiguration clientInfo, CancellationToken cancellationToken, StreamInformation _streamInformation)
    {
        _streamInformation.FailoverInProgress = true;

        DelayWithCancellation(200, cancellationToken).Wait();

        _logger.LogWarning("Stream failure detected for stream: {StreamUrl}", setting.CleanURLs ? "url removed" : clientInfo.CurentVideoStream.User_Url);

        _streamInformation.Stop();

        SwitchToNewStreamUrl(clientInfo, _streamInformation);

        _streamInformation.FailoverInProgress = false;
    }

    private async Task LogRetryAndDelay(int retryCount, int maxRetries, int waitTime, CancellationToken token, string streamUrl)
    {
        if (token.IsCancellationRequested)
        {
            _logger.LogInformation("Stream was cancelled for: {StreamUrl}", setting.CleanURLs ? "url removed" : streamUrl);
        }

        _logger.LogInformation("Stream received 0 bytes for stream: {StreamUrl} Retry {retryCount}/{maxRetries}",
            setting.CleanURLs ? "url removed" : streamUrl,
            retryCount,
            maxRetries);

        await DelayWithCancellation(waitTime, token); // wait before retrying
    }

    private bool ProcessStreamFailure(string streamUrl, StreamerConfiguration clientInfo, CancellationToken cancellationToken, StreamInformation? _streamInformation)
    {
        if (ShouldHandleFailover(_streamInformation))
        {
            if (_streamInformation.FailoverInProgress)
            {
                return false;
            }

            if (_streamInformation.ClientCount == 0)
            {
                StopStreamForNoClients(_streamInformation, streamUrl);
                return true;
            }

            HandleStreamFailover(clientInfo, cancellationToken, _streamInformation);
        }

        return false;
    }

    private void RegisterClientsToNewStream(ICollection<Guid> clientIds, StreamInformation streamStreamInfo)
    {
        foreach (Guid clientId in clientIds)
        {
            RegisterClientToNewStream(clientId, streamStreamInfo);
        }
    }

    private void RegisterClientToNewStream(Guid clientId, StreamInformation streamStreamInfo)
    {
        var clientInfo = streamStreamInfo.GetStreamConfiguration(clientId);
        streamStreamInfo.RingBuffer.RegisterClient(clientId, clientInfo.ClientUserAgent);
        streamStreamInfo.SetClientBufferDelegate(clientId, () => streamStreamInfo.RingBuffer);
    }

    private bool ShouldHandleFailover(StreamInformation _streamInformation)
    {
        return _streamInformation.ClientCount == 0 || _streamInformation.StreamerCancellationToken.IsCancellationRequested || _streamInformation.StreamingTask.IsFaulted || _streamInformation.StreamingTask.IsCanceled;
    }

    private async Task StartVideoStreaming(
    Stream stream,
    string streamUrl,
    StreamerConfiguration clientInfo,
    CircularRingBuffer buffer,
    CancellationTokenSource cancellationToken)
    {
        var chunkSize = clientInfo.BufferSize;
        _logger.LogInformation("Starting video read streaming, chunk size is {ChunkSize}, for stream: {StreamUrl}",
            chunkSize,
            setting.CleanURLs ? "url removed" : streamUrl);
        byte[] bufferChunk = new byte[chunkSize];

        var maxRetries = clientInfo.MaxConnectRetry > 0 ? clientInfo.MaxConnectRetry : 3;
        var waitTime = clientInfo.MaxConnectRetryTimeMS > 0 ? clientInfo.MaxConnectRetryTimeMS : 50;

        using (stream)
        {
            var retryCount = 0;
            while (!cancellationToken.IsCancellationRequested && retryCount < maxRetries)
            {
                try
                {
                    var bytesRead = await TryReadStream(bufferChunk, cancellationToken.Token, stream);
                    if (bytesRead == -1)
                    {
                        break;
                    }
                    if (bytesRead == 0)
                    {
                        retryCount++;
                        await LogRetryAndDelay(retryCount, maxRetries, waitTime, cancellationToken.Token, streamUrl);
                    }
                    else
                    {
                        buffer.WriteChunk(bufferChunk, bytesRead);
                        retryCount = 0; // reset retry count on successful read
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Stream error for: {StreamUrl}", setting.CleanURLs ? "url removed" : streamUrl);
                    break;
                }
            }
        }

        _logger.LogInformation("Stream stopped for: {StreamUrl}", setting.CleanURLs ? "url removed" : streamUrl);
        cancellationToken.Cancel();
    }

    private void StopStreamForNoClients(StreamInformation _streamInformation, string streamUrl)
    {
        if (!_streamInformation.StreamerCancellationToken.IsCancellationRequested)
        {
            _streamInformation.StreamerCancellationToken.Cancel();
        }
        _logger.LogWarning("No more clients for : {StreamUrl}", setting.CleanURLs ? "url removed" : streamUrl);
    }

    private void SwitchToNewStreamUrl(StreamerConfiguration clientInfo, StreamInformation _streamInformation)
    {
        var clientIds = _streamInformation.RingBuffer.GetClientIds();

        _streamManager.RemoveStreamInfo(clientInfo.CurentVideoStream.User_Url);

        string? newStreamUrl = ChannelUtils.ChannelManager(clientInfo);
        if (newStreamUrl == null)
        {
            return;
        }

        StreamInformation? newStreamInfo = GetOrCreateBuffer(newStreamUrl, clientInfo);
        if (newStreamInfo == null)
        {
            return;
        }

        // Update the old StreamInformation's CancellationToken with the new one
        _streamInformation.StreamerCancellationToken = newStreamInfo.StreamerCancellationToken;


        RegisterClientsToNewStream(clientIds, newStreamInfo);

        _logger.LogInformation("Failover handled, switched to new stream URL: {NewStreamUrl}", setting.CleanURLs ? "url removed" : newStreamUrl);
    }

    private bool TryGetStreamInfo(string streamUrl, out StreamInformation? _streamInformation)
    {
        _streamInformation = _streamManager.GetStreamInformationFromStreamUrl(streamUrl);
        if (_streamInformation == null)
        {
            _logger.LogWarning("HandleFailover Stream stopped: {StreamUrl}", setting.CleanURLs ? "url removed" : streamUrl);
            return false;
        }

        return true;
    }

    private async Task<int> TryReadStream(byte[] bufferChunk, CancellationToken token, Stream stream)
    {
        try
        {
            if (!stream.CanRead || token.IsCancellationRequested)
            {
                _logger.LogWarning("Stream is not readable or cancelled");
                return -1;
            }

            return await stream.ReadAsync(bufferChunk);
        }
        catch (TaskCanceledException)
        {
            return -1;
        }
        catch (Exception)
        {
            throw;
        }
    }
}
