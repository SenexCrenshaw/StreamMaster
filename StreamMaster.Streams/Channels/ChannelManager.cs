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
            logger.LogInformation("Streaming Stopped Event for VideoStreamId: {VideoStreamId} {VideoStreamName}", streamHandler.SMStream.Id, streamHandler.VideoStreamName);


            //  List<IChannelStatus> affectedChannelStatuses = channelService.GetChannelStatusesFromId(streamHandler.smch);
            //    foreach (IChannelStatus channelStatus in affectedChannelStatuses)
            //    {
            //        if (channelStatus != null)
            //        {
            //            if (channelStatus.FailoverInProgress)
            //            {
            //                continue;
            //            }

            //            if (!string.IsNullOrEmpty(channelStatus.OverrideVideoStreamId))
            //            {
            //                channelStatus.OverrideVideoStreamId = "";
            //                continue;
            //            }

            //            bool didSwitch = await streamSwitcher.SwitchToNextVideoStreamAsync(channelStatus.CurrentSMStream);

            //            if (streamHandler.ClientCount == 0 && !didSwitch)
            //            {
            //                clientStreamerManager.GetClientStreamerConfigurationsBySMChannelId(channelStatus.Id)
            //                    .ForEach(async x =>
            //                    {
            //                        await x.CancelClient();
            //                        await UnRegisterWithChannelManager(x).ConfigureAwait(false);
            //                    }
            //                    );

            //                continue;
            //            }
            //        }
            //    }
            //    if (streamHandler.ClientCount == 0)
            //    {
            //        streamHandler.Stop();
            //    }

        }
    }

    public VideoInfo GetVideoInfo(int SMChannelId)
    {
        IChannelStatus? channelStatus = channelService.GetChannelStatus(SMChannelId);
        if (channelStatus is null)
        {
            return new();
        }

        IStreamHandler? streamHandler = streamManager.GetStreamHandler(channelStatus.CurrentSMStream.Url);

        return streamHandler is null ? new() : streamHandler.GetVideoInfo();
    }

    public async Task ChangeVideoStreamChannel(string playingSMStreamId, string newSMStreamId)
    {
        logger.LogDebug("Starting ChangeVideoStreamChannel with playingSMStreamId: {playingSMStreamId} and newSMStreamId: {newSMStreamId}", playingSMStreamId, newSMStreamId);

        //foreach (IChannelStatus channelStatus in channelService.GetChannelStatusesFromId(playingSMStreamId))
        //{
        //    if (channelStatus != null)
        //    {

        //        if (!await streamSwitcher.SwitchToNextVideoStreamAsync(channelStatus.CurrentSMStream, newSMStreamId))
        //        {
        //            logger.LogWarning("Exiting ChangeVideoStreamChannel. Could not change channel to {newSMStreamId}", newSMStreamId);
        //            return;
        //        }
        //        return;
        //    }
        //}
        logger.LogWarning("Channel not found: {videoStreamId}", playingSMStreamId);
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
        if (channelStatus is null || config.Stream is null)
        {
            channelService.UnRegisterChannel(config.SMChannel.Id);
            logger.LogInformation("Exiting Register Client with null due to channelStatus or Read Buffer being null");
            return null;
        }

        logger.LogInformation("Finished Register Client");
        return (Stream)config.Stream;
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
        //using IServiceScope scope = serviceProvider.CreateScope();
        //IRepositoryWrapper repository = scope.ServiceProvider.GetRequiredService<IRepositoryWrapper>();
        //SMChannel? smChannel = repository.SMChannel.GetSMChannel(config.SMChannel.Id);

        if (config.SMChannel == null)
        {
            logger.LogError("EnsureChannelRegistration SMChannel is null");
            return null;
        }

        IChannelStatus? channelStatus = channelService.GetChannelStatus(config.SMChannel.Id);

        if (channelStatus == null)
        {
            channelStatus = await channelService.RegisterChannel(config.SMChannel);
            if (channelStatus == null)
            {
                logger.LogError("Could not register new channel for {ClientId} {ChannelVideoStreamId} {name}", config.ClientId, config.SMChannel.Id, config.SMChannel.Name);
                channelService.UnRegisterChannel(config.SMChannel.Id);
                return null;
            }

            logger.LogInformation("No existing channel for {ClientId} {ChannelVideoStreamId} {name}", config.ClientId, config.SMChannel.Id, config.SMChannel.Name);

            await streamSwitcher.SwitchToNextVideoStreamAsync(config.SMChannel, channelStatus);

        }
        else
        {
            IStreamHandler? handler = streamManager.GetStreamHandler(channelStatus.CurrentSMStream.Url);
            if (handler is null)
            {
                logger.LogError("Could not find handler for {ClientId} {ChannelVideoStreamId} {name}", config.ClientId, config.SMChannel.Id, config.SMChannel.Name);
                return null;
            }
            if (handler.IsFailed)
            {
                logger.LogInformation("Existing hanlder is failed, creating");

                await streamSwitcher.SwitchToNextVideoStreamAsync(config.SMChannel, channelStatus);
            }

            await clientStreamerManager.AddClientToHandler(config.ClientId, handler);
            logger.LogInformation("Reuse existing stream handler for {ClientId} {ChannelVideoStreamId} {name}", config.ClientId, config.SMChannel.Id, config.SMChannel.Name);
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

            if (!channelService.HasChannel(config.SMChannel.Id))
            {
                logger.LogDebug("UnRegisterWithChannelManager finished early, VideoStreamId not found in channelService, {clientId}  {name}", config.ClientId, config.ChannelName);
                //    return;
            }

            IChannelStatus? channelStatus = channelService.GetChannelStatus(config.SMChannel.Id);
            if (channelStatus != null)
            {

                IStreamHandler? StreamHandler = streamManager.GetStreamHandler(channelStatus.CurrentSMStream.Url);
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
                        await streamManager.StopAndUnRegisterHandler(channelStatus.CurrentSMStream.Url);

                        channelService.UnRegisterChannel(config.SMChannel.Id);
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