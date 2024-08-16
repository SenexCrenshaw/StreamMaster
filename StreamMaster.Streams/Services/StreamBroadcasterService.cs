using StreamMaster.Domain.Events;

using StreamMaster.Streams.Domain.Events;

using System.Collections.Concurrent;

namespace StreamMaster.Streams.Services
{
    public class StreamBroadcasterService : IStreamBroadcasterService
    {
        public event AsyncEventHandler<StreamBroadcasterStopped>? OnStreamBroadcasterStoppedEvent;

        private readonly ConcurrentDictionary<string, IStreamBroadcaster> streamBroadcasters = new();
        private readonly SemaphoreSlim GetOrCreateStreamDistributorSlim = new(1, 1);
        private readonly ILogger<StreamBroadcasterService> _logger;
        private readonly ILogger<IStreamBroadcaster> _channelDirectorLogger;
        private readonly IProxyFactory _proxyFactory;
        private readonly IVideoInfoService _videoInfoService;

        public StreamBroadcasterService(
            ILogger<StreamBroadcasterService> logger,
            ILogger<IStreamBroadcaster> channelDirectorLogger,
            IProxyFactory proxyFactory,
            IVideoInfoService videoInfoService)
        {
            _logger = logger;
            _channelDirectorLogger = channelDirectorLogger;
            _proxyFactory = proxyFactory;
            _videoInfoService = videoInfoService;
        }

        public IStreamBroadcaster? GetStreamBroadcaster(string? key)

        {
            return string.IsNullOrEmpty(key)
                ? null
                : !streamBroadcasters.TryGetValue(key, out IStreamBroadcaster? channelBroadcaster) ? null : channelBroadcaster;
        }

        public List<IStreamBroadcaster> GetStreamBroadcasters()
        {
            return [.. streamBroadcasters.Values];
        }

        public async Task<IStreamBroadcaster?> GetOrCreateStreamBroadcasterAsync(string sourceChannelName, SMStreamInfo smStreamInfo, CancellationToken cancellationToken)
        {
            await GetOrCreateStreamDistributorSlim.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                if (streamBroadcasters.TryGetValue(smStreamInfo.Url, out IStreamBroadcaster? channelDistributor))
                {
                    if (channelDistributor.IsFailed)
                    {
                        _ = StopAndUnRegister(smStreamInfo.Url);
                        _ = streamBroadcasters.TryGetValue(smStreamInfo.Url, out channelDistributor);
                    }

                    _logger.LogInformation("Reusing source channel distributor: {Id} {name}", smStreamInfo.Id, smStreamInfo.Name);
                    return channelDistributor;
                }

                channelDistributor = new StreamBroadcaster(_channelDirectorLogger, smStreamInfo);

                _logger.LogInformation("Created new source channel for: {Id} {name}", smStreamInfo.Id, smStreamInfo.Name);

                (Stream? stream, int? processId, ProxyStreamError? error) = await _proxyFactory.GetProxy(smStreamInfo, cancellationToken).ConfigureAwait(false);
                if (stream == null || processId == null || error != null)
                {
                    _logger.LogError("Could not create source stream for channel distributor: {Id} {name} {error}", smStreamInfo.Id, smStreamInfo.Name, error?.Message);
                    return null;
                }
                channelDistributor.SetSourceStream(stream, sourceChannelName, smStreamInfo.Name, cancellationToken);

                channelDistributor.OnStoppedEvent += OnDistributorStoppedEvent;
                bool test = streamBroadcasters.TryAdd(smStreamInfo.Url, channelDistributor);

                return channelDistributor;
            }
            finally
            {
                GetOrCreateStreamDistributorSlim.Release();
            }
        }

        public bool StopAndUnRegister(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return false;
            }

            if (streamBroadcasters.TryRemove(key, out IStreamBroadcaster? channelDistributor))
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

            foreach (KeyValuePair<string, IStreamBroadcaster> kvp in streamBroadcasters)
            {
                IStreamBroadcaster channelDistributor = kvp.Value;
                metrics[kvp.Key] = channelDistributor.Metrics;
            }

            return metrics;
        }

        private void OnDistributorStoppedEvent(object? sender, StreamBroadcasterStopped e)
        {
            OnStreamBroadcasterStoppedEvent?.Invoke(sender!, e);
            StopAndUnRegister(e.Id);
            _videoInfoService.RemoveSourceChannel(e.Name);
        }
    }
}
