namespace StreamMaster.Streams.Channels;

public sealed class ChannelManager : IChannelManager
{
    private readonly SemaphoreSlim _registerSemaphore = new(1, 1);
    private readonly object _disposeLock = new();
    private readonly ILogger<ChannelManager> logger;
    private readonly IChannelService channelService;
    private readonly IStreamManager streamManager;
    private readonly IClientStreamerManager clientStreamerManager;
    private bool _disposed = false;
    public ChannelManager(
    ILogger<ChannelManager> logger,
    IChannelService channelService,
    IStreamManager streamManager,
    IClientStreamerManager clientStreamerManager
    )
    {
        this.logger = logger;
        this.channelService = channelService;
        this.streamManager = streamManager;
        this.clientStreamerManager = clientStreamerManager;
        this.streamManager.OnStreamingStoppedEvent += StreamManager_OnStreamingStoppedEvent;
    }


    public async Task<Stream?> GetChannelAsync(IClientStreamerConfiguration config, CancellationToken cancellationToken = default)
    {
        try
        {
            await _registerSemaphore.WaitAsync();
            if (cancellationToken.IsCancellationRequested)
            {

                logger.LogInformation("Exiting GetChannel due to ClientMasterToken being cancelled");
                return null;
            }

            var channelStatus = await channelService.RegisterChannel(config);

            if (channelStatus == null)
            {
                await UnRegisterWithChannelManager(config);
                logger.LogInformation("Exiting GetChannel: status null");
                return null;

            }


            return config.ClientStream as Stream;
        }
        finally
        {
            _ = _registerSemaphore.Release();
        }
    }

    private async void StreamManager_OnStreamingStoppedEvent(object? sender, IStreamHandler streamHandler)
    {
        //if (streamHandler is not null)
        //{
        //    logger.LogInformation("Streaming Stopped Event for StreamId: {StreamId} {StreamName}", streamHandler.SMStream.Id, streamHandler.StreamName);


        //    List<IChannelStatus> affectedChannelStatuses = channelService.GetChannelStatusesFromSMStreamId(streamHandler.SMStream.Id);
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

        //            bool didSwitch = await streamSwitcher.SwitchChannelToNextStream(channelStatus);

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

        //}
    }

    public VideoInfo GetVideoInfo(int SMChannelId)
    {
        IChannelStatus? channelStatus = channelService.GetChannelStatus(SMChannelId);
        if (channelStatus is null)
        {
            return new();
        }

        IStreamHandler? streamHandler = streamManager.GetStreamHandler(channelStatus.SMStream.Url);

        return streamHandler is null ? new() : streamHandler.GetVideoInfo();
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


    public async Task RemoveClientAsync(IClientStreamerConfiguration config)
    {
        logger.LogInformation("Client exited");
        await UnRegisterWithChannelManager(config);
    }

    private async Task UnRegisterWithChannelManager(IClientStreamerConfiguration config)
    {
        try
        {
            await _registerSemaphore.WaitAsync();

            logger.LogInformation("UnRegisterWithChannelManager client: {clientId}  {name}", config.ClientId, config.SMChannel.Name);

            await clientStreamerManager.UnRegisterClient(config.ClientId);

            if (!channelService.HasChannel(config.SMChannel.Id))
            {
                logger.LogDebug("UnRegisterWithChannelManager finished early, VideoStreamId not found in channelService, {clientId}  {name}", config.ClientId, config.SMChannel.Name);
                //    return;
            }

            IChannelStatus? channelStatus = channelService.GetChannelStatus(config.SMChannel.Id);
            if (channelStatus != null)
            {

                IStreamHandler? StreamHandler = streamManager.GetStreamHandler(channelStatus.SMStream.Url);
                if (StreamHandler == null)
                {
                    logger.LogError("UnRegisterWithChannelManager cannot find handler for {clientId}  {name}", config.ClientId, config.SMChannel.Name);

                }
                else
                {
                    _ = StreamHandler.UnRegisterClientStreamer(config.ClientId);

                    if (StreamHandler.ClientCount == 0)
                    {

                        logger.LogInformation("ChannelManager No more clients, stopping streaming for {clientId}  {name}", config.ClientId, config.SMChannel.Name);
                        await streamManager.StopAndUnRegisterHandler(channelStatus.SMStream.Url);

                        channelService.UnRegisterChannel(config.SMChannel.Id);
                    }
                }
            }
            logger.LogInformation("Finished UnRegisterWithChannelManager with client: {clientId}  {name}", config.ClientId, config.SMChannel.Name);
        }
        finally
        {
            _ = _registerSemaphore.Release();
        }
    }
    public void SimulateStreamFailure(string streamUrl)
    {
        IStreamHandler? handler = streamManager.GetStreamHandlerFromStreamUrl(streamUrl);

        if (handler is not null)
        {
            handler.Stop();

            logger.LogInformation("Simulating stream failure for: {VideoStreamName}", handler.SMStream.Name);
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
            s.Stop();
        }
    }




}