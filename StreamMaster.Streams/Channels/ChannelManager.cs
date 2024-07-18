namespace StreamMaster.Streams.Channels;

public sealed class ChannelManager : IChannelManager
{
    private readonly SemaphoreSlim _registerSemaphore = new(1, 1);
    private readonly object _disposeLock = new();
    private readonly ILogger<ChannelManager> logger;
    private readonly IChannelService channelService;
    private readonly IStreamManager streamManager;
    private readonly IClientStreamerManager clientStreamerManager;
    private readonly IBroadcastService broadcastService;
    private readonly IServiceProvider serviceProvider;
    private readonly IMessageService messageService;
    private bool _disposed = false;
    public ChannelManager(
    ILogger<ChannelManager> logger,
    IChannelService channelService,
    IStreamManager streamManager,
    IClientStreamerManager clientStreamerManager,
    IBroadcastService broadcastService,
    IServiceProvider serviceProvider,
    IMessageService messageService
    )
    {
        this.logger = logger;
        this.messageService = messageService;
        this.serviceProvider = serviceProvider;
        this.channelService = channelService;
        this.broadcastService = broadcastService;
        this.streamManager = streamManager;
        this.clientStreamerManager = clientStreamerManager;
        this.streamManager.OnStreamingStoppedEvent += StreamManager_OnStreamingStoppedEvent;
        //broadcastService.StartBroadcasting();
    }
    private async void StreamManager_OnStreamingStoppedEvent(object? sender, StreamHandlerStopped StoppedEvent)
    {
        if (sender is not null and IStreamHandler streamHandler)
        {
            streamHandler.Stop();

            logger.LogInformation("Streaming Stopped Event for StreamId: {StreamId} {ChannelName} {StreamName}", streamHandler.SMStream.Id, streamHandler.SMChannel.Name, streamHandler.SMChannel.Name);

            List<IChannelStatus> affectedChannelStatuses = channelService.GetChannelStatusesFromSMStreamId(streamHandler.SMStream.Id);
            //List<IChannelStatus> affectedChannelStatuses = channelService.GetChannelStatusesFromSMStreamId(streamHandler.SMStream.Id);
            foreach (IChannelStatus channelStatus in affectedChannelStatuses)
            {
                if (channelStatus != null && channelStatus.Shutdown != true)
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

                    bool didSwitch = await channelService.SwitchChannelToNextStream(channelStatus);

                    if (!didSwitch)
                    {
                        clientStreamerManager.GetClientStreamerConfigurationsBySMChannelId(channelStatus.SMChannel.Id)
                            .ForEach(async x =>
                            {
                                await CancelClient(x.ClientId);
                            }
                            );

                        continue;
                    }
                }
            }

            if (streamHandler.ClientCount == 0)
            {
                streamManager.StopAndUnRegisterHandler(streamHandler.SMStream.Url);

            }

        }
    }

    public async Task<Stream?> GetChannelAsync(ClientStreamerConfiguration config, CancellationToken cancellationToken = default)
    {
        try
        {
            await _registerSemaphore.WaitAsync();
            if (cancellationToken.IsCancellationRequested)
            {

                logger.LogInformation("Exiting GetChannel due to ClientMasterToken being cancelled");
                return null;
            }


            IChannelStatus? channelStatus = await channelService.RegisterChannel(config);

            if (channelStatus == null)
            {
                await UnRegisterWithChannelManager(config);
                logger.LogInformation("Exiting GetChannel: status null");
                return null;

            }

            await messageService.SendInfo($"Client started streaming {config.SMChannel.Name}", "Client Start");
            return config.ClientStream as Stream;
        }
        finally
        {
            _ = _registerSemaphore.Release();
        }
    }




    public VideoInfo GetVideoInfo(int SMChannelId)
    {
        IChannelStatus? channelStatus = channelService.GetChannelStatus(SMChannelId);
        if (channelStatus is null)
        {
            return new();
        }

        return streamManager.GetVideoInfo(channelStatus.SMStream.Url);// streamHandler.GetVideoInfo();
    }

    public async Task ChangeVideoStreamChannelAsync(string playingSMStreamId, string newSMStreamId, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Starting ChangeVideoStreamChannel with playingSMStreamId: {playingSMStreamId} and newSMStreamId: {newSMStreamId}", playingSMStreamId, newSMStreamId);

        foreach (IChannelStatus channelStatus in channelService.GetChannelStatusesFromSMStreamId(playingSMStreamId))
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }
            if (channelStatus != null)
            {

                if (!await channelService.SwitchChannelToNextStream(channelStatus, newSMStreamId))
                {
                    logger.LogWarning("Exiting ChangeVideoStreamChannel. Could not change channel to {newSMStreamId}", newSMStreamId);
                    return;
                }
                return;
            }
        }

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
    public async Task CancelClient(Guid clientId)
    {
        _ = clientStreamerManager.CancelClient(clientId, true);

        ClientStreamerConfiguration? config = await clientStreamerManager.GetClientStreamerConfiguration(clientId);
        if (config == null)
        {
            return;
        }

        await UnRegisterWithChannelManager(config).ConfigureAwait(false);
    }


    public async Task RemoveClientAsync(ClientStreamerConfiguration config)
    {
        logger.LogInformation("Client exited");
        await UnRegisterWithChannelManager(config);
    }

    private async Task UnRegisterWithChannelManager(ClientStreamerConfiguration config)
    {
        try
        {
            //await _registerSemaphore.WaitAsync();

            if (clientStreamerManager.HasClient(config.ClientId))
            {
                await messageService.SendInfo($"Client Exited from {config.SMChannel.Name}", "Client Exited");
                await clientStreamerManager.UnRegisterClient(config.ClientId);
            }

            logger.LogInformation("UnRegisterWithChannelManager client: {clientId}  {name}", config.ClientId, config.SMChannel.Name);


            if (!channelService.HasChannel(config.SMChannel.Id))
            {
                logger.LogError("UnRegisterWithChannelManager cannot find channel for {SMChannelId} {name}", config.SMChannel.Id, config.SMChannel.Name);
                return;
            }

            IChannelStatus? channelStatus = channelService.GetChannelStatus(config.SMChannel.Id);
            if (channelStatus != null)
            {

                IStreamHandler? StreamHandler = streamManager.GetStreamHandler(channelStatus.SMStream.Url);
                if (StreamHandler == null)
                {
                    logger.LogError("UnRegisterWithChannelManager cannot find handler for {clientId}  {name}", config.ClientId, config.SMChannel.Name);
                    return;
                }

                if (streamManager.UnRegisterClientStreamer(channelStatus.SMStream.Url, config.ClientId, config.SMChannel.Name) == 0)
                {
                    logger.LogInformation("UnRegisterWithChannelManager No more clients, unregister channel {name}", config.SMChannel.Name);

                    channelService.UnRegisterChannel(config.SMChannel.Id);
                }

            }

            logger.LogInformation("UnRegisterWithChannelManager Finished with client: {clientId}  {name}", config.ClientId, config.SMChannel.Name);
        }
        finally
        {
            //_ = _registerSemaphore.Release();
        }
    }
    public async Task CancelChannel(int SMChannelId)
    {

        IChannelStatus? stat = channelService.GetChannelStatus(SMChannelId);
        if (stat is null)
        {
            logger.LogWarning("Channel not found: {SMChannelId}", SMChannelId);
            return;
        }

        stat.Shutdown = true;

        IStreamHandler? handler = streamManager.GetStreamHandlerFromStreamUrl(stat.SMStream.Url);

        if (handler is not null)
        {

            handler.Stop();

            foreach (ClientStreamerConfiguration config in handler.GetClientStreamerClientIdConfigs)
            {
                await CancelClient(config.ClientId);
            }
            channelService.UnRegisterChannel(SMChannelId);

            logger.LogInformation("Simulating stream failure for: {VideoStreamName}", handler.SMStream.Name);
        }
        else
        {
            logger.LogWarning("Stream not found, cannot simulate stream failure: {StreamUrl}", SMChannelId);
        }
    }

    public void MoveToNextStream(int SMChannelId)
    {

        IChannelStatus? stat = channelService.GetChannelStatus(SMChannelId);
        if (stat is null)
        {
            logger.LogWarning("Channel not found: {SMChannelId}", SMChannelId);
            return;
        }


        IStreamHandler? handler = streamManager.GetStreamHandlerFromStreamUrl(stat.SMStream.Url);

        if (handler is not null)
        {
            handler.CancelStreamThread();

            logger.LogInformation("Simulating stream failure for: {VideoStreamName}", handler.SMStream.Name);
        }
        else
        {
            logger.LogWarning("Stream not found, cannot simulate stream failure: {StreamUrl}", SMChannelId);
        }
    }

    public void CancelAllChannels()
    {
        foreach (IStreamHandler s in streamManager.GetStreamHandlers())
        {
            s.Stop();
        }
    }

}