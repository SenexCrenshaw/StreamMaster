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
            IOptionsMonitor<Setting> settings,
            IVideoInfoService videoInfoService,
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
                _logger.LogInformation("Streaming Stopped Event for channel Id: {StreamName}", e.Name);
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
                        //await StopChannelAsync(channelBroadcaster).ConfigureAwait(false);
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
                    //_channelBroadcasterService.StopBroadcaster(channelBroadcaster.Id);

                    //await _sourceBroadcasterService.UnRegisterChannelBroadcasterAsync(channelBroadcaster.Id);
                    //await CheckForEmptyBroadcastersAsync().ConfigureAwait(false);
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
                ////await _channelBroadcasterService.UnRegisterChannelAsync(channelBroadcaster);
                //_channelBroadcasterService.StopBroadcaster(channelBroadcaster.Id);
                ////channelBroadcaster.Stop();
                //await _sourceBroadcasterService.UnRegisterChannelBroadcasterAsync(channelBroadcaster.Id);
                ////await CheckForEmptyBroadcastersAsync().ConfigureAwait(false);
                return null;
            }

            ISourceBroadcaster? streamBroadcaster = _sourceBroadcasterService.GetStreamBroadcaster(channelBroadcaster.SMStreamInfo.Url);
            if (streamBroadcaster?.IsFailed != false)
            {
                if (!await SwitchChannelToNextStreamAsync(channelBroadcaster).ConfigureAwait(false))
                {
                    _logger.LogError("SwitchChannelToNextStream failed for {UniqueRequestId} {ChannelVideoStreamId} {name}", clientConfiguration.UniqueRequestId, clientConfiguration.SMChannel.Id, clientConfiguration.SMChannel.Name);
                    //await _channelBroadcasterService.UnRegisterChannelAsync(channelBroadcaster.Id);
                    //await _sourceBroadcasterService.UnRegisterChannelBroadcasterAsync(channelBroadcaster.Id);
                    //await CheckForEmptyBroadcastersAsync().ConfigureAwait(false);
                    await StopChannel(channelBroadcaster.Id);
                    clientConfiguration.Stop();
                    return null;
                }
            }

            channelBroadcaster.AddClientStreamer(clientConfiguration.UniqueRequestId, clientConfiguration);

            return channelBroadcaster;
        }

        public async Task<IChannelBroadcaster?> SetupChannelAsync(SMChannelDto smChannel)
        {
            // Implementation commented out for brevity
            return null;
        }

        private List<string> RunningUrls => _cacheManager.ChannelBroadcasters.Values
            .Where(a => a.ClientCount > 0 && a.SMStreamInfo != null)
            .Select(a => a.SMStreamInfo!.Url)
            .Distinct()
            .ToList();

        //public async Task CheckForEmptyBroadcastersAsync(CancellationToken cancellationToken = default)
        //{
        //    List<Task> tasks = [];

        //    foreach (IChannelBroadcaster? channelBroadcaster in _cacheManager.ChannelBroadcasters.Values.Where(a => a.SMStreamInfo != null))
        //    {
        //        if (channelBroadcaster.ClientChannelWriters.IsEmpty && !RunningUrls.Contains(channelBroadcaster.SMStreamInfo!.Url))
        //        {
        //            tasks.Add(StopChannelAsync(channelBroadcaster));
        //        }
        //    }

        //    await Task.WhenAll(tasks).ConfigureAwait(false);
        //}

        //public async Task StopChannelAsync(IChannelBroadcaster channelBroadcaster, bool force = false)
        //{
        //    if (channelBroadcaster.Shutdown)
        //    {
        //        return;
        //    }

        //    channelBroadcaster.Shutdown = true;

        //    //int delay = force ? 0 : _settings.CurrentValue.ShutDownDelay;
        //    //bool closed = delay > 0
        //    //    ? await UnRegisterChannelAfterDelayAsync(channelBroadcaster, TimeSpan.FromMilliseconds(delay), CancellationToken.None).ConfigureAwait(false)
        //    //    : await UnRegisterChannelAsync(channelBroadcaster).ConfigureAwait(false);
        //    await UnRegisterChannelAsync(channelBroadcaster).ConfigureAwait(false);

        //    //if (closed)
        //    //{
        //    //    //channelBroadcaster.Stop();
        //    //    //_sourceBroadcasterService.Stop(channelBroadcaster.Id.ToString());
        //    //    //foreach (IClientConfiguration clientConfiguration in channelBroadcaster.GetClientStreamerConfigurations())
        //    //    //{
        //    //    //    clientConfiguration.ClientStream?.Flush();
        //    //    //    clientConfiguration.ClientStream?.Dispose();
        //    //    //    await clientConfiguration.Response.CompleteAsync().ConfigureAwait(false);
        //    //    //}

        //    //    //List<ISourceBroadcaster> streamBroadcasters = _sourceBroadcasterService.GetStreamBroadcasters()
        //    //    //    .Where(cd => cd.ClientChannelWriters.Any(cc => cc.Key == channelBroadcaster.Id.ToString()))
        //    //    //    .ToList();

        //    //    //foreach (ISourceBroadcaster? sourceBroadcaster in streamBroadcasters)
        //    //    //{
        //    //    //    //if (_sourceBroadcasterService.RemoveChannelStreamer(sourceBroadcaster.Id))
        //    //    //    //{
        //    //    //    _sourceBroadcasterService.StopAndUnRegisterSourceBroadcaster(sourceBroadcaster.Id);
        //    //    //    //}
        //    //    //}

        //    //    //await CheckForEmptyBroadcastersAsync().ConfigureAwait(false);
        //    //}
        //}


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

            //if (!channelBroadcaster.SMStreamInfo.Id.StartsWith(IntroPlayListBuilder.IntroIDPrefix, StringComparison.InvariantCulture))
            //{
            //    _videoInfoService.SetSourceChannel(sourceChannelBroadcaster, channelBroadcaster.SMStreamInfo.Url, channelBroadcaster.SMStreamInfo.Name);
            //}

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
