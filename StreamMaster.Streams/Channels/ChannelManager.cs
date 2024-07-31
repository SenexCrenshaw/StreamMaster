namespace StreamMaster.Streams.Channels;

public sealed class ChannelManager : IChannelManager
{
    private readonly SemaphoreSlim _registerSemaphore = new(1, 1);
    private readonly object _disposeLock = new();
    private readonly ILogger<ChannelManager> logger;
    private readonly IChannelService channelService;
    private readonly IChannelDistributorService channelDistributorService;

    private readonly IMessageService messageService;
    private bool _disposed = false;
    public ChannelManager(
    ILogger<ChannelManager> logger,
    IChannelDistributorService channelDistributorService,
    IChannelService channelService,
    IMessageService messageService
    )
    {
        this.logger = logger;
        this.messageService = messageService;
        this.channelService = channelService;
        this.channelDistributorService = channelDistributorService;
    }

    public async Task<Stream?> GetChannelStreamAsync(IClientConfiguration config, CancellationToken cancellationToken = default)
    {
        try
        {
            await _registerSemaphore.WaitAsync(cancellationToken);
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
                channelService.Dispose();
                //clientStreamerManager.Dispose();

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
    public async Task CancelClient(string UniqueRequestId)
    {
        //_ = clientStreamerManager.CancelClient(UniqueRequestId, true);

        IClientConfiguration? config = await channelService.GetClientStreamerConfiguration(UniqueRequestId);
        if (config == null)
        {
            return;
        }

        await UnRegisterWithChannelManager(config).ConfigureAwait(false);
    }

    public async Task RemoveClientAsync(IClientConfiguration config)
    {
        logger.LogInformation("Client exited");
        await UnRegisterWithChannelManager(config);
    }

    private async Task UnRegisterWithChannelManager(IClientConfiguration config)
    {
        logger.LogInformation("UnRegisterWithChannelManager client: {UniqueRequestId} {name}", config.UniqueRequestId, config.SMChannel.Name);

        if (!await channelService.UnRegisterClient(config.UniqueRequestId))
        {
            logger.LogWarning("UnRegisterWithChannelManager channelService doesnt not have client: {UniqueRequestId} {name}", config.UniqueRequestId, config.SMChannel.Name);
            return;
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
        stat.ChannelDistributor.Stop();
        await channelService.CheckForEmptyChannelsAsync();

        //IChannelDistributor? channelDistributor = channelDistributorService.GetChannelDistributor(stat.SMStreamInfo.Url);

        //if (channelDistributor is not null)
        //{
        //    channelDistributor.Stop();

        //    //foreach (ClientConfiguration? config in handler.GetChannelStatuses.SelectMany(a => a.ClientStreamerConfigurations.Values))
        //    //{
        //    //    await CancelClient(config.UniqueRequestId);
        //    //}
        //    await channelService.CheckForEmptyChannelsAsync();

        //    logger.LogInformation("Simulating stream failure for: {VideoStreamName}", stat.SMStreamInfo.SourceName);
        //}
        //else
        //{
        //    logger.LogWarning("Stream not found, cannot simulate stream failure: {StreamUrl}", SMChannelId);
        //}
    }

    public void MoveToNextStream(int SMChannelId)
    {
        IChannelStatus? channelStatus = channelService.GetChannelStatus(SMChannelId);
        if (channelStatus is null)
        {
            logger.LogWarning("Channel not found: {SMChannelId}", SMChannelId);
            return;
        }

        IChannelDistributor? channelDistributor = channelDistributorService.GetChannelDistributor(channelStatus.SMStreamInfo.Url);

        if (channelDistributor is not null)
        {
            channelDistributor.Stop();

            logger.LogInformation("Simulating stream failure for: {VideoStreamName}", channelStatus.SMStreamInfo.Name);
        }
        else
        {
            logger.LogWarning("Stream not found, cannot simulate stream failure: {StreamUrl}", SMChannelId);
        }
    }

    public void CancelAllChannels()
    {
        foreach (IChannelDistributor s in channelDistributorService.GetChannelDistributors())
        {
            s.Stop();
        }
    }
}