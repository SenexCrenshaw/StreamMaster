using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using StreamMasterApplication.Common.Interfaces;

using StreamMasterDomain.Common;
using StreamMasterDomain.Dto;
using StreamMasterDomain.Extensions;
using StreamMasterDomain.Repository;

using StreamMasterInfrastructure.VideoStreamManager.Buffers;

namespace StreamMasterInfrastructure.VideoStreamManager.Channels;

public class ChannelManager(
    ILogger<ChannelManager> logger,
    ILogger<RingBufferReadStream> ringBufferReadStreamLogger,
    IBroadcastService broadcastService,
    IStreamSwitcher streamSwitcher,
    IChannelService channelService,
    IStreamManager streamManager,
    IClientStreamerManager clientStreamerManager,
    IServiceProvider serviceProvider
    ) : IChannelManager
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

            if (!await streamSwitcher.SwitchToNextVideoStreamAsync(channelStatus, newVideoStreamId))
            {
                logger.LogWarning("Exiting ChangeVideoStreamChannel. Could not change channel to {newVideoStreamId}", newVideoStreamId);
                return;
            }

            IStreamHandler? newStreamHandler = streamManager.GetStreamHandler(newVideoStreamId).DeepCopy();

            if (oldStreamHandler is not null && newStreamHandler is not null)
            {
                foreach (IClientStreamerConfiguration clientStreamerConfiguration in clientStreamerManager.GetClientStreamerConfigurations)
                {
                    oldStreamHandler.UnRegisterClientStreamer(clientStreamerConfiguration);
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
        clientStreamerManager.FailClient(clientId);
    }

    public async Task<Stream?> GetStream(IClientStreamerConfiguration config)
    {
        broadcastService.StartBroadcasting();
        return await RegisterClient(config);
    }

    public async Task RemoveClient(IClientStreamerConfiguration config)
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
        IStreamHandler? _streamInformation = streamManager.GetStreamHandlerFromStreamUrl(streamUrl);

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
                if (channelService.GetChannelStatuses().Count == 0)
                {
                    logger.LogInformation("Exiting ChannelWatcher loop due to no channels");
                    break;
                }
                foreach (IChannelStatus channelStatus in channelService.GetChannelStatuses())
                {
                    var clientCount = clientStreamerManager.ClientCount(channelStatus.VideoStreamId);
                    if (clientCount == 0)
                    {
                        continue;
                    }

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
        var a = clientStreamerManager.GetClientStreamerConfigurationsByChannelVideoStreamId(channelStatus.ParentVideoStreamId);

        if (a.Any())
        {
            foreach (IClientStreamerConfiguration client in clientStreamerManager.GetClientStreamerConfigurations)
            {
                if (client.ClientMasterToken.IsCancellationRequested)
                {
                    logger.LogInformation("Client {clientId} is finished", client.ClientId);
                    await UnRegisterWithChannelManager(client);
                }
            }
        }

        a = clientStreamerManager.GetClientStreamerConfigurationsByChannelVideoStreamId(channelStatus.ParentVideoStreamId);
        if (!a.Any())
        {
            logger.LogInformation("No clients for channel: {videoStreamId}", channelStatus.VideoStreamId);
            logger.LogDebug("Exiting CheckClients for channel: {VideoStreamId}", channelStatus.VideoStreamId);
            streamManager.StopAndUnRegisterHandler(channelStatus.VideoStreamId);

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
            logger.LogDebug("Video Streaming cancellation requested for channelStatus: {channelStatus}, stopping stream and attempting to handle next video stream", channelStatus.ParentVideoStreamId);
            //string oldId = channelStatus.VideoStreamId;
            try
            {
                bool handled = await streamSwitcher.SwitchToNextVideoStreamAsync(channelStatus);
                //_ = streamManager.Stop(oldId);
                logger.LogDebug("ProcessStreamStatus handled: {handled} after stopping streaming and handling next video stream", handled);
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

    private async Task<Stream?> RegisterClient(IClientStreamerConfiguration config)
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

    private async Task<IChannelStatus?> RegisterWithChannelManager(IClientStreamerConfiguration config)
    {
        await _registerSemaphore.WaitAsync();
        try
        {
            IChannelStatus? channelStatus = null;
            IStreamHandler? streamHandler = streamManager.GetStreamHandler(config.ChannelVideoStreamId);

            if (!channelService.HasChannel(config.ChannelVideoStreamId))
            {
                channelStatus = await RegisterNewChannel(config, streamHandler);
                streamHandler = streamManager.GetStreamHandler(channelStatus.VideoStreamId);
            }

            if (channelStatus == null || streamHandler == null)
            {
                logger.LogError("Failed to register with channel manager.");
                channelService.UnregisterChannel(config.ChannelVideoStreamId);
                return null;
            }

            clientStreamerManager.RegisterClient(config);

            var readBuffer = new RingBufferReadStream(() => streamHandler.RingBuffer, ringBufferReadStreamLogger, config);
            clientStreamerManager.SetClientBufferDelegate(config.ClientId, () => streamHandler.RingBuffer);

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

    private async Task<IChannelStatus?> RegisterNewChannel(IClientStreamerConfiguration config, IStreamHandler? streamHandler)
    {
        using IServiceScope scope = serviceProvider.CreateScope();
        IRepositoryWrapper repository = scope.ServiceProvider.GetRequiredService<IRepositoryWrapper>();
        VideoStreamDto? videoStream = await repository.VideoStream.GetVideoStreamById(config.ChannelVideoStreamId);

        if (videoStream == null)
        {
            return null;
        }

        IChannelStatus? channelStatus = channelService.RegisterChannel(config.ChannelVideoStreamId, config.ChannelVideoStreamId);

        if (streamHandler == null)
        {
            logger.LogInformation("No existing stream handler for {ChannelVideoStreamId}, creating", config.ChannelVideoStreamId);
            if (!await streamSwitcher.SwitchToNextVideoStreamAsync(channelStatus).ConfigureAwait(false))
            {
                return null;
            }

            channelStatus = channelService.GetChannelStatus(config.ChannelVideoStreamId);

            streamHandler = streamManager.GetStreamHandler(channelStatus.VideoStreamId);
        }
        else
        {
            logger.LogInformation("Reuse existing stream handler for {ChannelVideoStreamId}", config.ChannelVideoStreamId);
        }

        if (streamHandler == null)
        {
            logger.LogError("RegisterNewChannel: Failed to get streamHandler for ChannelVideoStreamId with id {id}", config.ChannelVideoStreamId);
            return null;
        }

        return channelStatus;
    }

    private IChannelStatus? HandleExistingChannel(IClientStreamerConfiguration config, IStreamHandler? streamHandler)
    {
        IChannelStatus? channelStatus = channelService.GetChannelStatus(config.ChannelVideoStreamId);
        if (streamHandler == null)
        {
            logger.LogError("HandleExistingChannel: Failed to get streamHandler for videostream with id {id}", channelStatus.VideoStreamId);
            return null;
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
                logger.LogDebug("Exiting UnRegisterWithChannelManager due to VideoStreamId not found in _channelStatuses");
                return;
            }

            clientStreamerManager.UnRegisterClient(config.ClientId);

            IStreamHandler? StreamHandler = streamManager.GetStreamHandlerByClientId(config.ClientId);
            if (StreamHandler == null)
            {
                logger.LogError("UnRegisterWithChannelManager cannot find handler for {ClientId}", config.ClientId);
                return;
            }

            _ = StreamHandler.UnRegisterClientStreamer(config);

            if (StreamHandler.ClientCount == 0)
            {
                logger.LogInformation("ChannelManager No more clients, stopping streaming for {videoStreamId}", StreamHandler.VideoStreamId);
                _ = streamManager.StopAndUnRegisterHandler(StreamHandler.VideoStreamId);

                channelService.UnregisterChannel(config.ChannelVideoStreamId);
            }

            logger.LogInformation("Finished UnRegisterWithChannelManager with config: {config}", config.ClientId);
        }
        finally
        {
            _unregisterSemaphore.Release();
        }
    }
}