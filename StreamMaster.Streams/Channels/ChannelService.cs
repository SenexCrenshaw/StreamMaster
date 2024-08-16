using StreamMaster.Streams.Domain.Events;

namespace StreamMaster.Streams.Channels
{
    public sealed class ChannelService : IChannelService, IDisposable
    {
        private readonly ILogger<ChannelService> _logger;
        private readonly ISwitchToNextStreamService _switchToNextStreamService;
        private readonly IChannelBroadcasterService _channelDistributorService;
        private readonly IChannelStatusService _channelStatusService;
        private readonly IVideoInfoService _videoInfoService;
        private readonly ICacheManager _cacheManager;
        private readonly IOptionsMonitor<Setting> _settings;

        private readonly object _disposeLock = new();
        private bool _disposed = false;

        public ChannelService(
            ILogger<ChannelService> logger,
            IOptionsMonitor<Setting> settings,
            IVideoInfoService videoInfoService,
            IChannelBroadcasterService channelDistributorService,
            IChannelStatusService channelStatusService,
            ICacheManager cacheManager,
            ISwitchToNextStreamService switchToNextStreamService)
        {
            _channelStatusService = channelStatusService;
            _cacheManager = cacheManager;
            _videoInfoService = videoInfoService;
            _channelDistributorService = channelDistributorService;
            _switchToNextStreamService = switchToNextStreamService;
            _logger = logger;
            _settings = settings;

            _channelDistributorService.OnChannelDirectorStoppedEvent += ChannelDistributorService_OnStoppedEventAsync;
            _channelStatusService.OnChannelStatusStoppedEvent += ChannelStatusService_OnChannelStatusStoppedEventAsync;
        }

        private async Task ChannelStatusService_OnChannelStatusStoppedEventAsync(object? sender, ChannelStatusStopped e)
        {
            if (sender is IChannelStatus channelStatus)
            {
                _logger.LogInformation("Streaming Stopped Event for StreamId: {StreamId} {StreamName}", e.Id, e.Name);
                await CloseChannelAsync(channelStatus).ConfigureAwait(false);
            }
        }

        private async Task ChannelDistributorService_OnStoppedEventAsync(object? sender, ChannelDirectorStopped e)
        {
            if (sender is IChannelBroadcaster channelDistributor)
            {
                _logger.LogInformation("Streaming Stopped Event for StreamId: {StreamId} {StreamName}", e.Id, e.Name);

                List<IChannelStatus> affectedChannelStatuses = _cacheManager.ChannelStatuses.Values
                    .Where(a => a.SMStreamInfo?.Url == e.Id)
                    .ToList();

                foreach (IChannelStatus? channelStatus in affectedChannelStatuses)
                {
                    if (channelStatus.Shutdown || channelStatus.FailoverInProgress)
                    {
                        continue;
                    }

                    bool didSwitch = await SwitchChannelToNextStreamAsync(channelStatus).ConfigureAwait(false);
                    if (!didSwitch)
                    {
                        await CloseChannelAsync(channelStatus).ConfigureAwait(false);
                    }
                }
            }
        }

        public async Task<IClientConfiguration?> GetClientStreamerConfigurationAsync(string uniqueRequestId, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            IClientConfiguration? config = _cacheManager.ChannelStatuses.Values
                .SelectMany(a => a.GetClientStreamerConfigurations())
                .FirstOrDefault(a => a.UniqueRequestId == uniqueRequestId);

            if (config != null)
            {
                return await Task.FromResult(config).ConfigureAwait(false);
            }

            _logger.LogDebug("Client configuration for {UniqueRequestId} not found", uniqueRequestId);
            return null;
        }

        public List<IClientConfiguration> GetClientStreamerConfigurations()
        {
            return _cacheManager.ChannelStatuses.Values
                .SelectMany(a => a.GetClientStreamerConfigurations())
                .ToList();
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
                    foreach (IChannelStatus channelStatus in _cacheManager.ChannelStatuses.Values)
                    {
                        channelStatus.Stop();
                    }
                    _cacheManager.ChannelStatuses.Clear();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while disposing the ChannelService.");
                }
                finally
                {
                    _disposed = true;
                }
            }
        }

        public async Task<IChannelStatus?> GetOrCreateChannelStatusAsync(IClientConfiguration config, int streamGroupProfileId)
        {
            IChannelStatus? channelStatus = GetChannelStatus(config.SMChannel.Id);

            if (channelStatus == null)
            {
                _logger.LogInformation("No existing channel for {ChannelVideoStreamId} {name}", config.SMChannel.Id, config.SMChannel.Name);

                channelStatus = await _channelStatusService.GetOrCreateChannelStatusAsync(config, streamGroupProfileId, CancellationToken.None).ConfigureAwait(false);
                _cacheManager.ChannelStatuses.TryAdd(config.SMChannel.Id, channelStatus);

                if (!await SwitchChannelToNextStreamAsync(channelStatus).ConfigureAwait(false))
                {
                    await UnRegisterChannelAsync(channelStatus);
                    await CheckForEmptyChannelsAsync().ConfigureAwait(false);
                    return null;
                }
            }
            else
            {
                _logger.LogInformation("Reuse existing stream handler for {ChannelVideoStreamId} {name}", config.SMChannel.Id, config.SMChannel.Name);
            }

            if (channelStatus.SMStreamInfo == null)
            {
                await UnRegisterChannelAsync(channelStatus);
                await CheckForEmptyChannelsAsync().ConfigureAwait(false);
                return null;
            }

            IChannelBroadcaster? channelDistributor = _channelDistributorService.GetChannelBroadcaster(channelStatus.SMStreamInfo.Url);
            if (channelDistributor?.IsFailed != false)
            {
                if (!await SwitchChannelToNextStreamAsync(channelStatus).ConfigureAwait(false))
                {
                    _logger.LogError("SwitchChannelToNextStream failed for {UniqueRequestId} {ChannelVideoStreamId} {name}", config.UniqueRequestId, config.SMChannel.Id, config.SMChannel.Name);
                    await UnRegisterChannelAsync(channelStatus);
                    await CheckForEmptyChannelsAsync().ConfigureAwait(false);
                    return null;
                }
            }

            channelStatus.AddClientStreamer(config.UniqueRequestId, config);

            return channelStatus;
        }

        public async Task<IChannelStatus?> SetupChannelAsync(SMChannelDto smChannel)
        {
            // Implementation commented out for brevity
            return null;
        }

        private List<string> RunningUrls => _cacheManager.ChannelStatuses.Values
            .Where(a => a.ClientCount > 0 && a.SMStreamInfo != null)
            .Select(a => a.SMStreamInfo!.Url)
            .Distinct()
            .ToList();

        public async Task CheckForEmptyChannelsAsync(CancellationToken cancellationToken = default)
        {
            List<Task> tasks = [];

            foreach (IChannelStatus? channelStatus in _cacheManager.ChannelStatuses.Values.Where(a => a.SMStreamInfo != null))
            {
                if (channelStatus.ClientCount == 0 && !RunningUrls.Contains(channelStatus.SMStreamInfo!.Url))
                {
                    tasks.Add(CloseChannelAsync(channelStatus));
                }
            }

            await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        public async Task CloseChannelAsync(IChannelStatus channelStatus, bool force = false)
        {
            channelStatus.Shutdown = true;

            int delay = force ? 0 : _settings.CurrentValue.ShutDownDelay;
            bool closed = delay > 0
                ? await UnRegisterChannelAfterDelayAsync(channelStatus, TimeSpan.FromMilliseconds(delay), CancellationToken.None).ConfigureAwait(false)
                : await UnRegisterChannelAsync(channelStatus).ConfigureAwait(false);

            if (closed)
            {
                channelStatus.Stop();
                foreach (IClientConfiguration config in channelStatus.GetClientStreamerConfigurations())
                {
                    config.ClientStream?.Flush();
                    config.ClientStream?.Dispose();
                    await config.Response.CompleteAsync().ConfigureAwait(false);
                }

                List<IChannelBroadcaster> channelBroadcasters = _channelDistributorService.GetChannelBroadcasters()
                    .Where(cd => cd.ClientChannels.Any(cc => cc.Key == channelStatus.Id.ToString()))
                    .ToList();

                foreach (IChannelBroadcaster? channelBroadcaster in channelBroadcasters)
                {
                    if (channelBroadcaster.RemoveClientChannel(channelStatus.Id))
                    {
                        _channelDistributorService.StopAndUnRegister(channelBroadcaster.Id);
                    }
                }

                await CheckForEmptyChannelsAsync().ConfigureAwait(false);
            }
        }

        private async Task<bool> UnRegisterChannelAfterDelayAsync(IChannelStatus channelStatus, TimeSpan delay, CancellationToken cancellationToken)
        {
            await Task.Delay(delay, cancellationToken).ConfigureAwait(false);

            return channelStatus.ClientCount == 0 && channelStatus.SMStreamInfo != null
                && await UnRegisterChannelAsync(channelStatus).ConfigureAwait(false);
        }

        private async Task<bool> UnRegisterChannelAsync(IChannelStatus channelStatus)
        {
            _cacheManager.ChannelStatuses.TryRemove(channelStatus.SMChannel.Id, out _);
            if (channelStatus.SMStreamInfo?.Url != null)
            {
                _channelStatusService.StopAndUnRegisterChannelStatus(channelStatus.Id);
            }

            foreach (IClientConfiguration config in channelStatus.GetClientStreamerConfigurations())
            {
                await UnRegisterClientAsync(config.UniqueRequestId).ConfigureAwait(false);
            }

            return true;
        }

        public IChannelStatus? GetChannelStatus(int smChannelId)
        {
            _cacheManager.ChannelStatuses.TryGetValue(smChannelId, out IChannelStatus? channelStatus);
            return channelStatus;
        }

        public List<IChannelStatus> GetChannelStatusesFromSMStreamId(string streamId)
        {
            if (string.IsNullOrEmpty(streamId))
            {
                _logger.LogError("StreamId is null or empty");
                return [];
            }

            try
            {
                return _cacheManager.ChannelStatuses.Values
                    .Where(a => a?.SMStreamInfo?.Id == streamId)
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting channel statuses from SMStream ID {StreamId}.", streamId);
                return [];
            }
        }

        public IChannelStatus? GetChannelStatusFromSMChannelId(int smChannelId)
        {
            return _cacheManager.ChannelStatuses.TryGetValue(smChannelId, out IChannelStatus? channelStatus) ? channelStatus : null;
        }

        public List<IChannelStatus> GetChannelStatusFromStreamUrl(string videoUrl)
        {
            return _cacheManager.ChannelStatuses.Values
                .Where(a => a?.SMStreamInfo?.Url == videoUrl)
                .ToList();
        }

        public List<IChannelStatus> GetChannelStatuses()
        {
            return [.. _cacheManager.ChannelStatuses.Values];
        }

        public bool HasChannel(int smChannelId)
        {
            return _cacheManager.ChannelStatuses.ContainsKey(smChannelId);
        }

        public int GetGlobalStreamsCount()
        {
            return _cacheManager.ChannelStatuses.Values.Count(a => a.IsGlobal);
        }

        public async Task<bool> SwitchChannelToNextStreamAsync(IChannelStatus channelStatus, string? overrideSMStreamId = null)
        {
            if (channelStatus.FailoverInProgress)
            {
                return false;
            }

            _logger.LogDebug("Starting SwitchToNextStream with channelStatus: {channelStatus} and overrideNextVideoStreamId: {overrideNextVideoStreamId}", channelStatus, overrideSMStreamId);

            bool didChange = await _switchToNextStreamService.SetNextStreamAsync(channelStatus, overrideSMStreamId).ConfigureAwait(false);

            if (channelStatus.SMStreamInfo == null || !didChange)
            {
                _logger.LogDebug("Exiting SwitchToNextStream with false due to smStream being null");
                channelStatus.FailoverInProgress = false;
                return false;
            }

            IChannelBroadcaster? sourceChannelBroadcaster = await _channelDistributorService.GetOrCreateChannelDistributorAsync(channelStatus.SMChannel.Name, channelStatus.SMStreamInfo, CancellationToken.None).ConfigureAwait(false);

            if (sourceChannelBroadcaster == null)
            {
                _logger.LogDebug("Exiting, Source Channel Distributor is null");
                channelStatus.FailoverInProgress = false;
                return false;
            }

            channelStatus.SetSourceChannelBroadcaster(sourceChannelBroadcaster);

            if (!channelStatus.SMStreamInfo.Id.StartsWith(IntroPlayListBuilder.IntroIDPrefix, StringComparison.InvariantCulture))
            {
                _videoInfoService.SetSourceChannel(sourceChannelBroadcaster, channelStatus.SMStreamInfo.Id, channelStatus.SMStreamInfo.Name);
            }

            channelStatus.FailoverInProgress = false;

            _logger.LogDebug("Finished SwitchToNextStream");
            return true;
        }

        public async Task<bool> UnRegisterClientAsync(string uniqueRequestId, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            bool removed = false;
            foreach (IChannelStatus channelStatus in _cacheManager.ChannelStatuses.Values)
            {
                List<IClientConfiguration> clientConfigs = channelStatus.GetClientStreamerConfigurations();
                IClientConfiguration? configToRemove = clientConfigs.Find(a => a.UniqueRequestId == uniqueRequestId);

                if (configToRemove != null && channelStatus.RemoveClientStreamer(uniqueRequestId))
                {
                    removed = true;
                    break;
                }
            }

            if (removed)
            {
                await CheckForEmptyChannelsAsync(cancellationToken).ConfigureAwait(false);
                _logger.LogDebug("Client configuration for {UniqueRequestId} removed", uniqueRequestId);
            }
            else
            {
                _logger.LogDebug("Client configuration for {UniqueRequestId} not found", uniqueRequestId);
            }

            return await Task.FromResult(removed).ConfigureAwait(false);
        }
    }
}
