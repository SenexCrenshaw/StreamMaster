using StreamMaster.Domain.Enums;
using StreamMaster.Streams.Domain.Events;
using StreamMaster.Streams.Services;

namespace StreamMaster.Streams.Channels;

public sealed class ChannelService : IChannelService
{
    private readonly ILogger<ChannelService> logger;
    private readonly ISwitchToNextStreamService _switchToNextStreamService;
    private readonly ISourceBroadcasterService _sourceBroadcasterService;
    private readonly IChannelBroadcasterService _channelBroadcasterService;
    private readonly IStreamLimitsService _streamLimitsService;
    private readonly ChannelLockService _channelLockService = new();
    private readonly ICacheManager _cacheManager;
    private readonly IMessageService messageService;
    private readonly Lock _disposeLock = new();
    private bool _disposed = false;
    private readonly SemaphoreSlim _registerSemaphore = new(1, 1);

    public ChannelService(
        ILogger<ChannelService> logger,
        IStreamLimitsService streamLimitsService,
        ISourceBroadcasterService sourceBroadcasterService,
        IChannelBroadcasterService channelBroadcasterService,
        ICacheManager cacheManager,
        IMessageService messageService,
        ISwitchToNextStreamService switchToNextStreamService)
    {
        this.messageService = messageService;
        _streamLimitsService = streamLimitsService;
        _channelBroadcasterService = channelBroadcasterService;
        _cacheManager = cacheManager;
        _sourceBroadcasterService = sourceBroadcasterService;
        _switchToNextStreamService = switchToNextStreamService;
        this.logger = logger;
        _sourceBroadcasterService.OnStreamBroadcasterStopped += StreamBroadcasterService_OnStreamBroadcasterStoppedEventAsync;
    }

    /// <inheritdoc/>
    public async Task MoveToNextStreamAsync(int smChannelId)
    {
        IChannelBroadcaster? channel = GetChannelBroadcaster(smChannelId);
        if (channel?.SMStreamInfo is null)
        {
            logger.LogWarning("Channel not found or has no stream info: {smChannelId}", smChannelId);
            return;
        }

        ISourceBroadcaster? sourceBroadcaster = _sourceBroadcasterService.GetStreamBroadcaster(channel.SMStreamInfo.Url);
        if (sourceBroadcaster != null)
        {
            await sourceBroadcaster.StopAsync();
            logger.LogInformation("Simulated stream failure for {StreamName}", channel.SMStreamInfo.Name);
        }
        else
        {
            logger.LogWarning("Stream broadcaster not found for channel: {smChannelId}", smChannelId);
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
        foreach (IChannelBroadcaster channel in GetChannelStatusesFromSMStreamId(playingSMStreamId))
        {
            if (cancellationToken.IsCancellationRequested)
            {
                logger.LogInformation("ChangeVideoStreamChannel canceled.");
                return;
            }

            if (!await SwitchChannelToNextStreamAsync(channel, clientConfiguration: null, overrideSMStreamId: newSMStreamId))
            {
                logger.LogWarning("Failed to switch channel to {newSMStreamId}", newSMStreamId);
                return;
            }
        }

        logger.LogWarning("Channel with playing SM stream ID {playingSMStreamId} not found", playingSMStreamId);
    }

    private async Task StreamBroadcasterService_OnStreamBroadcasterStoppedEventAsync(object? sender, SourceBroadcasterStopped e)
    {
        if (sender is ISourceBroadcaster sourceBroadcaster)
        {
            //logger.LogInformation("Streaming Stopped Event for stream Id: {StreamName}", e.Name);

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
                }
            }
        }
    }

    /// <inheritdoc/>
    public async Task<bool> AddClientToChannelAsync(IClientConfiguration clientConfiguration, int streamGroupProfileId, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Adding client {UniqueRequestId} to channel {Name}.", clientConfiguration.UniqueRequestId, clientConfiguration.SMChannel.Name);

        if (cancellationToken.IsCancellationRequested)
        {
            logger.LogInformation("AddClientToChannelAsync canceled for client {UniqueRequestId}", clientConfiguration.UniqueRequestId);
            return false;
        }

        IChannelBroadcaster? channelBroadcaster = await GetOrCreateChannelBroadcasterAsync(clientConfiguration, streamGroupProfileId);
        if (channelBroadcaster == null)
        {
            await UnRegisterClientAsync(clientConfiguration.UniqueRequestId, cancellationToken);
            logger.LogWarning("Failed to add client {UniqueRequestId} to channel {Name}.", clientConfiguration.UniqueRequestId, clientConfiguration.SMChannel.Name);
            return false;
        }

        channelBroadcaster.AddChannelStreamer(clientConfiguration);
        await messageService.SendInfo($"Client started streaming {clientConfiguration.SMChannel.Name}", "Client Start");
        return true;
    }

    public async Task StopChannelAsync(int channelId)
    {
        await _channelBroadcasterService.StopChannelBroadcasterAsync(channelId, true);
        //await _sourceBroadcasterService.UnRegisterChannelBroadcasterAsync(channelId);
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
            //        await StopChannelBroadcasterAsync(channelBroadcaster.Id);
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

            //            await StopChannelBroadcasterAsync(channelBroadcaster.Id);
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

        ISourceBroadcaster? sourceBroadcaster = null;

        while (sourceBroadcaster == null)
        {
            logger.LogDebug("Starting SwitchToNextStream with channelBroadcaster: {channelBroadcaster} and overrideNextVideoStreamId: {overrideNextVideoStreamId}", channelBroadcaster, overrideSMStreamId);

            bool didChange = await _switchToNextStreamService.SetNextStreamAsync(channelBroadcaster, overrideSMStreamId).ConfigureAwait(false);
            if (channelBroadcaster.SMStreamInfo == null || !didChange)
            {
                logger.LogDebug("Exiting SwitchToNextStream with false due to smStream being null");
                channelBroadcaster.FailoverInProgress = false;
                return false;
            }

            sourceBroadcaster = await _sourceBroadcasterService.GetOrCreateStreamBroadcasterAsync(channelBroadcaster.SMStreamInfo, CancellationToken.None).ConfigureAwait(false);

            if (sourceBroadcaster != null)
            {
                break;
            }
        }

        if (sourceBroadcaster == null)
        {
            logger.LogDebug("Exiting, Source Channel Distributor is null");
            channelBroadcaster.FailoverInProgress = false;
            return false;
        }

        sourceBroadcaster.AddChannelBroadcaster(channelBroadcaster);
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

        ISourceBroadcaster? sourceBroadcaster = await _sourceBroadcasterService.GetOrCreateMultiViewStreamBroadcasterAsync(channelBroadcaster, CancellationToken.None).ConfigureAwait(false);

        if (sourceBroadcaster == null)
        {
            logger.LogDebug("Exiting, Source Channel Distributor is null");
            channelBroadcaster.FailoverInProgress = false;
            return false;
        }

        sourceBroadcaster.AddChannelBroadcaster(channelBroadcaster);

        channelBroadcaster.FailoverInProgress = false;

        logger.LogDebug("Finished SwitchToNextStream");
        return true;
    }

    public async Task UnRegisterClientAsync(string uniqueRequestId, CancellationToken cancellationToken = default)
    {
        await _channelBroadcasterService.UnRegisterClientAsync(uniqueRequestId, cancellationToken);
    }

    /// <inheritdoc/>
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

                foreach (IChannelBroadcaster broadcaster in _cacheManager.ChannelBroadcasters.Values)
                {
                    broadcaster.Stop();
                }

                _cacheManager.ChannelBroadcasters.Clear();

                // Unsubscribe events
                _sourceBroadcasterService.OnStreamBroadcasterStopped -= StreamBroadcasterService_OnStreamBroadcasterStoppedEventAsync;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while disposing ChannelService.");
            }
            finally
            {
                _disposed = true;
            }
        }
    }
}