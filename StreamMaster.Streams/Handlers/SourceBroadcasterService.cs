using StreamMaster.Domain.Events;
using StreamMaster.Streams.Domain.Events;

using System.Collections.Concurrent;

namespace StreamMaster.Streams.Handlers
{
    public class SourceBroadcasterService(ILogger<SourceBroadcasterService> logger, IVideoInfoService _videoInfoService, ILogger<ISourceBroadcaster> sourceBroadcasterLogger, IOptionsMonitor<Setting> _settings, IStreamFactory proxyFactory)
        : ISourceBroadcasterService
    {
        public event AsyncEventHandler<StreamBroadcasterStopped>? OnStreamBroadcasterStoppedEvent;

        private readonly ConcurrentDictionary<string, ISourceBroadcaster> sourceBroadcasters = new();
        private readonly SemaphoreSlim GetOrCreateStreamDistributorSlim = new(1, 1);

        public ISourceBroadcaster? GetStreamBroadcaster(string? key)
        {
            return string.IsNullOrEmpty(key)
                ? null
                : !sourceBroadcasters.TryGetValue(key, out ISourceBroadcaster? channelBroadcaster) ? null : channelBroadcaster;
        }

        public List<ISourceBroadcaster> GetStreamBroadcasters()
        {
            return [.. sourceBroadcasters.Values];
        }

        public async Task<ISourceBroadcaster?> GetOrCreateStreamBroadcasterAsync(IChannelBroadcaster channelBroadcaster, CancellationToken cancellationToken)
        {
            if (channelBroadcaster.SMStreamInfo == null)
            {
                return null;
            }

            SMStreamInfo smStreamInfo = channelBroadcaster.SMStreamInfo;

            await GetOrCreateStreamDistributorSlim.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                if (sourceBroadcasters.TryGetValue(smStreamInfo.Url, out ISourceBroadcaster? sourceBroadcaster))
                {
                    if (sourceBroadcaster.IsFailed)
                    {
                        _ = StopAndUnRegisterSourceBroadcaster(smStreamInfo.Url);
                        _ = sourceBroadcasters.TryGetValue(smStreamInfo.Url, out sourceBroadcaster);
                    }

                    logger.LogInformation("Reusing source stream: {Id} {name}", smStreamInfo.Id, smStreamInfo.Name);
                    return sourceBroadcaster;
                }

                sourceBroadcaster = new SourceBroadcaster(sourceBroadcasterLogger, smStreamInfo, _settings);

                logger.LogInformation("Created new source stream for: {Id} {name}", smStreamInfo.Id, smStreamInfo.Name);

                (Stream? stream, int? processId, ProxyStreamError? error) = await proxyFactory.GetStream(channelBroadcaster, cancellationToken).ConfigureAwait(false);
                if (stream == null || processId == null || error != null)
                {
                    logger.LogError("Could not create source stream for channel distributor: {Id} {name} {error}", smStreamInfo.Id, smStreamInfo.Name, error?.Message);
                    return null;
                }

                sourceBroadcaster.SetSourceStream(stream, smStreamInfo.Name, cancellationToken);
                sourceBroadcaster.OnStreamBroadcasterStoppedEvent += OnStoppedEvent;

                if (sourceBroadcasters.TryAdd(smStreamInfo.Url, sourceBroadcaster))
                {
                    if (!smStreamInfo.Id.StartsWith(IntroPlayListBuilder.IntroIDPrefix, StringComparison.InvariantCulture))
                    {
                        _videoInfoService.SetSourceChannel(sourceBroadcaster, smStreamInfo.Id, smStreamInfo.Name);
                    }
                }

                return sourceBroadcaster;
            }
            finally
            {
                GetOrCreateStreamDistributorSlim.Release();
            }
        }

        public bool StopAndUnRegisterSourceBroadcaster(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return false;
            }

            if (sourceBroadcasters.TryRemove(key, out ISourceBroadcaster? sourceBroadcaster))
            {
                sourceBroadcaster.Stop();
                _videoInfoService.StopVideoPlugin(sourceBroadcaster.SMStreamInfo.Id);
                return true;
            }

            return false;
        }

        public IDictionary<string, IStreamHandlerMetrics> GetMetrics()
        {
            Dictionary<string, IStreamHandlerMetrics> metrics = [];

            foreach (KeyValuePair<string, ISourceBroadcaster> kvp in sourceBroadcasters)
            {
                ISourceBroadcaster channelDistributor = kvp.Value;
                metrics[kvp.Key] = channelDistributor.Metrics;
            }

            return metrics;
        }

        private void OnStoppedEvent(object? sender, StreamBroadcasterStopped e)
        {
            if (sender is ISourceBroadcaster channelBroadcaster)
            {
                StopAndUnRegisterSourceBroadcaster(e.Id);
                OnStreamBroadcasterStoppedEvent?.Invoke(sender!, e);
            }
        }

        private async Task CheckForEmptyBroadcastersAsync(CancellationToken cancellationToken = default)
        {
            foreach (ISourceBroadcaster? sourceBroadcaster in sourceBroadcasters.Values)
            {
                int count = sourceBroadcaster.ClientChannels.Count(a => a.Key != "VideoInfo");
                if (count == 0)
                {
                    int delay = _settings.CurrentValue.ShutDownDelay;
                    if (delay > 0)
                    {
                        await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
                    }
                    count = sourceBroadcaster.ClientChannels.Count(a => a.Key != "VideoInfo");
                    if (count != 0)
                    {
                        return;
                    }

                    sourceBroadcaster.Stop();
                    // StopAndUnRegisterSourceBroadcaster(sourceBroadcaster.Id);
                }
            }
        }

        public async Task UnRegisterChannelBroadcasterAsync(int channelBroadcasterId)
        {
            ISourceBroadcaster? sourceBroadcaster = sourceBroadcasters.Values.FirstOrDefault(broadcaster => broadcaster.ClientChannels.ContainsKey(channelBroadcasterId.ToString()));
            if (sourceBroadcaster == null)
            {
                return;
            }

            if (sourceBroadcaster.ClientChannels.TryRemove(channelBroadcasterId.ToString(), out _))
            {
                await CheckForEmptyBroadcastersAsync();
            }
        }
    }
}
