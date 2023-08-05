using AutoMapper;

using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using StreamMasterApplication.Common.Interfaces;
using StreamMasterApplication.Common.Models;
using StreamMasterApplication.Hubs;

using StreamMasterDomain.Common;
using StreamMasterDomain.Dto;
using StreamMasterDomain.Enums;
using StreamMasterDomain.Repository;

using System.Collections.Concurrent;
using System.Data;

namespace StreamMasterInfrastructure.VideoStreamManager;

public class ChannelManager : IDisposable, IChannelManager
{
    private readonly Timer _broadcastTimer;
    private readonly ConcurrentDictionary<string, ChannelStatus> _channelStatuses;
    private readonly IHubContext<StreamMasterHub, IStreamMasterHub> _hub;
    private readonly ILogger<ChannelManager> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IStreamManager _streamManager;

    public ChannelManager(ILogger<ChannelManager> logger, IServiceProvider serviceProvider, IHubContext<StreamMasterHub, IStreamMasterHub> hub, ILoggerFactory loggerFactory)
    {
        _logger = logger;
        _hub = hub;
        _streamManager = new StreamManager(loggerFactory);
        _broadcastTimer = new Timer(BroadcastMessage, null, 1000, 1000);
        _serviceProvider = serviceProvider;
        _channelStatuses = new();
    }

    private static Setting setting => FileUtil.GetSetting();

    public async Task ChangeVideoStreamChannel(string playingVideoStreamId, string newVideoStreamId)
    {
        _logger.LogDebug($"Starting ChangeVideoStreamChannel with playingVideoStreamId: {playingVideoStreamId} and newVideoStreamId: {newVideoStreamId}");

        if (_channelStatuses.TryGetValue(playingVideoStreamId, out ChannelStatus? channelStatus))
        {
            _logger.LogDebug($"Channel status found for playingVideoStreamId: {playingVideoStreamId}");

            IStreamInformation? oldInfo = channelStatus.StreamInformation;

            if (!await HandleNextVideoStream(channelStatus, newVideoStreamId))
            {
                _logger.LogWarning("Exiting ChangeVideoStreamChannel. Could not change channel to {newVideoStreamId}", newVideoStreamId);
                return;
            }

            if (oldInfo is not null && channelStatus.StreamInformation is not null)
            {
                List<Guid> clientIds = channelStatus.ClientIds.Values.ToList();
                foreach (Guid client in clientIds)
                {
                    ClientStreamerConfiguration? c = channelStatus.StreamInformation.GetStreamConfiguration(client);

                    if (c == null)
                    {
                        _logger.LogDebug($"Stream configuration is null for client: {client}, skipping unregistration");
                        continue;
                    }

                    oldInfo.UnRegisterStreamConfiguration(c);
                    _logger.LogDebug($"Unregistered stream configuration for client: {client}");
                }
            }
            _logger.LogWarning("Channel changed for {videoStreamId} switching video stream id to {newVideoStreamId}", playingVideoStreamId, newVideoStreamId);
            _logger.LogDebug($"Exiting ChangeVideoStreamChannel after channel change");

            return;
        }
        _logger.LogWarning("Channel not found: {videoStreamId}", playingVideoStreamId);
        _logger.LogDebug($"Exiting ChangeVideoStreamChannel due to channel not found");

        return;
    }

    public void Dispose()
    {
        _broadcastTimer?.Dispose();
        _channelStatuses.Clear();
        GC.SuppressFinalize(this);
    }

    public async Task FailClient(Guid clientId)
    {
        _logger.LogDebug($"Starting FailClient with clientId: {clientId}");

        foreach (ChannelStatus? channelStatus in _channelStatuses.Values.Where(a => a.StreamInformation is not null))
        {
            ClientStreamerConfiguration? c = channelStatus.StreamInformation.GetStreamConfiguration(clientId);

            if (c != null)
            {
                _logger.LogWarning("Failing client: {clientId}", clientId);
                c.CancellationTokenSource.Cancel();
                _logger.LogDebug($"Cancelled CancellationTokenSource for client: {clientId}");
                _logger.LogDebug($"Exiting FailClient after failing client: {clientId}");
                break;
            }
        }
        _logger.LogDebug($"Exiting FailClient, no matching client found for clientId: {clientId}");
    }

    public List<StreamStatisticsResult> GetAllStatisticsForAllUrls()
    {
        List<StreamStatisticsResult> allStatistics = new();

        ICollection<IStreamInformation> infos = _streamManager.GetStreamInformations();
        foreach (IStreamInformation? info in infos.Where(a => a.RingBuffer != null))
        {
            allStatistics.AddRange(info.RingBuffer.GetAllStatisticsForAllUrls());
        }

        return allStatistics;
    }

    public SingleStreamStatisticsResult GetSingleStreamStatisticsResult(string streamUrl)
    {
        return _streamManager.GetSingleStreamStatisticsResult(streamUrl);
    }

    public async Task<Stream?> GetStream(ClientStreamerConfiguration config)
    {
        return await RegisterClient(config);
    }

    public void RemoveClient(ClientStreamerConfiguration config)
    {
        UnRegisterClient(config);
    }

    public bool ShouldHandleFailover(ChannelStatus channelStatus)
    {
        if (channelStatus.StreamInformation is null)
        {
            return false;
        }

        IStreamInformation _streamInformation = channelStatus.StreamInformation;
        return _streamInformation.ClientCount == 0 || _streamInformation.VideoStreamingCancellationToken.IsCancellationRequested || _streamInformation.StreamingTask.IsFaulted || _streamInformation.StreamingTask.IsCanceled;
    }

    public void SimulateStreamFailure(string streamUrl)
    {
        IStreamInformation? _streamInformation = _streamManager.GetStreamInformationFromStreamUrl(streamUrl);

        if (_streamInformation is not null)
        {
            if (_streamInformation.VideoStreamingCancellationToken is not null && !_streamInformation.VideoStreamingCancellationToken.IsCancellationRequested)
            {
                _streamInformation.VideoStreamingCancellationToken.Cancel();
            }
            _logger.LogInformation("Simulating stream failure for: {StreamUrl}", setting.CleanURLs ? "url removed" : streamUrl);
        }
        else
        {
            _logger.LogWarning("Stream not found, cannot simulate stream failure: {StreamUrl}", setting.CleanURLs ? "url removed" : streamUrl);
        }
    }

    public void SimulateStreamFailureForAll()
    {
        foreach (IStreamInformation s in _streamManager.GetStreamInformations())
        {
            s.VideoStreamingCancellationToken.Cancel();
        }
    }

    private void BroadcastMessage(object? state)
    {
        _ = _hub.Clients.All.StreamStatisticsResultsUpdate(GetAllStatisticsForAllUrls());
    }

    private async Task ChannelWatcher(ChannelStatus channelStatus)
    {
        try
        {
            _logger.LogInformation("Channel watcher starting for channel: {videoStreamId}", channelStatus.VideoStreamId);

            bool quit = false;

            while (!channelStatus.ChannelWatcherToken.Token.IsCancellationRequested)
            {
                quit = await ProcessStreamStatus(channelStatus);
                if (quit)
                {
                    _logger.LogDebug($"Exiting ChannelWatcher loop due to quit condition for channel: {channelStatus.VideoStreamId}");
                    break;
                }

                await DelayWithCancellation(50, channelStatus.ChannelWatcherToken.Token);
            }
        }
        catch (TaskCanceledException)
        {
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Channel watcher error for channel: {videoStreamId}", channelStatus.VideoStreamId);
            _logger.LogDebug($"Exiting ChannelWatcher due to error for channel: {channelStatus.VideoStreamId}");
            return;
        }

        _logger.LogInformation("Channel watcher stopped for channel: {videoStreamId}", channelStatus.VideoStreamId);
        _logger.LogDebug($"Exiting ChannelWatcher for channel: {channelStatus.VideoStreamId}");
    }

    private async Task DelayWithCancellation(int milliseconds, CancellationToken cancellationToken)
    {
        try
        {
            await Task.Delay(milliseconds, cancellationToken);
        }
        catch (TaskCanceledException)
        {
            _logger.LogInformation("Task was cancelled");
            throw;
        }
    }

    private int GetGlobalStreamsCount()
    {
        return _channelStatuses.Count(a => a.Value.IsGlobal);
    }

    private async Task<ChildVideoStreamDto?> GetNextChildVideoStream(ChannelStatus channelStatus, string? overrideNextVideoStreamId = null)
    {
        _logger.LogDebug($"Starting GetNextChildVideoStream with channelStatus: {channelStatus.VideoStreamName} and overrideNextVideoStreamId: {overrideNextVideoStreamId}");

        using IServiceScope scope = _serviceProvider.CreateScope();
        // = scope.ServiceProvider.GetRequiredService<IAppDbContext>();
        IRepositoryWrapper repository = scope.ServiceProvider.GetRequiredService<IRepositoryWrapper>();
        IMapper mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
        M3UFile? m3uFile;
        int allStreamsCount = 0;

        if (!string.IsNullOrEmpty(overrideNextVideoStreamId))
        {
            VideoStream vs = await repository.VideoStream.GetVideoStreamByIdAsync(overrideNextVideoStreamId);
            if (vs == null)
            {
                _logger.LogError("GetNextChildVideoStream could not get videoStream for id {VideoStreamId}", overrideNextVideoStreamId);
                _logger.LogDebug($"Exiting GetNextChildVideoStream with null due to newVideoStream being null");
                return null;
            }

            ChildVideoStreamDto newVideoStream = mapper.Map<ChildVideoStreamDto>(vs);
            if (newVideoStream == null)
            {
                _logger.LogError("GetNextChildVideoStream could not get videoStream for id {VideoStreamId}", overrideNextVideoStreamId);
                _logger.LogDebug($"Exiting GetNextChildVideoStream with null due to newVideoStream being null");
                return null;
            }

            IEnumerable<M3UFile> m3uFilesRepo = await repository.M3UFile.GetAllM3UFilesAsync();

            m3uFile = m3uFilesRepo.FirstOrDefault(a => a.Id == newVideoStream.M3UFileId);
            if (m3uFile == null)
            {
                if (GetGlobalStreamsCount() >= setting.GlobalStreamLimit)
                {
                    _logger.LogInformation("Max Global stream count {GlobalStreamsCount} reached for stream: {StreamUrl}", GetGlobalStreamsCount(), setting.CleanURLs ? "url removed" : newVideoStream.User_Url);
                    return null;
                }

                SetIsGlobal(channelStatus);
                _logger.LogInformation("Global stream count {GlobalStreamsCount}", GetGlobalStreamsCount());
                return newVideoStream;
            }
            else
            {
                allStreamsCount = _streamManager.GetStreamsCountForM3UFile(newVideoStream.M3UFileId);

                if (newVideoStream.Id != channelStatus.VideoStreamId && allStreamsCount >= m3uFile.MaxStreamCount)
                {
                    _logger.LogInformation("Max stream count {MaxStreams} reached for stream: {StreamUrl}", newVideoStream.MaxStreams, setting.CleanURLs ? "url removed" : newVideoStream.User_Url);
                }
                else
                {
                    _logger.LogDebug($"Exiting GetNextChildVideoStream with newVideoStream: {newVideoStream}");
                    return newVideoStream;
                }
            }
        }

        (VideoStreamHandlers videoStreamHandler, List<ChildVideoStreamDto> childVideoStreamDtos)? result = await repository.VideoStream.GetStreamsFromVideoStreamById(channelStatus.VideoStreamId);
        if (result == null)
        {
            _logger.LogError("GetNextChildVideoStream could not get videoStream for id {VideoStreamId}", channelStatus.VideoStreamId);
            _logger.LogDebug($"Exiting GetNextChildVideoStream with null due to result being null");
            return null;
        }

        ChildVideoStreamDto[] videoStreams = result.Value.childVideoStreamDtos.OrderBy(a => a.Rank).ToArray();
        if (!videoStreams.Any())
        {
            _logger.LogError("GetNextChildVideoStream could not get child videoStreams for id {VideoStreamId}", channelStatus.VideoStreamId);
            _logger.LogDebug($"Exiting GetNextChildVideoStream with null due to no child videoStreams found");
            return null;
        }

        VideoStreamHandlers videoHandler = result.Value.videoStreamHandler == VideoStreamHandlers.SystemDefault ? VideoStreamHandlers.Loop : result.Value.videoStreamHandler;

        if (channelStatus.Rank >= videoStreams.Length)
        {
            channelStatus.Rank = 0;
        }

        while (channelStatus.Rank < videoStreams.Length)
        {
            ChildVideoStreamDto toReturn = videoStreams[channelStatus.Rank++];
            IEnumerable<M3UFile> m3uFilesRepo = await repository.M3UFile.GetAllM3UFilesAsync();

            m3uFile = m3uFilesRepo.FirstOrDefault(a => a.Id == toReturn.M3UFileId);
            if (m3uFile == null)
            {
                if (GetGlobalStreamsCount() >= setting.GlobalStreamLimit)
                {
                    _logger.LogInformation("Max Global stream count {GlobalStreamsCount} reached for stream: {StreamUrl}", GetGlobalStreamsCount(), setting.CleanURLs ? "url removed" : toReturn.User_Url);
                    continue;
                }

                SetIsGlobal(channelStatus);
                _logger.LogInformation("Global stream count {GlobalStreamsCount}", GetGlobalStreamsCount());
            }
            else
            {
                allStreamsCount = _streamManager.GetStreamsCountForM3UFile(toReturn.M3UFileId);
                if (allStreamsCount >= m3uFile.MaxStreamCount)
                {
                    _logger.LogInformation("Max stream count {MaxStreams} reached for stream: {StreamUrl}", toReturn.MaxStreams, setting.CleanURLs ? "url removed" : toReturn.User_Url);
                    continue;
                }
            }

            _logger.LogDebug($"Exiting GetNextChildVideoStream with toReturn: {toReturn}");
            return toReturn;
        }

        _logger.LogDebug($"Exiting GetNextChildVideoStream with null due to no suitable videoStream found");
        return null;
    }

    private async Task<bool> HandleNextVideoStream(ChannelStatus channelStatus, string? overrideNextVideoStreamId = null)
    {
        _logger.LogDebug($"Starting HandleNextVideoStream with channelStatus: {channelStatus} and overrideNextVideoStreamId: {overrideNextVideoStreamId}");

        channelStatus.FailoverInProgress = true;

        DelayWithCancellation(200, channelStatus.ChannelWatcherToken.Token).Wait();

        ChildVideoStreamDto? childVideoStreamDto = await GetNextChildVideoStream(channelStatus, overrideNextVideoStreamId);
        if (childVideoStreamDto is null)
        {
            _logger.LogDebug($"Exiting HandleNextVideoStream with false due to childVideoStreamDto being null");

            channelStatus.FailoverInProgress = false;
            return false;
        }

        ICollection<ClientStreamerConfiguration>? oldConfigs = null;
        if (channelStatus.StreamInformation is not null)
        {
            oldConfigs = channelStatus.StreamInformation.GetStreamConfigurations();
        }

        channelStatus.StreamInformation = await _streamManager.GetOrCreateBuffer(childVideoStreamDto, channelStatus.VideoStreamId, channelStatus.VideoStreamName, channelStatus.Rank);

        if (channelStatus.StreamInformation is null)
        {
            _logger.LogDebug($"Exiting HandleNextVideoStream with false due to channelStatus.StreamInformation being null");

            channelStatus.FailoverInProgress = false;
            return false;
        }

        if (oldConfigs is not null)
        {
            RegisterClientsToNewStream(oldConfigs, channelStatus.StreamInformation);
        }

        channelStatus.FailoverInProgress = false;
        _logger.LogDebug($"Finished HandleNextVideoStream");

        return true;
    }

    private async Task<bool> ProcessStreamStatus(ChannelStatus channelStatus)
    {
        if (channelStatus.ChannelWatcherToken.Token.IsCancellationRequested)
        {
            _logger.LogDebug($"ChannelWatcherToken cancellation requested for channelStatus: {channelStatus}");
            _logger.LogDebug($"Exiting ProcessStreamStatus with true due to ChannelWatcherToken cancellation request");
            return true;
        }

        if (channelStatus.StreamInformation is null)
        {
            _logger.LogDebug($"ChannelStatus StreamInformation is null for channelStatus: {channelStatus}, attempting to handle next video stream");
            bool handled = await HandleNextVideoStream(channelStatus);
            _logger.LogDebug($"Exiting ProcessStreamStatus with {!handled} after handling next video stream");
            return !handled;
        }

        if (channelStatus.StreamInformation.VideoStreamingCancellationToken.IsCancellationRequested)
        {
            _logger.LogDebug($"VideoStreamingCancellationToken cancellation requested for channelStatus: {channelStatus}, stopping stream and attempting to handle next video stream");
            _streamManager.Stop(channelStatus.StreamInformation.StreamUrl);
            bool handled = await HandleNextVideoStream(channelStatus);
            _logger.LogDebug($"Exiting ProcessStreamStatus with {!handled} after stopping streaming and handling next video stream");
            return !handled;
        }

        return false;
    }

    private async Task<Stream?> RegisterClient(ClientStreamerConfiguration config)
    {
        _logger.LogDebug($"Starting RegisterClient with config: {config}");

        ChannelStatus? channelStatus = await RegisterWithChannelManager(config);
        if (channelStatus is null || config.ReadBuffer is null)
        {
            _logger.LogDebug($"Exiting RegisterClient with null due to channelStatus or config.ReadBuffer being null");
            return null;
        }

        _logger.LogDebug($"Finished RegisterClient, returning config.ReadBuffer");
        return (Stream)config.ReadBuffer;
    }

    private void RegisterClientsToNewStream(ICollection<ClientStreamerConfiguration> configs, IStreamInformation streamStreamInfo)
    {
        foreach (ClientStreamerConfiguration config in configs)
        {
            RegisterClientToNewStream(config, streamStreamInfo);
        }
    }

    private void RegisterClientToNewStream(ClientStreamerConfiguration config, IStreamInformation streamStreamInfo)
    {
        _logger.LogInformation("Registered client id: {clientId} to videostream url {StreamUrl}", config.ClientId, setting.CleanURLs ? "url removed" : streamStreamInfo.StreamUrl);

        streamStreamInfo.RegisterStreamConfiguration(config);
    }

    private async Task<ChannelStatus?> RegisterWithChannelManager(ClientStreamerConfiguration config)
    {
        _logger.LogDebug($"Starting RegisterWithChannelManager with config: {config}");

        ChannelStatus channelStatus;
        if (!_channelStatuses.ContainsKey(config.VideoStreamId))
        {
            using IServiceScope scope = _serviceProvider.CreateScope();
            IRepositoryWrapper repository = scope.ServiceProvider.GetRequiredService<IRepositoryWrapper>();
            VideoStream? videoStream = await repository.VideoStream.GetVideoStreamByIdAsync(config.VideoStreamId);


            if (videoStream is null)
                return null;

            config.VideoStreamName = videoStream.User_Tvg_name;
            channelStatus = new ChannelStatus(config.VideoStreamId, config.VideoStreamName);

            if (!await HandleNextVideoStream(channelStatus).ConfigureAwait(false) || channelStatus.StreamInformation is null)
            {
                return null;
            }

            _channelStatuses.TryAdd(config.VideoStreamId, channelStatus);
            _ = ChannelWatcher(channelStatus).ConfigureAwait(false);
        }
        else
        {
            channelStatus = _channelStatuses[config.VideoStreamId];
        }

        if (channelStatus.StreamInformation is null)
        {
            _logger.LogError($"Failed to get or create buffer for childVideoStreamDto in RegisterWithChannelManager");
            _logger.LogDebug($"Exiting RegisterWithChannelManager with null due to failure to get or create buffer");
            return null;
        }

        channelStatus.ClientIds.TryAdd(config.ClientId, config.ClientId);

        _logger.LogInformation("ChannelManager added channel: {videoStreamId}", config.VideoStreamId);

        config.ReadBuffer = new RingBufferReadStream(() => channelStatus.StreamInformation.RingBuffer, config);

        channelStatus.StreamInformation.RegisterStreamConfiguration(config);

        if (channelStatus is null)
        {
            _logger.LogDebug($"Exiting RegisterWithChannelManager with null due to channelStatus being null");
            return null;
        }

        _logger.LogDebug($"Finished RegisterWithChannelManager with config: {config}");

        return channelStatus;
    }

    private void SetIsGlobal(ChannelStatus channelStatus)
    {
        channelStatus.IsGlobal = true;
        _channelStatuses.TryUpdate(channelStatus.VideoStreamId, channelStatus, channelStatus);
    }

    private void UnRegisterClient(ClientStreamerConfiguration config)
    {
        UnRegisterWithChannelManager(config);
    }

    private async Task UnRegisterWithChannelManager(ClientStreamerConfiguration config)
    {
        _logger.LogDebug($"Starting UnRegisterWithChannelManager with config: {config}");

        if (!_channelStatuses.ContainsKey(config.VideoStreamId))
        {
            _logger.LogDebug($"Exiting UnRegisterWithChannelManager due to VideoStreamId not found in _channelStatuses");
            return;
        }

        ChannelStatus channelStatus = _channelStatuses[config.VideoStreamId];

        if (channelStatus.StreamInformation is not null)
        {
            channelStatus.StreamInformation.UnRegisterStreamConfiguration(config);

            if (channelStatus.StreamInformation.ClientCount == 0)
            {
                channelStatus.ChannelWatcherToken.Cancel();

                if (channelStatus.StreamInformation is not null)
                {
                    _streamManager.Stop(channelStatus.StreamInformation.StreamUrl);
                }

                _channelStatuses.TryRemove(config.VideoStreamId, out _);
                _logger.LogInformation("ChannelManager No more clients, stopping streaming for {videoStreamId}", config.VideoStreamId);
                _logger.LogDebug($"Exiting UnRegisterWithChannelManager after stopping streaming due to no more clients");
            }
        }

        channelStatus.ClientIds.TryRemove(config.ClientId, out _);
        _logger.LogDebug($"Finished UnRegisterWithChannelManager with config: {config}");
    }
}
