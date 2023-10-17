using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using StreamMasterApplication.Common.Interfaces;
using StreamMasterApplication.Common.Models;

using StreamMasterDomain.Common;
using StreamMasterDomain.Dto;
using StreamMasterDomain.Extensions;
using StreamMasterDomain.Repository;

using StreamMasterInfrastructure.VideoStreamManager.Buffers;

namespace StreamMasterInfrastructure.VideoStreamManager;

public class ChannelManager(ILogger<ChannelManager> logger, IBroadcastService broadcastService, IStreamSwitcher streamSwitcher, IChannelService channelService, IStreamManager streamManager, IServiceProvider serviceProvider) : IChannelManager
{
    private readonly SemaphoreSlim _registerSemaphore = new(1, 1);
    private readonly SemaphoreSlim _unregisterSemaphore = new(1, 1);

    private static readonly CancellationTokenSource ChannelWatcherToken = new();
    private static bool ChannelWatcherStarted = false;
    public async Task ChangeVideoStreamChannel(string playingVideoStreamId, string newVideoStreamId)
    {
        logger.LogDebug("Starting ChangeVideoStreamChannel with playingVideoStreamId: {playingVideoStreamId} and newVideoStreamId: {newVideoStreamId}", playingVideoStreamId, newVideoStreamId);

        IChannelStatus? channelStatus = channelService.GetChannelStatus(playingVideoStreamId);

        if (channelStatus != null)
        {
            logger.LogDebug("Channel status found for playingVideoStreamId: {playingVideoStreamId}", playingVideoStreamId);

            IStreamHandler? oldStreamHandler = streamManager.GetStreamHandler(newVideoStreamId).DeepCopy();

            //IStreamHandler? oldInfo = channelStatus.StreamHandler;

            if (!await streamSwitcher.SwitchToNextVideoStreamAsync(channelStatus, newVideoStreamId))
            {
                logger.LogWarning("Exiting ChangeVideoStreamChannel. Could not change channel to {newVideoStreamId}", newVideoStreamId);
                return;
            }

            IStreamHandler? newStreamHandler = streamManager.GetStreamHandler(newVideoStreamId).DeepCopy();

            if (oldStreamHandler is not null && newStreamHandler is not null)
            {
                foreach (ClientStreamerConfiguration clientStreamerConfiguration in channelStatus.GetChannelClientClientStreamerConfigurations)
                {
                    _ = oldStreamHandler.UnRegisterClientStreamer(clientStreamerConfiguration);
                    logger.LogDebug("Unregistered stream configuration for client: {ClientId}", clientStreamerConfiguration.ClientId);
                }
            }
            logger.LogWarning("Channel changed for {videoStreamId} switching video stream id to {newVideoStreamId}", playingVideoStreamId, newVideoStreamId);
            logger.LogDebug("Exiting ChangeVideoStreamChannel after channel change");

            return;
        }
        logger.LogWarning("Channel not found: {videoStreamId}", playingVideoStreamId);
        logger.LogDebug("Exiting ChangeVideoStreamChannel due to channel not found");

        return;
    }
    public void Dispose()
    {
        broadcastService.StopBroadcasting();
        GC.SuppressFinalize(this);
    }
    public void FailClient(Guid clientId)
    {
        //logger.LogDebug("Starting FailClient with clientId: {clientId}", clientId);

        foreach (IStreamHandler streamHandler in streamManager.GetStreamHandlers())
        {
            ClientStreamerConfiguration? c = streamHandler.GetClientStreamerConfiguration(clientId);

            if (c != null)
            {
                c.ClientMasterToken.Cancel();
                logger.LogWarning("Failing client: {clientId}", clientId);
                logger.LogDebug("Cancelled CancellationTokenSource for client: {clientId}", clientId);
                logger.LogDebug("Exiting FailClient after failing client: {clientId}", clientId);
                break;
            }
        }
        logger.LogDebug("Exiting FailClient, no matching client found for clientId: {clientId}", clientId);
    }

    public async Task<Stream?> GetStream(ClientStreamerConfiguration config)
    {
        broadcastService.StartBroadcasting();
        return await RegisterClient(config);
    }

    public async Task RemoveClient(ClientStreamerConfiguration config)
    {
        await UnRegisterWithChannelManager(config);
    }

    public bool ShouldHandleFailover(ChannelStatus channelStatus)
    {
        IStreamHandler? streamHandler = streamManager.GetStreamHandler(channelStatus.VideoStreamId);

        if (streamHandler is null)
        {
            return false;
        }

        return streamHandler.ClientCount == 0 || streamHandler.VideoStreamingCancellationToken.IsCancellationRequested;
    }

    public void SimulateStreamFailure(string streamUrl)
    {
        IStreamHandler? _streamInformation = streamManager.GetStreamInformationFromStreamUrl(streamUrl);

        if (_streamInformation is not null)
        {
            if (_streamInformation.VideoStreamingCancellationToken?.IsCancellationRequested == false)
            {
                _streamInformation.VideoStreamingCancellationToken.Cancel();
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
        foreach (IStreamHandler s in streamManager.GetStreamInformations())
        {
            s.VideoStreamingCancellationToken.Cancel();
        }
    }

    //private async Task ChannelWatcher(IChannelStatus channelStatus)
    private async Task ChannelWatcher()
    {
        ChannelWatcherStarted = true;
        logger.LogInformation("Channel watcher starting");
        try
        {
            while (!ChannelWatcherToken.IsCancellationRequested)
            {
                if (channelService.GetChannelStatuses().Count == 0)
                {
                    logger.LogInformation("Exiting ChannelWatcher loop due to no channels");
                    break;
                }
                foreach (IChannelStatus channelStatus in channelService.GetChannelStatuses())
                {
                    bool hasClients = await CheckClients(channelStatus);
                    if (!hasClients)
                    {
                        continue;
                    }

                    bool quit = await ProcessStreamStatus(channelStatus);
                    if (quit)
                    {
                        logger.LogDebug("Exiting ChannelWatcher loop due to quit condition for channel: {VideoStreamId}", channelStatus.VideoStreamId);
                    }
                }
                await ChannelWatcherToken.Token.ApplyDelay(50);
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

    private async Task<bool> CheckClients(IChannelStatus channelStatus)
    {
        if (channelStatus.GetChannelClientClientStreamerConfigurations.Any())
        {
            foreach (ClientStreamerConfiguration client in channelStatus.GetChannelClientClientStreamerConfigurations)
            {
                if (client.ClientMasterToken.IsCancellationRequested)
                {
                    logger.LogInformation("Client {clientId} is finished", client.ClientId);
                    await UnRegisterWithChannelManager(client);
                }
            }
        }

        if (!channelStatus.GetChannelClientClientStreamerConfigurations.Any())
        {
            logger.LogInformation("No clients for channel: {videoStreamId}", channelStatus.VideoStreamId);
            logger.LogDebug("Exiting CheckClients for channel: {VideoStreamId}", channelStatus.VideoStreamId);
            streamManager.Stop(channelStatus.VideoStreamId);
            channelService.UnregisterChannel(channelStatus.VideoStreamId);
            return false;
        }
        return true;
    }

    private async Task<bool> ProcessStreamStatus(IChannelStatus channelStatus)
    {
        IStreamHandler? streamHandler = streamManager.GetStreamHandler(channelStatus.VideoStreamId);
        if (streamHandler is null)
        {
            logger.LogDebug("ChannelStatus StreamInformation is null for channelStatus: {channelStatus}, attempting to switch to next video stream", channelStatus);
            bool handled;
            try
            {
                handled = await streamSwitcher.SwitchToNextVideoStreamAsync(channelStatus);
            }
            catch (TaskCanceledException)
            {
                logger.LogInformation("Task was cancelled");
                throw;
            }

            logger.LogDebug("Exiting ProcessStreamStatus with {handled} after switching to next video stream", !handled);
            return !handled;
        }

        if (streamHandler.VideoStreamingCancellationToken.IsCancellationRequested)
        {
            logger.LogDebug("VideoStreamingCancellationToken cancellation requested for channelStatus: {channelStatus}, stopping stream and attempting to handle next video stream", channelStatus);
            _ = streamManager.Stop(channelStatus.VideoStreamId);
            try
            {
                bool handled = await streamSwitcher.SwitchToNextVideoStreamAsync(channelStatus);
                logger.LogDebug("Exiting ProcessStreamStatus with {handled} after stopping streaming and handling next video stream", !handled);
                return !handled;
            }
            catch (TaskCanceledException)
            {
                logger.LogInformation("Task was cancelled");
                throw;
            }
        }

        return false;
    }

    private async Task<Stream?> RegisterClient(ClientStreamerConfiguration config)
    {
        IChannelStatus? channelStatus = await RegisterWithChannelManager(config);
        if (channelStatus is null || config.ReadBuffer is null)
        {
            logger.LogInformation("Exiting Register Client with null due to channelStatus or Read Buffer being null");
            return null;
        }

        logger.LogInformation("Finished Register Client");
        return (Stream)config.ReadBuffer;
    }

    private async Task<IChannelStatus?> RegisterWithChannelManager(ClientStreamerConfiguration config)
    {
        await _registerSemaphore.WaitAsync();
        try
        {
            IChannelStatus? channelStatus;
            IStreamHandler? streamHandler = streamManager.GetStreamHandler(config.VideoStreamId);

            if (!channelService.HasChannel(config.VideoStreamId))
            {
                channelStatus = await RegisterNewChannel(config, streamHandler);
                streamHandler = streamManager.GetStreamHandler(config.VideoStreamId);
            }
            else
            {
                channelStatus = HandleExistingChannel(config, streamHandler);
                logger.LogInformation("Reuse existing stream handler for {VideoStreamId}", config.VideoStreamId);
            }

            if (channelStatus == null || streamHandler == null)
            {
                logger.LogError("Failed to register with channel manager.");
                channelService.UnregisterChannel(config.VideoStreamId);
                return null;
            }

            channelStatus.RegisterClient(config);
            config.ReadBuffer = new RingBufferReadStream(() => streamHandler.RingBuffer, config);
            streamHandler.RegisterClientStreamer(config);

            if (!ChannelWatcherStarted)
            {
                _ = ChannelWatcher().ConfigureAwait(false);
            }

            logger.LogDebug("Finished RegisterWithChannelManager with config: {config}", config.ClientId);
            return channelStatus;
        }
        finally
        {
            _registerSemaphore.Release();
        }
    }

    private async Task<IChannelStatus?> RegisterNewChannel(ClientStreamerConfiguration config, IStreamHandler? streamHandler)
    {
        using IServiceScope scope = serviceProvider.CreateScope();
        IRepositoryWrapper repository = scope.ServiceProvider.GetRequiredService<IRepositoryWrapper>();
        VideoStreamDto? videoStream = await repository.VideoStream.GetVideoStreamById(config.VideoStreamId);

        if (videoStream == null)
        {
            return null;
        }

        config.VideoStreamName = videoStream.User_Tvg_name;
        IChannelStatus channelStatus = channelService.RegisterChannel(config.VideoStreamId, config.VideoStreamName);

        if (streamHandler == null)
        {
            logger.LogInformation("No existing stream handler for {VideoStreamId}, creating", config.VideoStreamId);
            if (!await streamSwitcher.SwitchToNextVideoStreamAsync(channelStatus).ConfigureAwait(false))
            {
                return null;
            }
            streamHandler = streamManager.GetStreamHandler(config.VideoStreamId);
        }
        else
        {
            logger.LogInformation("Reuse existing stream handler for {VideoStreamId}", config.VideoStreamId);
        }

        if (streamHandler == null)
        {
            logger.LogError("Failed to get streamHandler for videostream with id {id}", config.VideoStreamId);
            return null;
        }

        return channelStatus;
    }

    private IChannelStatus? HandleExistingChannel(ClientStreamerConfiguration config, IStreamHandler? streamHandler)
    {
        IChannelStatus? channelStatus = channelService.GetChannelStatus(config.VideoStreamId);
        if (streamHandler == null)
        {
            logger.LogError("Failed to get streamHandler for videostream with id {id}", config.VideoStreamId);
            return null;
        }
        return channelStatus;
    }

    //private void CompleteClientRegistration(IChannelStatus channelStatus, IStreamHandler streamHandler, ClientStreamerConfiguration config)
    //{
    //    channelStatus.RegisterClient(config);
    //    config.ReadBuffer = new RingBufferReadStream(() => streamHandler.RingBuffer, config);
    //    streamHandler.RegisterClientStreamer(config);
    //}

    private async Task UnRegisterWithChannelManager(ClientStreamerConfiguration config)
    {
        await _unregisterSemaphore.WaitAsync();
        try
        {
            if (!channelService.HasChannel(config.VideoStreamId))
            {
                logger.LogDebug("Exiting UnRegisterWithChannelManager due to VideoStreamId not found in _channelStatuses");
                return;
            }
            IChannelStatus? channelStatus = channelService.GetChannelStatus(config.VideoStreamId);

            channelStatus?.UnRegisterClient(config.ClientId);

            IStreamHandler? StreamHandler = streamManager.GetStreamHandler(config.VideoStreamId);

            if (StreamHandler?.HasClient(config.ClientId) == true)
            {
                _ = StreamHandler.UnRegisterClientStreamer(config);

                if (StreamHandler.ClientCount == 0)
                {
                    logger.LogInformation("ChannelManager No more clients, stopping streaming for {videoStreamId}", config.VideoStreamId);
                    _ = streamManager.Stop(config.VideoStreamId);
                }
            }

            channelStatus = channelService.GetChannelStatus(config.VideoStreamId);
            if (channelStatus?.ClientCount == 0)
            {
                channelService.UnregisterChannel(config.VideoStreamId);
            }

            logger.LogInformation("Finished UnRegisterWithChannelManager with config: {config}", config.ClientId);
        }
        finally
        {
            _unregisterSemaphore.Release();
        }
    }
}