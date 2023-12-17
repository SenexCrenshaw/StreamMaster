using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using StreamMasterApplication.Common.Interfaces;

using StreamMasterDomain.Common;
using StreamMasterDomain.Dto;
using StreamMasterDomain.Extensions;
using StreamMasterDomain.Repository;
using StreamMasterDomain.Services;

namespace StreamMasterInfrastructure.VideoStreamManager.Channels;

public sealed class ChannelManager(
    ILogger<ChannelManager> logger,
    IBroadcastService broadcastService,
    IStreamSwitcher streamSwitcher,
    IChannelService channelService,
    IStreamManager streamManager,
    IClientStreamerManager clientStreamerManager,
    ISettingsService settingService,
    IServiceProvider serviceProvider
    ) : IChannelManager
{
    private readonly SemaphoreSlim _registerSemaphore = new(1, 1);
    private readonly SemaphoreSlim _unregisterSemaphore = new(1, 1);

    private static CancellationTokenSource ChannelWatcherToken = new();
    private static bool ChannelWatcherStarted;

    public async Task ChangeVideoStreamChannel(string playingVideoStreamId, string newVideoStreamId)
    {
        logger.LogDebug("Starting ChangeVideoStreamChannel with playingVideoStreamId: {playingVideoStreamId} and newVideoStreamId: {newVideoStreamId}", playingVideoStreamId, newVideoStreamId);

        foreach (IChannelStatus channelStatus in channelService.GetChannelStatusesFromVideoStreamId(playingVideoStreamId))
        {
            if (channelStatus != null)
            {
                if (!await streamSwitcher.SwitchToNextVideoStreamAsync(channelStatus.ChannelVideoStreamId, newVideoStreamId))
                {
                    logger.LogWarning("Exiting ChangeVideoStreamChannel. Could not change channel to {newVideoStreamId}", newVideoStreamId);
                    return;
                }
                return;
            }
        }
        logger.LogWarning("Channel not found: {videoStreamId}", playingVideoStreamId);
        logger.LogDebug("Exiting ChangeVideoStreamChannel due to channel not found");

        return;
    }
    private readonly object _disposeLock = new();
    private bool _disposed = false;

    public void Dispose()
    {
        lock (_disposeLock)
        {
            if (_disposed)
            {
                return;
            }

            try
            {

                broadcastService.StopBroadcasting();
                ChannelWatcherToken.Cancel();
                ChannelWatcherToken.Dispose();
                streamManager.Dispose();
                channelService.Dispose();
                clientStreamerManager.Dispose();
                _registerSemaphore.Dispose();
                _unregisterSemaphore.Dispose();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred during disposing of StreamManager");
            }
            finally
            {
                _disposed = true;
            }
        }

    }
    public void FailClient(Guid clientId)
    {
        _ = clientStreamerManager.CancelClient(clientId);
    }

    public async Task<Stream?> GetChannel(IClientStreamerConfiguration config)
    {
        if (config.ClientMasterToken.IsCancellationRequested)
        {
            logger.LogInformation("Exiting GetChannel due to ClientMasterToken being cancelled");
            return null;
        }

        Stream? res = await RegisterClientAndGetStream(config);
        if (res is null)
        {
            logger.LogInformation("Exiting GetChannel due to RegisterClient returning null");
            return null;
        }

        broadcastService.StartBroadcasting();
        return res;
    }

    public async Task RemoveClient(IClientStreamerConfiguration config)
    {
        await UnRegisterWithChannelManager(config);
    }

    public void SimulateStreamFailure(string streamUrl)
    {
        IStreamHandler? handler = streamManager.GetStreamHandlerFromStreamUrl(streamUrl);

        if (handler is not null)
        {
            if (handler.VideoStreamingCancellationToken?.IsCancellationRequested == false)
            {
                handler.VideoStreamingCancellationToken.Cancel();
            }
            logger.LogInformation("Simulating stream failure for: {StreamUrl}", streamUrl);
        }
        else
        {
            logger.LogWarning("Stream not found, cannot simulate stream failure: {StreamUrl}", streamUrl);
        }
    }

    public void SimulateStreamFailureForAll()
    {
        foreach (IStreamHandler s in streamManager.GetStreamHandlers())
        {
            s.VideoStreamingCancellationToken.Cancel();
        }
    }

    private async Task ChannelWatcher()
    {
        ChannelWatcherStarted = true;
        logger.LogInformation("Channel watcher starting");
        try
        {
            while (!ChannelWatcherToken.IsCancellationRequested)
            {
                foreach (IChannelStatus channelStatus in channelService.GetChannelStatuses())
                {
                    if (channelStatus.FailoverInProgress)
                    {
                        continue;
                    }

                    if (channelStatus.IsStarted)
                    {
                        //    int clientCount = clientStreamerManager.ClientCount(channelStatus.CurrentVideoStream.Id);
                        //    if (clientCount == 0)
                        //    {
                        //        continue;
                        //    }

                        bool hasClients = await CheckAndCleanUpClientStreamers(channelStatus);
                        if (!hasClients)
                        {
                            continue;
                        }
                    }

                    bool handled = await ProcessStreamStatus(channelStatus);
                    if (!handled)
                    {
                        channelService.UnRegisterChannel(channelStatus.ChannelVideoStreamId);
                        logger.LogDebug("ChannelWatcher ProcessStreamStatus not handled for : {ChannelName} {VideoStreamId}", channelStatus.ChannelName, channelStatus.CurrentVideoStream.Id);
                        continue;
                    }
                }

                if (channelService.GetChannelStatuses().Count == 0)
                {
                    logger.LogInformation("Exiting ChannelWatcher loop due to no channels");
                    ChannelWatcherToken.Cancel();
                    break;
                }
                _ = await ChannelWatcherToken.Token.ApplyDelay(25);
            }

        }
        catch (TaskCanceledException)
        {
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Channel watcher error");
        }

        ChannelWatcherStarted = false;

        logger.LogInformation("Channel watcher stopped");
    }

    private async Task<bool> CheckAndCleanUpClientStreamers(IChannelStatus channelStatus)
    {
        List<IClientStreamerConfiguration> a = clientStreamerManager.GetClientStreamerConfigurationsByChannelVideoStreamId(channelStatus.ChannelVideoStreamId);

        if (a.Any())
        {
            foreach (IClientStreamerConfiguration client in clientStreamerManager.GetAllClientStreamerConfigurations)
            {
                if (client.ClientMasterToken.IsCancellationRequested)
                {
                    logger.LogInformation("Client {clientId} is finished", client.ClientId);
                    await UnRegisterWithChannelManager(client);
                }
            }
        }

        a = clientStreamerManager.GetClientStreamerConfigurationsByChannelVideoStreamId(channelStatus.ChannelVideoStreamId);
        if (!a.Any())
        {
            logger.LogInformation("No clients for channel: {videoStreamId}", channelStatus.CurrentVideoStream.Id);
            logger.LogDebug("Exiting CheckClients for channel: {VideoStreamId}", channelStatus.CurrentVideoStream.Id);
            _ = streamManager.StopAndUnRegisterHandler(channelStatus.CurrentVideoStream.User_Url);

            return false;
        }
        return true;
    }

    private async Task<bool> HandleChannelVideoStreamSwitch(IChannelStatus channelStatus)
    {
        logger.LogDebug("HandleChannelVideoStreamSwitch for channelStatus: {channelName} {channelStatus}", channelStatus.CurrentVideoStream.User_Tvg_name, channelStatus.CurrentVideoStream.Id);

        Setting setting = await settingService.GetSettingsAsync().ConfigureAwait(false);

        int maxRetries = setting.MaxConnectRetry > 0 ? setting.MaxConnectRetry : 3;
        int waitTime = setting.MaxConnectRetryTimeMS > 0 ? setting.MaxConnectRetryTimeMS : 50;

        try
        {
            bool handled = false;
            while (!handled || maxRetries > 0)
            {
                handled = await streamSwitcher.SwitchToNextVideoStreamAsync(channelStatus.ChannelVideoStreamId);
                if (handled)
                {
                    break;
                }
                await Task.Delay(waitTime);
                maxRetries--;
            }

            logger.LogDebug("HandleChannelVideoStreamSwitch handled: {handled}", handled);
            return handled;
        }
        catch (TaskCanceledException)
        {
            logger.LogInformation("Task was cancelled");
            throw;
        }
    }

    private async Task<bool> ProcessStreamStatus(IChannelStatus channelStatus)
    {
        if (channelStatus.IsStarted)
        {
            IStreamHandler? streamHandler = streamManager.GetStreamHandler(channelStatus.CurrentVideoStream.User_Url);
            if (streamHandler == null)
            {
                return false;
            }

            if (!streamHandler.VideoStreamingCancellationToken.IsCancellationRequested)
            {
                return true;
            }

            if (streamHandler.IsFailed)
            {
                return false;
            }

            streamHandler.SetFailed();
        }

        return await HandleChannelVideoStreamSwitch(channelStatus).ConfigureAwait(false);
    }

    private async Task<Stream?> RegisterClientAndGetStream(IClientStreamerConfiguration config)
    {
        clientStreamerManager.RegisterClient(config);


        IChannelStatus? channelStatus = await RegisterWithChannelManager(config);
        if (channelStatus is null || config.ReadBuffer is null)
        {
            channelService.UnRegisterChannel(config.ChannelVideoStreamId);
            logger.LogInformation("Exiting Register Client with null due to channelStatus or Read Buffer being null");
            return null;
        }

        logger.LogInformation("Finished Register Client");
        return (Stream)config.ReadBuffer;
    }

    private async Task<IChannelStatus?> RegisterWithChannelManager(IClientStreamerConfiguration config)
    {
        await _registerSemaphore.WaitAsync();
        try
        {

            IChannelStatus? channelStatus = await EnsureChannelRegistration(config);

            if (channelStatus == null)
            {
                logger.LogError("Failed to register with channel manager. channelStatus is null");
                return null;
            }

            if (!ChannelWatcherStarted)
            {
                ChannelWatcherToken = new();
                _ = ChannelWatcher().ConfigureAwait(false);
            }

            logger.LogDebug("Finished RegisterWithChannelManager with config: {config}", config.ClientId);
            return channelStatus;
        }
        finally
        {
            _ = _registerSemaphore.Release();
        }
    }

    private async Task<IChannelStatus?> EnsureChannelRegistration(IClientStreamerConfiguration config)
    {
        using IServiceScope scope = serviceProvider.CreateScope();
        IRepositoryWrapper repository = scope.ServiceProvider.GetRequiredService<IRepositoryWrapper>();
        VideoStreamDto? channelVideoStream = await repository.VideoStream.GetVideoStreamById(config.ChannelVideoStreamId);

        if (channelVideoStream == null)
        {
            logger.LogError("Could not find video stream for {ClientId} {ChannelVideoStreamId}", config.ClientId, config.ChannelVideoStreamId);
            return null;
        }

        IChannelStatus? channelStatus = channelService.GetChannelStatus(config.ChannelVideoStreamId);

        if (channelStatus == null)
        {
            channelStatus = channelService.RegisterChannel(channelVideoStream);
            if (channelStatus == null)
            {
                logger.LogError("Could not register new channel for {ClientId} {ChannelVideoStreamId} {name}", config.ClientId, config.ChannelVideoStreamId, channelVideoStream.User_Tvg_name);
                channelService.UnRegisterChannel(config.ChannelVideoStreamId);
                return null;
            }

            logger.LogInformation("No existing channel for {ClientId} {ChannelVideoStreamId} {name}", config.ClientId, config.ChannelVideoStreamId, channelVideoStream.User_Tvg_name);

            await HandleChannelVideoStreamSwitch(channelStatus).ConfigureAwait(false);

        }
        else
        {
            IStreamHandler? handler = streamManager.GetStreamHandler(channelStatus.CurrentVideoStream.User_Url);
            if (handler is null)
            {
                logger.LogError("Could not find handler for {ClientId} {ChannelVideoStreamId} {name}", config.ClientId, config.ChannelVideoStreamId, channelVideoStream.User_Tvg_name);
                return null;
            }

            await clientStreamerManager.AddClientToHandler(config.ClientId, handler);
            logger.LogInformation("Reuse existing stream handler for {ClientId} {ChannelVideoStreamId} {name}", config.ClientId, config.ChannelVideoStreamId, channelVideoStream.User_Tvg_name);
        }

        return channelStatus;
    }

    private async Task UnRegisterWithChannelManager(IClientStreamerConfiguration config)
    {
        await _unregisterSemaphore.WaitAsync();
        try
        {
            if (!channelService.HasChannel(config.ChannelVideoStreamId))
            {
                logger.LogDebug("UnRegisterWithChannelManager finished early, VideoStreamId not found in channelService");
                return;
            }

            clientStreamerManager.UnRegisterClient(config.ClientId);
            IChannelStatus? channelStatus = channelService.GetChannelStatus(config.ChannelVideoStreamId);
            if (channelStatus == null)
            {
                logger.LogError("UnRegisterWithChannelManager cannot find channelStatus for ClientId {ClientId}", config.ClientId);
                return;
            }
            IStreamHandler? StreamHandler = streamManager.GetStreamHandler(channelStatus.CurrentVideoStream.User_Url);
            if (StreamHandler == null)
            {
                logger.LogError("UnRegisterWithChannelManager cannot find handler for ClientId {ClientId}", config.ClientId);
                return;
            }

            _ = StreamHandler.UnRegisterClientStreamer(config.ClientId);

            if (StreamHandler.ClientCount == 0)
            {
                logger.LogInformation("ChannelManager No more clients, stopping streaming for {videoStreamId} {name}", StreamHandler.VideoStreamId, channelStatus.CurrentVideoStream.User_Tvg_name);
                _ = streamManager.StopAndUnRegisterHandler(channelStatus.CurrentVideoStream.User_Url);

                channelService.UnRegisterChannel(config.ChannelVideoStreamId);
            }

            logger.LogInformation("Finished UnRegisterWithChannelManager with config: {config}", config.ClientId);
        }
        finally
        {
            _ = _unregisterSemaphore.Release();
        }
    }
}