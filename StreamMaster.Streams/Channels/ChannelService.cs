using StreamMaster.Streams.Domain.Events;

namespace StreamMaster.Streams.Channels
{
    public sealed class ChannelService : IChannelService, IDisposable
    {
        private readonly ILogger<ChannelService> _logger;
        private readonly ISwitchToNextStreamService _switchToNextStreamService;
        private readonly ISourceBroadcasterService _sourceBroadcasterService;
        private readonly IChannelBroadcasterService _channelBroadcasterService;
        private readonly ICacheManager _cacheManager;

        private readonly object _disposeLock = new();
        private bool _disposed = false;

        public ChannelService(
            ILogger<ChannelService> logger,
            ISourceBroadcasterService sourceBroadcasterService,
            IChannelBroadcasterService channelBroadcasterService,
            ICacheManager cacheManager,
            ISwitchToNextStreamService switchToNextStreamService)
        {
            _channelBroadcasterService = channelBroadcasterService;
            _cacheManager = cacheManager;

            _sourceBroadcasterService = sourceBroadcasterService;
            _switchToNextStreamService = switchToNextStreamService;
            _logger = logger;

            _sourceBroadcasterService.OnStreamBroadcasterStoppedEvent += StreamBroadcasterService_OnStreamBroadcasterStoppedEventAsync;
            _channelBroadcasterService._OnChannelBroadcasterStoppedEvent += ChannelBroadscasterService_OnChannelBroadcasterStoppedEventAsync;
        }

        private async Task ChannelBroadscasterService_OnChannelBroadcasterStoppedEventAsync(object? sender, ChannelBroascasterStopped e)
        {
            if (sender is IChannelBroadcaster channelBroadcaster)
            {
                _logger.LogInformation("Channel Stopped Event for channel Id: {StreamName}", e.Name);
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
                _logger.LogInformation("Streaming Stopped Event for stream Id: {StreamName}", e.Name);

                List<IChannelBroadcaster> channelBroadcasters = _cacheManager.ChannelBroadcasters.Values
                    .Where(a => a.SMStreamInfo?.Url == e.Id)
                    .ToList();

                foreach (IChannelBroadcaster? channelBroadcaster in channelBroadcasters)
                {
                    if (channelBroadcaster.Shutdown || channelBroadcaster.FailoverInProgress)
                    {
                        continue;
                    }

                    bool didSwitch = await SwitchChannelToNextStreamAsync(channelBroadcaster).ConfigureAwait(false);
                    if (!didSwitch)
                    {
                        await StopChannel(channelBroadcaster.Id);
                        // clientConfiguration.Stop();
                    }
                }
            }
        }

        public async Task StopChannel(int channelId)
        {
            await _channelBroadcasterService.StopChannelAsync(channelId);
            await _sourceBroadcasterService.UnRegisterChannelBroadcasterAsync(channelId);
        }

        public IClientConfiguration? GetClientStreamerConfiguration(string uniqueRequestId)
        {
            IClientConfiguration? config = _cacheManager.ChannelBroadcasters.Values
            .SelectMany(a => a.GetClientStreamerConfigurations())
            .FirstOrDefault(a => a.UniqueRequestId == uniqueRequestId);

            if (config != null)
            {
                return config;
            }

            _logger.LogDebug("Client configuration for {UniqueRequestId} not found", uniqueRequestId);
            return null;
        }

        public List<IClientConfiguration> GetClientStreamerConfigurations()
        {
            return _cacheManager.ChannelBroadcasters.Values
                .SelectMany(a => a.GetClientStreamerConfigurations())
                .ToList();
        }

        public async Task<IChannelBroadcaster?> GetOrCreateChannelBroadcasterAsync(IClientConfiguration clientConfiguration, int streamGroupProfileId)
        {
            IChannelBroadcaster? channelBroadcaster = GetChannelBroadcaster(clientConfiguration.SMChannel.Id);

            if (channelBroadcaster == null)
            {
                _logger.LogInformation("No existing channel for {ChannelVideoStreamId} {name}", clientConfiguration.SMChannel.Id, clientConfiguration.SMChannel.Name);

                channelBroadcaster = await _channelBroadcasterService.GetOrCreateChannelBroadcasterAsync(clientConfiguration, streamGroupProfileId, CancellationToken.None).ConfigureAwait(false);
                _cacheManager.ChannelBroadcasters.TryAdd(clientConfiguration.SMChannel.Id, channelBroadcaster);
                if (!await SwitchChannelToNextStreamAsync(channelBroadcaster).ConfigureAwait(false))
                {
                    await StopChannel(channelBroadcaster.Id);
                    clientConfiguration.Stop();

                    return null;
                }
            }
            else
            {
                _logger.LogInformation("Reuse existing stream handler for {ChannelVideoStreamId} {name}", clientConfiguration.SMChannel.Id, clientConfiguration.SMChannel.Name);
            }

            if (channelBroadcaster.SMStreamInfo == null)
            {
                await StopChannel(channelBroadcaster.Id);
                clientConfiguration.Stop();
                return null;
            }

            ISourceBroadcaster? streamBroadcaster = _sourceBroadcasterService.GetStreamBroadcaster(channelBroadcaster.SMStreamInfo.Url);
            if (streamBroadcaster?.IsFailed != false)
            {
                if (!await SwitchChannelToNextStreamAsync(channelBroadcaster).ConfigureAwait(false))
                {
                    _logger.LogError("SwitchChannelToNextStream failed for {UniqueRequestId} {ChannelVideoStreamId} {name}", clientConfiguration.UniqueRequestId, clientConfiguration.SMChannel.Id, clientConfiguration.SMChannel.Name);

                    await StopChannel(channelBroadcaster.Id);
                    clientConfiguration.Stop();
                    return null;
                }
            }

            channelBroadcaster.AddClientStreamer(clientConfiguration.UniqueRequestId, clientConfiguration);

            return channelBroadcaster;
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
                _logger.LogError("StreamId is null or empty");
                return [];
            }

            try
            {
                return _cacheManager.ChannelBroadcasters.Values
                    .Where(a => a?.SMStreamInfo?.Id == streamId)
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting channel statuses from SMStream ID {StreamId}.", streamId);
                return [];
            }
        }

        public IChannelBroadcaster? GetChannelStatusFromSMChannelId(int smChannelId)
        {
            return _cacheManager.ChannelBroadcasters.TryGetValue(smChannelId, out IChannelBroadcaster? channelBroadcaster) ? channelBroadcaster : null;
        }

        public List<IChannelBroadcaster> GetChannelStatusFromStreamUrl(string videoUrl)
        {
            return _cacheManager.ChannelBroadcasters.Values
                .Where(a => a?.SMStreamInfo?.Url == videoUrl)
                .ToList();
        }

        public List<IChannelBroadcaster> GetChannelStatuses()
        {
            return [.. _cacheManager.ChannelBroadcasters.Values];
        }

        public bool HasChannel(int smChannelId)
        {
            return _cacheManager.ChannelBroadcasters.ContainsKey(smChannelId);
        }

        public int GetGlobalStreamsCount()
        {
            return _cacheManager.ChannelBroadcasters.Values.Count(a => a.IsGlobal);
        }

        public async Task<bool> SwitchChannelToNextStreamAsync(IChannelBroadcaster channelBroadcaster, string? overrideSMStreamId = null)
        {
            if (channelBroadcaster.FailoverInProgress)
            {
                return false;
            }

            _logger.LogDebug("Starting SwitchToNextStream with channelBroadcaster: {channelBroadcaster} and overrideNextVideoStreamId: {overrideNextVideoStreamId}", channelBroadcaster, overrideSMStreamId);

            bool didChange = await _switchToNextStreamService.SetNextStreamAsync(channelBroadcaster, overrideSMStreamId).ConfigureAwait(false);

            if (channelBroadcaster.SMStreamInfo == null || !didChange)
            {
                _logger.LogDebug("Exiting SwitchToNextStream with false due to smStream being null");
                channelBroadcaster.FailoverInProgress = false;
                return false;
            }

            ISourceBroadcaster? sourceChannelBroadcaster = await _sourceBroadcasterService.GetOrCreateStreamBroadcasterAsync(channelBroadcaster.SMStreamInfo, CancellationToken.None).ConfigureAwait(false);

            if (sourceChannelBroadcaster == null)
            {
                _logger.LogDebug("Exiting, Source Channel Distributor is null");
                channelBroadcaster.FailoverInProgress = false;
                return false;
            }

            channelBroadcaster.SetSourceChannelBroadcaster(sourceChannelBroadcaster);

            channelBroadcaster.FailoverInProgress = false;

            _logger.LogDebug("Finished SwitchToNextStream");
            return true;
        }

        public async Task<bool> UnRegisterClientAsync(string uniqueRequestId, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (await _channelBroadcasterService.UnRegisterClientAsync(uniqueRequestId, cancellationToken))
            {
                _logger.LogDebug("Client configuration for {UniqueRequestId} removed", uniqueRequestId);
                return true;
            }

            _logger.LogDebug("Client configuration for {UniqueRequestId} not found", uniqueRequestId);
            return false;
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
                    foreach (IChannelBroadcaster channelBroascaster in _cacheManager.ChannelBroadcasters.Values)
                    {
                        channelBroascaster.Stop();
                    }
                    _cacheManager.ChannelBroadcasters.Clear();
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
    }
}
