using StreamMaster.Domain.Enums;
using StreamMaster.Streams.Domain.Events;

namespace StreamMaster.Streams.Channels;

public sealed class ChannelService : IChannelService
{
    private readonly ILogger<ChannelService> logger;
    private readonly ISwitchToNextStreamService _switchToNextStreamService;
    private readonly ISourceBroadcasterService _sourceBroadcasterService;
    private readonly IChannelBroadcasterService _channelBroadcasterService;
    private readonly IStreamLimitsService _streamLimitsService;
    private readonly IChannelLockService _channelLockService;
    private readonly ICacheManager _cacheManager;
    private readonly IMessageService messageService;
    private readonly Lock _disposeLock = new();
    private bool _disposed = false;
    private readonly SemaphoreSlim _registerSemaphore = new(1, 1);

    public ChannelService(
        ILogger<ChannelService> logger,
        IStreamLimitsService streamLimitsService,
        IChannelLockService channelLockService,
        ISourceBroadcasterService sourceBroadcasterService,
        IChannelBroadcasterService channelBroadcasterService,
        ICacheManager cacheManager,
        IMessageService messageService,
        ISwitchToNextStreamService switchToNextStreamService)
    {
        this.messageService = messageService;
        _channelLockService = channelLockService;
        _streamLimitsService = streamLimitsService;
        _channelBroadcasterService = channelBroadcasterService;
        _cacheManager = cacheManager;
        _sourceBroadcasterService = sourceBroadcasterService;
        _switchToNextStreamService = switchToNextStreamService;
        this.logger = logger;
        _sourceBroadcasterService.OnStreamBroadcasterStoppedEvent += StreamBroadcasterService_OnStreamBroadcasterStoppedEventAsync;
        _channelBroadcasterService.OnChannelBroadcasterStoppedEvent += ChannelBroadscasterService_OnChannelBroadcasterStoppedEventAsync;
    }

    /// <inheritdoc/>
    public async Task MoveToNextStreamAsync(int smChannelId)
    {
        IChannelBroadcaster? channelStatus = GetChannelBroadcaster(smChannelId);
        if (channelStatus is null || channelStatus.SMStreamInfo is null)
        {
            logger.LogWarning("Channel not found: {smChannelId}", smChannelId);
            return;
        }

        ISourceBroadcaster? sourceBroadcaster = _sourceBroadcasterService.GetStreamBroadcaster(channelStatus.SMStreamInfo.Url);

        if (sourceBroadcaster is not null)
        {
            await sourceBroadcaster.StopAsync();
            logger.LogInformation("Simulating stream failure for: {VideoStreamName}", channelStatus.SMStreamInfo.Name);
        }
        else
        {
            logger.LogWarning("Stream not found, cannot simulate stream failure: {smChannelId}", smChannelId);
        }
    }

    /// <inheritdoc/>
    public async Task CancelAllChannelsAsync()
    {
        foreach (ISourceBroadcaster sourceBroadcaster in _sourceBroadcasterService.GetStreamBroadcasters())
        {
            await sourceBroadcaster.StopAsync();
        }
    }

    /// <inheritdoc/>
    public async Task ChangeVideoStreamChannelAsync(string playingSMStreamId, string newSMStreamId, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Starting ChangeVideoStreamChannel with playingSMStreamId: {playingSMStreamId} and newSMStreamId: {newSMStreamId}", playingSMStreamId, newSMStreamId);

        foreach (IChannelBroadcaster channelStatus in GetChannelStatusesFromSMStreamId(playingSMStreamId))
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            if (!await SwitchChannelToNextStreamAsync(channelStatus, clientConfiguration: null, newSMStreamId))
            {
                logger.LogWarning("Exiting ChangeVideoStreamChannel. Could not change channel to {newSMStreamId}", newSMStreamId);
                return;
            }

            return;
        }

        logger.LogWarning("Channel not found: {playingSMStreamId}", playingSMStreamId);
        logger.LogDebug("Exiting ChangeVideoStreamChannel due to channel not found.");
    }

    private async Task ChannelBroadscasterService_OnChannelBroadcasterStoppedEventAsync(object? sender, ChannelBroascasterStopped e)
    {
        if (sender is IChannelBroadcaster channelBroadcaster)
        {
            logger.LogInformation("Channel Stopped Event for channel Id: {StreamName}", e.Name);
            if (channelBroadcaster.SMStreamInfo != null)
            {
                await _sourceBroadcasterService.UnRegisterChannelBroadcasterAsync(channelBroadcaster.Id);
            }
        }
    }

    private async Task StreamBroadcasterService_OnStreamBroadcasterStoppedEventAsync(object? sender, StreamBroadcasterStopped e)
    {
        if (sender is ISourceBroadcaster sourceBroadcaster)
        {
            logger.LogInformation("Streaming Stopped Event for stream Id: {StreamName}", e.Name);

            List<IChannelBroadcaster> channelBroadcasters = [.. _cacheManager.ChannelBroadcasters.Values.Where(a => a.SMStreamInfo?.Url == e.Id)];

            foreach (IChannelBroadcaster? channelBroadcaster in channelBroadcasters)
            {
                if (channelBroadcaster.Shutdown || channelBroadcaster.FailoverInProgress)
                {
                    continue;
                }

                bool didSwitch = await SwitchChannelToNextStreamAsync(channelBroadcaster, clientConfiguration: null).ConfigureAwait(false);
                if (!didSwitch)
                {
                    await StopChannelAsync(channelBroadcaster.Id);
                    // clientConfiguration.Stop();
                }
            }
        }
    }

    /// <inheritdoc/>
    public async Task<bool> AddClientToChannelAsync(IClientConfiguration clientConfiguration, int streamGroupProfileId, CancellationToken cancellationToken = default)
    {
        try
        {
            logger.LogInformation("Client {UniqueRequestId} requesting channel {Name}.", clientConfiguration.UniqueRequestId, clientConfiguration.SMChannel.Name);

            if (cancellationToken.IsCancellationRequested)
            {
                logger.LogInformation("Exiting GetChannelStreamAsync {UniqueRequestId} {Name} due to cancellation.", clientConfiguration.UniqueRequestId, clientConfiguration.SMChannel.Name);
                return false;
            }

            IChannelBroadcaster? channelBroadcaster = await GetOrCreateChannelBroadcasterAsync(clientConfiguration, streamGroupProfileId);

            if (channelBroadcaster == null)
            {
                await UnRegisterClientAsync(clientConfiguration.UniqueRequestId, cancellationToken);
                logger.LogInformation("Exiting GetChannelStreamAsync:  {UniqueRequestId} channel {Name} status is null.", clientConfiguration.UniqueRequestId, clientConfiguration.SMChannel.Name);
                return false;
            }

            channelBroadcaster.AddChannelStreamer(clientConfiguration);

            await messageService.SendInfo($"Client started streaming {clientConfiguration.SMChannel.Name}", "Client Start");
            return true;
        }
        finally
        {
            //logger.LogInformation("Client {UniqueRequestId} requesting channel {Name} release.", clientConfiguration.UniqueRequestId, clientConfiguration.SMChannel.Name);

            //_ = _registerSemaphore.Release();
        }
    }

    public async Task StopChannelAsync(int channelId)
    {
        await _channelBroadcasterService.StopChannelAsync(channelId);
        await _sourceBroadcasterService.UnRegisterChannelBroadcasterAsync(channelId);
    }

    public IClientConfiguration? GetClientStreamerConfiguration(string uniqueRequestId)
    {
        IClientConfiguration? config = _cacheManager.ChannelBroadcasters.Values
        .SelectMany(a => a.Clients.Values)
        .FirstOrDefault(a => a.UniqueRequestId == uniqueRequestId);

        if (config != null)
        {
            return config;
        }

        logger.LogDebug("Client configuration for {UniqueRequestId} not found", uniqueRequestId);
        return null;
    }

    public List<IClientConfiguration> GetClientStreamerConfigurations()
    {
        return [.. _cacheManager.ChannelBroadcasters.Values.SelectMany(a => a.Clients.Values)];
    }

    public async Task<IChannelBroadcaster?> GetOrCreateChannelBroadcasterAsync(IClientConfiguration clientConfiguration, int streamGroupProfileId)
    {
        bool lockAquired = false;
        try
        {
            // Validate that there are streams or channels available depending on the type.
            if (clientConfiguration.SMChannel.SMChannelType == SMChannelTypeEnum.MultiView)
            {
                if (clientConfiguration.SMChannel.SMChannelDtos.Count == 0)
                {
                    logger.LogInformation("MultiView Channel with ChannelId {smChannelId} has no channels, exiting", clientConfiguration.SMChannel.Id);
                    return null;
                }
            }
            else
            {
                if (clientConfiguration.SMChannel.SMStreamDtos.Count == 0)
                {
                    logger.LogInformation("Channel with ChannelId {smChannelId} has no streams, exiting", clientConfiguration.SMChannel.Id);
                    return null;
                }
            }

            // Attempt to get an existing channel broadcaster.
            IChannelBroadcaster? channelBroadcaster = GetChannelBroadcaster(clientConfiguration.SMChannel.Id);

            // If no broadcaster is found, we need to create one.
            if (channelBroadcaster == null)
            {
                // Conditional locking based on channel type and stream limits.
                if (clientConfiguration.SMChannel.SMChannelType != SMChannelTypeEnum.MultiView)
                {
                    string firstStream = clientConfiguration.SMChannel.SMStreamDtos[0].Id;
                    (int currentStreamCount, int maxStreamCount) = _streamLimitsService.GetStreamLimits(firstStream);

                    // Only acquire the lock if the stream count limit is near.
                    if (currentStreamCount >= maxStreamCount - 1)
                    {
                        await _channelLockService.AcquireLockAsync(clientConfiguration.SMChannel.Id);
                        lockAquired = true;
                    }
                }
                else
                {
                    // Always acquire the lock for MultiView channels.
                    await _channelLockService.AcquireLockAsync(clientConfiguration.SMChannel.Id);
                    lockAquired = true;
                }

                // Double-check after lock acquisition to avoid race conditions.
                channelBroadcaster = GetChannelBroadcaster(clientConfiguration.SMChannel.Id);
                if (channelBroadcaster == null)
                {
                    logger.LogInformation("No existing channel for {ChannelVideoStreamId} {name}", clientConfiguration.SMChannel.Id, clientConfiguration.SMChannel.Name);

                    channelBroadcaster = await _channelBroadcasterService
                        .GetOrCreateChannelBroadcasterAsync(clientConfiguration, streamGroupProfileId, CancellationToken.None)
                        .ConfigureAwait(false);

                    // Handle MultiView specific setup.
                    if (clientConfiguration.SMChannel.SMChannelType == SMChannelTypeEnum.MultiView)
                    {
                        if (!await SetupMultiView(channelBroadcaster).ConfigureAwait(false))
                        {
                            await StopChannelAsync(channelBroadcaster.Id);
                            clientConfiguration.Stop();
                            return null;
                        }
                    }
                    // Handle regular channel setup.
                    else
                    {
                        if (!await SwitchChannelToNextStreamAsync(channelBroadcaster, clientConfiguration).ConfigureAwait(false))
                        {
                            await StopChannelAsync(channelBroadcaster.Id);
                            clientConfiguration.Stop();
                            return null;
                        }
                    }
                }
            }
            else
            {
                // Log and reuse the existing channel broadcaster.
                logger.LogInformation("Reuse existing stream handler for {ChannelVideoStreamId} {name}", clientConfiguration.SMChannel.Id, clientConfiguration.SMChannel.Name);
                clientConfiguration.SMChannel.CurrentRank = channelBroadcaster.SMChannel.CurrentRank;
            }

            //// Handle stream validation for regular channels.
            //if (clientConfiguration.SMChannel.SMChannelType == SMChannelTypeEnum.Regular)
            //{
            //    if (channelBroadcaster.SMStreamInfo == null)
            //    {
            //        await StopChannelAsync(channelBroadcaster.Id);
            //        clientConfiguration.Stop();
            //        return null;
            //    }

            //    ISourceBroadcaster? streamBroadcaster = _sourceBroadcasterService.GetStreamBroadcaster(channelBroadcaster.SMStreamInfo.Url);

            //    // Check if the stream broadcaster is failed.
            //    if (streamBroadcaster?.IsFailed != false)
            //    {
            //        if (!await SwitchChannelToNextStreamAsync(channelBroadcaster, clientConfiguration).ConfigureAwait(false))
            //        {
            //            logger.LogError("SwitchChannelToNextStream failed for {UniqueRequestId} {ChannelVideoStreamId} {name}", clientConfiguration.UniqueRequestId, clientConfiguration.SMChannel.Id, clientConfiguration.SMChannel.Name);

            //            await StopChannelAsync(channelBroadcaster.Id);
            //            clientConfiguration.Stop();
            //            return null;
            //        }
            //    }
            //}

            // Add the client streamer to the channel broadcaster.
            //await channelBroadcaster.AddChannelStreamer(clientConfiguration);

            return channelBroadcaster;
        }
        finally
        {
            // Release the lock regardless of success or failure.
            if (lockAquired)
            {
                _channelLockService.ReleaseLock(clientConfiguration.SMChannel.Id);
            }
        }
    }
    public IChannelBroadcaster? GetChannelBroadcaster(int smChannelId)
    {
        _cacheManager.ChannelBroadcasters.TryGetValue(smChannelId, out IChannelBroadcaster? channelBroadcaster);
        return channelBroadcaster;
    }

    public List<IChannelBroadcaster> GetChannelStatusesFromSMStreamId(string streamId)
    {
        if (string.IsNullOrEmpty(streamId))
        {
            logger.LogError("StreamId is null or empty");
            return [];
        }

        try
        {
            return [.. _cacheManager.ChannelBroadcasters.Values.Where(a => a?.SMStreamInfo?.Id == streamId)];
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while getting channel statuses from SMStream ID {StreamId}.", streamId);
            return [];
        }
    }

    public IChannelBroadcaster? GetChannelStatusFromSMChannelId(int smChannelId)
    {
        return _cacheManager.ChannelBroadcasters.TryGetValue(smChannelId, out IChannelBroadcaster? channelBroadcaster) ? channelBroadcaster : null;
    }

    public List<IChannelBroadcaster> GetChannelStatusFromStreamUrl(string videoUrl)
    {
        return [.. _cacheManager.ChannelBroadcasters.Values.Where(a => a?.SMStreamInfo?.Url == videoUrl)];
    }

    public List<IChannelBroadcaster> GetChannelStatuses()
    {
        return [.. _cacheManager.ChannelBroadcasters.Values];
    }

    //private static bool ChannelHasStreamsOrChannels(SMChannel smChannel)
    //{
    //    return smChannel.SMChannelType == StreamMaster.Domain.Enums.SMChannelTypeEnum.MultiView
    //        ? smChannel.SMChannels.Count > 0
    //        : smChannel.SMStreams.Count > 0 && !string.IsNullOrEmpty(smChannel.SMStreams.First().SMStream!.Url);
    //}

    public bool HasChannel(int smChannelId)
    {
        return _cacheManager.ChannelBroadcasters.ContainsKey(smChannelId);
    }

    public int GetGlobalStreamsCount()
    {
        return _cacheManager.ChannelBroadcasters.Values.Count(a => a.IsGlobal);
    }

    public async Task<bool> SwitchChannelToNextStreamAsync(IChannelBroadcaster channelBroadcaster, IClientConfiguration? clientConfiguration, string? overrideSMStreamId = null)
    {
        if (channelBroadcaster.FailoverInProgress)
        {
            return false;
        }

        ISourceBroadcaster? sourceChannelBroadcaster = null;

        while (sourceChannelBroadcaster == null)
        {
            logger.LogDebug("Starting SwitchToNextStream with channelBroadcaster: {channelBroadcaster} and overrideNextVideoStreamId: {overrideNextVideoStreamId}", channelBroadcaster, overrideSMStreamId);

            bool didChange = await _switchToNextStreamService.SetNextStreamAsync(channelBroadcaster, overrideSMStreamId).ConfigureAwait(false);
            if (channelBroadcaster.SMStreamInfo == null || !didChange)
            {
                logger.LogDebug("Exiting SwitchToNextStream with false due to smStream being null");
                channelBroadcaster.FailoverInProgress = false;
                return false;
            }

            sourceChannelBroadcaster = await _sourceBroadcasterService.GetOrCreateStreamBroadcasterAsync(channelBroadcaster, CancellationToken.None).ConfigureAwait(false);

            if (sourceChannelBroadcaster != null)
            {
                break;
            }
        }

        if (sourceChannelBroadcaster == null)
        {
            logger.LogDebug("Exiting, Source Channel Distributor is null");
            channelBroadcaster.FailoverInProgress = false;
            return false;
        }

        channelBroadcaster.SetSourceChannelBroadcaster(sourceChannelBroadcaster);

        channelBroadcaster.FailoverInProgress = false;

        logger.LogDebug("Finished SwitchToNextStream");
        return true;
    }

    private async Task<bool> SetupMultiView(IChannelBroadcaster channelBroadcaster)
    {
        if (channelBroadcaster.FailoverInProgress)
        {
            return false;
        }

        logger.LogDebug("Starting SetupMultiView with channelBroadcaster: {channelBroadcaster}", channelBroadcaster);

        SMStreamInfo smStreamInfo = new()
        {
            Id = channelBroadcaster.SMChannel.Id.ToString(),
            Name = channelBroadcaster.SMChannel.Name,
            Url = channelBroadcaster.SMChannel.Id.ToString(),
            SMStreamType = SMStreamTypeEnum.Custom,
            ClientUserAgent = "",
            CommandProfile = new()
        };

        channelBroadcaster.SetSMStreamInfo(smStreamInfo);

        ISourceBroadcaster? sourceChannelBroadcaster = await _sourceBroadcasterService.GetOrCreateStreamBroadcasterAsync(channelBroadcaster, CancellationToken.None).ConfigureAwait(false);

        if (sourceChannelBroadcaster == null)
        {
            logger.LogDebug("Exiting, Source Channel Distributor is null");
            channelBroadcaster.FailoverInProgress = false;
            return false;
        }

        channelBroadcaster.SetSourceChannelBroadcaster(sourceChannelBroadcaster);

        channelBroadcaster.FailoverInProgress = false;

        logger.LogDebug("Finished SwitchToNextStream");
        return true;
    }

    public async Task UnRegisterClientAsync(string uniqueRequestId, CancellationToken cancellationToken = default)
    {
        await _channelBroadcasterService.UnRegisterClientAsync(uniqueRequestId, cancellationToken);
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
                _registerSemaphore.Dispose();

                foreach (IChannelBroadcaster channelBroadcaster in _cacheManager.ChannelBroadcasters.Values)
                {
                    channelBroadcaster.Stop();
                }
                _cacheManager.ChannelBroadcasters.Clear();

                // Unsubscribe events
                _sourceBroadcasterService.OnStreamBroadcasterStoppedEvent -= StreamBroadcasterService_OnStreamBroadcasterStoppedEventAsync;
                _channelBroadcasterService.OnChannelBroadcasterStoppedEvent -= ChannelBroadscasterService_OnChannelBroadcasterStoppedEventAsync;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while disposing the ChannelService.");
            }
            finally
            {
                _disposed = true;
            }
        }
    }
}