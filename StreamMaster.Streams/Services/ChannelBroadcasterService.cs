using StreamMaster.Domain.Events;

using StreamMaster.Streams.Domain.Events;

using System.Collections.Concurrent;

namespace StreamMaster.Streams.Services
{
    public class ChannelBroadcasterService : IChannelBroadcasterService
    {
        public event AsyncEventHandler<ChannelDirectorStopped>? OnChannelDirectorStoppedEvent;

        private readonly ConcurrentDictionary<string, IChannelBroadcaster> channelBroadcasters = new();
        private readonly SemaphoreSlim GetOrCreateChannelDistributorSlim = new(1, 1);
        private readonly ILogger<ChannelBroadcasterService> _logger;
        private readonly ILogger<IChannelBroadcaster> _channelDirectorLogger;
        private readonly IProxyFactory _proxyFactory;
        private readonly IVideoInfoService _videoInfoService;

        public ChannelBroadcasterService(
            ILogger<ChannelBroadcasterService> logger,
            ILogger<IChannelBroadcaster> channelDirectorLogger,
            IProxyFactory proxyFactory,
            IVideoInfoService videoInfoService)
        {
            _logger = logger;
            _channelDirectorLogger = channelDirectorLogger;
            _proxyFactory = proxyFactory;
            _videoInfoService = videoInfoService;
        }

        public IChannelBroadcaster? GetChannelBroadcaster(string? key)

        {
            return string.IsNullOrEmpty(key)
                ? null
                : !channelBroadcasters.TryGetValue(key, out IChannelBroadcaster? channelBroadcaster) ? null : channelBroadcaster;
        }

        public List<IChannelBroadcaster> GetChannelBroadcasters()
        {
            return [.. channelBroadcasters.Values];
        }

        public async Task<IChannelBroadcaster?> GetOrCreateChannelDistributorAsync(string sourceChannelName, SMStreamInfo smStreamInfo, CancellationToken cancellationToken)
        {
            await GetOrCreateChannelDistributorSlim.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                if (channelBroadcasters.TryGetValue(smStreamInfo.Url, out IChannelBroadcaster? channelDistributor))
                {
                    if (channelDistributor.IsFailed)
                    {
                        _ = StopAndUnRegister(smStreamInfo.Url);
                        _ = channelBroadcasters.TryGetValue(smStreamInfo.Url, out channelDistributor);
                    }

                    _logger.LogInformation("Reusing source channel distributor: {Id} {name}", smStreamInfo.Id, smStreamInfo.Name);
                    return channelDistributor;
                }

                channelDistributor = new ChannelBroadcaster(_channelDirectorLogger, smStreamInfo);

                _logger.LogInformation("Created new source channel for: {Id} {name}", smStreamInfo.Id, smStreamInfo.Name);

                (Stream? stream, int? processId, ProxyStreamError? error) = await _proxyFactory.GetProxy(smStreamInfo, cancellationToken).ConfigureAwait(false);
                if (stream == null || processId == null || error != null)
                {
                    _logger.LogError("Could not create source stream for channel distributor: {Id} {name} {error}", smStreamInfo.Id, smStreamInfo.Name, error?.Message);
                    return null;
                }
                channelDistributor.SetSourceStream(stream, sourceChannelName, smStreamInfo.Name, cancellationToken);

                channelDistributor.OnStoppedEvent += OnDistributorStoppedEvent;
                bool test = channelBroadcasters.TryAdd(smStreamInfo.Url, channelDistributor);

                return channelDistributor;
            }
            finally
            {
                GetOrCreateChannelDistributorSlim.Release();
            }
        }

        public bool StopAndUnRegister(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return false;
            }

            if (channelBroadcasters.TryRemove(key, out IChannelBroadcaster? channelDistributor))
            {
                channelDistributor.Stop();
                return true;
            }

            //_logger.LogWarning("StopAndUnRegister channel distributor: {Id} does not exist!", key);
            return false;
        }

        public IDictionary<string, IStreamHandlerMetrics> GetMetrics()
        {
            Dictionary<string, IStreamHandlerMetrics> metrics = [];

            foreach (KeyValuePair<string, IChannelBroadcaster> kvp in channelBroadcasters)
            {
                IChannelBroadcaster channelDistributor = kvp.Value;
                metrics[kvp.Key] = channelDistributor.Metrics;
            }

            return metrics;
        }

        private void OnDistributorStoppedEvent(object? sender, ChannelDirectorStopped e)
        {
            OnChannelDirectorStoppedEvent?.Invoke(sender!, e);
            StopAndUnRegister(e.Id);
            _videoInfoService.RemoveSourceChannel(e.Name);
        }
    }
}
