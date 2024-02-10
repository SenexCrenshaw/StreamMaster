using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using StreamMaster.Domain.Models;
using StreamMaster.Domain.Repository;

namespace StreamMaster.Streams.Channels;

public sealed class ChannelManager : IChannelManager
{
    private readonly SemaphoreSlim _registerSemaphore = new(1, 1);

    private readonly object _disposeLock = new();
    private readonly ILogger<ChannelManager> logger;
    private readonly IStreamSwitcher streamSwitcher;
    private readonly IChannelService channelService;
    private readonly IStreamManager streamManager;
    private readonly IClientStreamerManager clientStreamerManager;
    private readonly IStatisticsManager statisticsManager;
    private readonly IServiceProvider serviceProvider;
    private bool _disposed = false;

    public ChannelManager(
        ILogger<ChannelManager> logger,
        //IBroadcastService broadcastService,
        IStreamSwitcher streamSwitcher,
        IChannelService channelService,
        IStreamManager streamManager,
        IStatisticsManager statisticsManager,
        IClientStreamerManager clientStreamerManager,
        IServiceProvider serviceProvider
    )
    {
        this.logger = logger;
        this.streamSwitcher = streamSwitcher;
        this.channelService = channelService;
        this.streamManager = streamManager;
        this.clientStreamerManager = clientStreamerManager;
        this.serviceProvider = serviceProvider;
        this.statisticsManager = statisticsManager;
        this.streamManager.OnStreamingStoppedEvent += StreamManager_OnStreamingStoppedEvent;
    }

    private async void StreamManager_OnStreamingStoppedEvent(object? sender, IStreamHandler streamHandler)
    {
        if (streamHandler is not null)
        {
            logger.LogInformation("Streaming Stopped Event for VideoStreamId: {VideoStreamId} {VideoStreamName}", streamHandler.VideoStreamId, streamHandler.VideoStreamName);


            List<IChannelStatus> affectedChannelStatuses = channelService.GetChannelStatusesFromVideoStreamId(streamHandler.VideoStreamId);
            foreach (IChannelStatus channelStatus in affectedChannelStatuses)
            {
                if (channelStatus != null)
                {
                    if (channelStatus.FailoverInProgress)
                    {
                        continue;
                    }

                    if (!string.IsNullOrEmpty(channelStatus.OverrideVideoStreamId))
                    {
                        channelStatus.OverrideVideoStreamId = "";
                        continue;
                    }

                    bool didSwitch = await streamSwitcher.SwitchToNextVideoStreamAsync(channelStatus.ChannelVideoStreamId);

                    if (streamHandler.ClientCount == 0 && !didSwitch)
                    {
                        clientStreamerManager.GetClientStreamerConfigurationsByChannelVideoStreamId(channelStatus.ChannelVideoStreamId)
                            .ForEach(async x =>
                            {
                                await UnRegisterWithChannelManager(x).ConfigureAwait(false);
                            }
                            );

                        continue;
                    }
                }
            }
            if (streamHandler.ClientCount == 0)
            {
                streamHandler.Stop();
            }

        }
    }

    public VideoInfo GetVideoInfo(string channelVideoStreamId)
    {
        IChannelStatus? channelStatus = channelService.GetChannelStatus(channelVideoStreamId);
        if (channelStatus is null)
        {
            return new();
        }

        IStreamHandler? streamHandler = streamManager.GetStreamHandler(channelStatus.CurrentVideoStream.User_Url);

        return streamHandler is null ? new() : streamHandler.GetVideoInfo();
    }

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
                streamManager.Dispose();
                channelService.Dispose();
                clientStreamerManager.Dispose();
                _registerSemaphore.Dispose();
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
        _ = clientStreamerManager.CancelClient(clientId, true);
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

        return res;
    }

    public async Task RemoveClient(IClientStreamerConfiguration config)
    {
        logger.LogInformation("Client exited");
        await UnRegisterWithChannelManager(config);
    }

    public async Task SimulateStreamFailure(string streamUrl)
    {
        IStreamHandler? handler = streamManager.GetStreamHandlerFromStreamUrl(streamUrl);

        if (handler is not null)
        {
            handler.Stop();

            logger.LogInformation("Simulating stream failure for: {VideoStreamName}", handler.VideoStreamName);
        }
        else
        {
            logger.LogWarning("Stream not found, cannot simulate stream failure: {StreamUrl}", streamUrl);
        }
    }

    public async Task SimulateStreamFailureForAll()
    {
        foreach (IStreamHandler s in streamManager.GetStreamHandlers())
        {
            s.Stop();
        }
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

        try
        {
            await _registerSemaphore.WaitAsync();

            IChannelStatus? channelStatus = await EnsureChannelRegistration(config);

            if (channelStatus == null)
            {
                logger.LogError("Failed to register with channel manager. channelStatus is null");
                return null;
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

            await streamSwitcher.SwitchToNextVideoStreamAsync(channelStatus.ChannelVideoStreamId);

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
        try
        {
            await _registerSemaphore.WaitAsync();

            logger.LogInformation("UnRegisterWithChannelManager client: {clientId}  {name}", config.ClientId, config.ChannelName);

            await clientStreamerManager.UnRegisterClient(config.ClientId);

            statisticsManager.UnRegisterClient(config.ClientId);

            if (!channelService.HasChannel(config.ChannelVideoStreamId))
            {
                logger.LogDebug("UnRegisterWithChannelManager finished early, VideoStreamId not found in channelService, {clientId}  {name}", config.ClientId, config.ChannelName);
                //    return;
            }

            IChannelStatus? channelStatus = channelService.GetChannelStatus(config.ChannelVideoStreamId);
            if (channelStatus != null)
            {

                IStreamHandler? StreamHandler = streamManager.GetStreamHandler(channelStatus.CurrentVideoStream.User_Url);
                if (StreamHandler == null)
                {
                    logger.LogError("UnRegisterWithChannelManager cannot find handler for {clientId}  {name}", config.ClientId, config.ChannelName);

                }
                else
                {
                    _ = StreamHandler.UnRegisterClientStreamer(config.ClientId);

                    if (StreamHandler.ClientCount == 0)
                    {

                        logger.LogInformation("ChannelManager No more clients, stopping streaming for {clientId}  {name}", config.ClientId, config.ChannelName);
                        await streamManager.StopAndUnRegisterHandler(channelStatus.CurrentVideoStream.User_Url);

                        channelService.UnRegisterChannel(config.ChannelVideoStreamId);
                    }
                }
            }
            logger.LogInformation("Finished UnRegisterWithChannelManager with client: {clientId}  {name}", config.ClientId, config.ChannelName);
        }
        finally
        {
            _ = _registerSemaphore.Release();
        }
    }
}