using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using StreamMasterApplication.Common.Interfaces;
using StreamMasterApplication.Common.Models;
using StreamMasterApplication.Hubs;

using StreamMasterDomain.Common;
using StreamMasterDomain.Dto;
using StreamMasterDomain.Extensions;
using StreamMasterDomain.Repository;
using StreamMasterDomain.Services;

using System.Data;
using System.Net;
using System.Net.Sockets;

namespace StreamMasterInfrastructure.VideoStreamManager;

public class ChannelManager : IDisposable, IChannelManager
{
    private readonly Timer _broadcastTimer;
    private readonly IHubContext<StreamMasterHub, IStreamMasterHub> _hub;
    private readonly ILogger<ChannelManager> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IStreamManager _streamManager;
    private readonly ISettingsService _settingsService;
    private readonly IChannelService _channelService;
    private readonly IStreamSwitcher _streamSwitcher;

    public ChannelManager(ILogger<ChannelManager> logger, IStreamSwitcher streamSwitcher, IChannelService channelService, IStreamManager streamManager, ISettingsService settingsService, IServiceProvider serviceProvider, IHubContext<StreamMasterHub, IStreamMasterHub> hub)
    {
        _settingsService = settingsService;
        _logger = logger;
        _hub = hub;
        _streamManager = streamManager;
        _broadcastTimer = new Timer(BroadcastMessage, null, 1000, 1000);
        _serviceProvider = serviceProvider;
        _channelService = channelService;
        _streamSwitcher = streamSwitcher;
    }
    public async Task ChangeVideoStreamChannel(string playingVideoStreamId, string newVideoStreamId)
    {
        _logger.LogDebug("Starting ChangeVideoStreamChannel with playingVideoStreamId: {playingVideoStreamId} and newVideoStreamId: {newVideoStreamId}", playingVideoStreamId, newVideoStreamId);

        IChannelStatus? channelStatus = _channelService.GetChannelStatus(playingVideoStreamId);

        if (channelStatus != null)
        {
            _logger.LogDebug("Channel status found for playingVideoStreamId: {playingVideoStreamId}", playingVideoStreamId);

            IStreamHandler? oldInfo = channelStatus.StreamHandler;

            if (!await _streamSwitcher.SwitchToNextVideoStreamAsync(channelStatus, newVideoStreamId))
            {
                _logger.LogWarning("Exiting ChangeVideoStreamChannel. Could not change channel to {newVideoStreamId}", newVideoStreamId);
                return;
            }

            if (oldInfo is not null && channelStatus.StreamHandler is not null)
            {
                foreach (Guid clientID in channelStatus.GetChannelClientIds)
                {
                    ClientStreamerConfiguration? c = channelStatus.StreamHandler.GetClientStreamerConfiguration(clientID);

                    if (c == null)
                    {
                        _logger.LogDebug("Stream configuration is null for client: {clientID}, skipping unregistration", clientID);
                        continue;
                    }

                    _ = oldInfo.UnRegisterClientStreamer(c);
                    _logger.LogDebug("Unregistered stream configuration for client: {clientID}", clientID);
                }
            }
            _logger.LogWarning("Channel changed for {videoStreamId} switching video stream id to {newVideoStreamId}", playingVideoStreamId, newVideoStreamId);
            _logger.LogDebug("Exiting ChangeVideoStreamChannel after channel change");

            return;
        }
        _logger.LogWarning("Channel not found: {videoStreamId}", playingVideoStreamId);
        _logger.LogDebug("Exiting ChangeVideoStreamChannel due to channel not found");

        return;
    }

    public void Dispose()
    {
        _broadcastTimer?.Dispose();
        GC.SuppressFinalize(this);
    }

    public void FailClient(Guid clientId)
    {
        _logger.LogDebug("Starting FailClient with clientId: {clientId}", clientId);

        foreach (IStreamHandler streamHandler in _channelService.GetStreamHandlers())
        {
            ClientStreamerConfiguration? c = streamHandler.GetClientStreamerConfiguration(clientId);

            if (c != null)
            {
                c.ClientMasterToken.Cancel();
                _logger.LogWarning("Failing client: {clientId}", clientId);
                _logger.LogDebug("Cancelled CancellationTokenSource for client: {clientId}", clientId);
                _logger.LogDebug("Exiting FailClient after failing client: {clientId}", clientId);
                break;
            }
        }
        _logger.LogDebug("Exiting FailClient, no matching client found for clientId: {clientId}", clientId);
    }

    public async Task<List<StreamStatisticsResult>> GetAllStatisticsForAllUrls()
    {
        List<StreamStatisticsResult> allStatistics = new();

        ICollection<IStreamHandler> infos = _streamManager.GetStreamInformations();
        foreach (IStreamHandler? info in infos.Where(a => a.RingBuffer != null))
        {
            allStatistics.AddRange(info.RingBuffer.GetAllStatisticsForAllUrls());
        }

        Setting settings = await _settingsService.GetSettingsAsync();

        if (settings.ShowClientHostNames)
        {
            foreach (StreamStatisticsResult streamStatisticsResult in allStatistics)
            {
                try
                {
                    IPHostEntry host = await Dns.GetHostEntryAsync(streamStatisticsResult.ClientIPAddress).ConfigureAwait(false);
                    streamStatisticsResult.ClientIPAddress = host.HostName;
                }
                catch (SocketException)
                {
                }
                catch (ArgumentException)
                {
                }
            }
        }

        return allStatistics;
    }

    public async Task<Stream?> GetStream(ClientStreamerConfiguration config)
    {
        return await RegisterClient(config);
    }

    public void RemoveClient(ClientStreamerConfiguration config)
    {
        UnRegisterWithChannelManager(config);
    }

    public static bool ShouldHandleFailover(ChannelStatus channelStatus)
    {
        if (channelStatus.StreamHandler is null)
        {
            return false;
        }

        IStreamHandler _streamInformation = channelStatus.StreamHandler;
        return _streamInformation.ClientCount == 0 || _streamInformation.VideoStreamingCancellationToken.IsCancellationRequested;
    }

    public void SimulateStreamFailure(string streamUrl)
    {
        IStreamHandler? _streamInformation = _streamManager.GetStreamInformationFromStreamUrl(streamUrl);

        if (_streamInformation is not null)
        {
            if (_streamInformation.VideoStreamingCancellationToken?.IsCancellationRequested == false)
            {
                _streamInformation.VideoStreamingCancellationToken.Cancel();
            }
            _logger.LogInformation("Simulating stream failure for: {StreamUrl}", streamUrl);
        }
        else
        {
            _logger.LogWarning("Stream not found, cannot simulate stream failure: {StreamUrl}", streamUrl);
        }
    }

    public void SimulateStreamFailureForAll()
    {
        foreach (IStreamHandler s in _streamManager.GetStreamInformations())
        {
            s.VideoStreamingCancellationToken.Cancel();
        }
    }

    private void BroadcastMessage(object? state)
    {
        _ = _hub.Clients.All.StreamStatisticsResultsUpdate(GetAllStatisticsForAllUrls().Result);
    }

    private async Task ChannelWatcher(IChannelStatus channelStatus)
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
                    _logger.LogDebug("Exiting ChannelWatcher loop due to quit condition for channel: {VideoStreamId}", channelStatus.VideoStreamId);
                    break;
                }

                await channelStatus.ChannelWatcherToken.Token.ApplyDelay(50);
            }
        }
        catch (TaskCanceledException)
        {
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Channel watcher error for channel: {videoStreamId}", channelStatus.VideoStreamId);
            _logger.LogDebug("Exiting ChannelWatcher due to error for channel: {VideoStreamId}", channelStatus.VideoStreamId);
            return;
        }

        _logger.LogInformation("Channel watcher stopped for channel: {videoStreamId}", channelStatus.VideoStreamId);
        _logger.LogDebug("Exiting ChannelWatcher for channel: {VideoStreamId}", channelStatus.VideoStreamId);
    }


    private async Task<bool> ProcessStreamStatus(IChannelStatus channelStatus)
    {
        if (channelStatus.ChannelWatcherToken.Token.IsCancellationRequested)
        {
            _logger.LogDebug("ChannelWatcherToken cancellation requested for channelStatus: {channelStatus}", channelStatus);
            _logger.LogDebug("Exiting ProcessStreamStatus with true due to ChannelWatcherToken cancellation request");
            return true;
        }

        if (channelStatus.StreamHandler is null)
        {
            _logger.LogDebug("ChannelStatus StreamInformation is null for channelStatus: {channelStatus}, attempting to handle next video stream", channelStatus);
            bool handled;
            try
            {
                handled = await _streamSwitcher.SwitchToNextVideoStreamAsync(channelStatus);
            }
            catch (TaskCanceledException)
            {
                _logger.LogInformation("Task was cancelled");
                throw;
            }

            _logger.LogDebug("Exiting ProcessStreamStatus with {handled} after handling next video stream", !handled);
            return !handled;
        }

        if (channelStatus.StreamHandler.VideoStreamingCancellationToken.IsCancellationRequested)
        {
            _logger.LogDebug("VideoStreamingCancellationToken cancellation requested for channelStatus: {channelStatus}, stopping stream and attempting to handle next video stream", channelStatus);
            _ = _streamManager.Stop(channelStatus.StreamHandler.StreamUrl);
            try
            {
                bool handled = await _streamSwitcher.SwitchToNextVideoStreamAsync(channelStatus);
                _logger.LogDebug("Exiting ProcessStreamStatus with {handled} after stopping streaming and handling next video stream", !handled);
                return !handled;
            }
            catch (TaskCanceledException)
            {
                _logger.LogInformation("Task was cancelled");
                throw;
            }
        }

        return false;
    }

    private async Task<Stream?> RegisterClient(ClientStreamerConfiguration config)
    {
        _logger.LogDebug("Starting RegisterClient with config: {config}", config.ClientId);

        IChannelStatus? channelStatus = await RegisterWithChannelManager(config);
        if (channelStatus is null || config.ReadBuffer is null)
        {
            _logger.LogDebug("Exiting RegisterClient with null due to channelStatus or config.ReadBuffer being null");
            return null;
        }

        _logger.LogDebug("Finished RegisterClient, returning config.ReadBuffer");
        return (Stream)config.ReadBuffer;
    }

    private async Task<IChannelStatus?> RegisterWithChannelManager(ClientStreamerConfiguration config)
    {
        _logger.LogDebug("Starting RegisterWithChannelManager with config: {config}", config.ClientId);

        IChannelStatus? channelStatus;
        if (!_channelService.HasChannel(config.VideoStreamId))
        {
            using IServiceScope scope = _serviceProvider.CreateScope();
            IRepositoryWrapper repository = scope.ServiceProvider.GetRequiredService<IRepositoryWrapper>();
            VideoStreamDto? videoStream = await repository.VideoStream.GetVideoStreamById(config.VideoStreamId);

            if (videoStream is null)
            {
                return null;
            }

            config.VideoStreamName = videoStream.User_Tvg_name;
            channelStatus = new ChannelStatus(config.VideoStreamId, config.VideoStreamName);

            try
            {
                if (!await _streamSwitcher.SwitchToNextVideoStreamAsync(channelStatus).ConfigureAwait(false) || channelStatus.StreamHandler is null)
                {
                    return null;
                }
            }
            catch (TaskCanceledException)
            {
                _logger.LogInformation("Task was cancelled");
                throw;
            }

            _channelService.RegisterChannel(config.VideoStreamId, config.VideoStreamName);
            _ = ChannelWatcher(channelStatus).ConfigureAwait(false);
        }
        else
        {
            channelStatus = _channelService.GetChannelStatus(config.VideoStreamId);
        }

        if (channelStatus is null)
        {
            _logger.LogDebug("Exiting RegisterWithChannelManager with null due to channelStatus being null");
            return null;
        }

        if (channelStatus.StreamHandler is null)
        {
            _logger.LogError("Failed to get or create buffer for childVideoStreamDto in RegisterWithChannelManager");
            _logger.LogDebug("Exiting RegisterWithChannelManager with null due to failure to get or create buffer");
            return null;
        }

        channelStatus.AddToClientIds(config.ClientId);

        _logger.LogInformation("ChannelManager added channel: {videoStreamId}", config.VideoStreamId);

        config.ReadBuffer = new RingBufferReadStream(() => channelStatus.StreamHandler.RingBuffer, config);

        channelStatus.StreamHandler.RegisterClientStreamer(config);

        _logger.LogDebug("Finished RegisterWithChannelManager with config: {config}", config.ClientId);

        return channelStatus;
    }

    private void UnRegisterWithChannelManager(ClientStreamerConfiguration config)
    {
        _logger.LogDebug("Starting UnRegisterWithChannelManager with config: {config}", config.ClientId);

        if (!_channelService.HasChannel(config.VideoStreamId))
        {
            _logger.LogDebug("Exiting UnRegisterWithChannelManager due to VideoStreamId not found in _channelStatuses");
            return;
        }

        IChannelStatus? channelStatus = _channelService.GetChannelStatus(config.VideoStreamId);

        if (channelStatus is not null)
        {
            if (channelStatus.StreamHandler is not null)
            {
                _ = channelStatus.StreamHandler.UnRegisterClientStreamer(config);

                if (channelStatus.StreamHandler.ClientCount == 0)
                {
                    channelStatus.ChannelWatcherToken.Cancel();

                    if (channelStatus.StreamHandler is not null)
                    {
                        _ = _streamManager.Stop(channelStatus.StreamHandler.StreamUrl);
                    }
                    _channelService.UnregisterChannel(config.VideoStreamId);
                    _logger.LogInformation("ChannelManager No more clients, stopping streaming for {videoStreamId}", config.VideoStreamId);
                    _logger.LogDebug("Exiting UnRegisterWithChannelManager after stopping streaming due to no more clients");
                }
            }

            channelStatus.RemoveClientId(config.ClientId);
        }
        _logger.LogDebug("Finished UnRegisterWithChannelManager with config: {config}", config.ClientId);
    }
}