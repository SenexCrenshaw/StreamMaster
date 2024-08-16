using StreamMaster.Domain.Events;
using StreamMaster.Streams.Domain.Events;

using System.Collections.Concurrent;

namespace StreamMaster.Streams.Handlers
{
    public class SourceBroadcasterService(ILogger<SourceBroadcasterService> logger, ILogger<ISourceBroadcaster> sourceBroadcasterLogger, IProxyFactory proxyFactory)
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

        public async Task<ISourceBroadcaster?> GetOrCreateStreamBroadcasterAsync(SMStreamInfo smStreamInfo, CancellationToken cancellationToken)
        {
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

                sourceBroadcaster = new SourceBroadcaster(sourceBroadcasterLogger, smStreamInfo);

                logger.LogInformation("Created new source stream for: {Id} {name}", smStreamInfo.Id, smStreamInfo.Name);

                (Stream? stream, int? processId, ProxyStreamError? error) = await proxyFactory.GetProxy(smStreamInfo, cancellationToken).ConfigureAwait(false);
                if (stream == null || processId == null || error != null)
                {
                    logger.LogError("Could not create source stream for channel distributor: {Id} {name} {error}", smStreamInfo.Id, smStreamInfo.Name, error?.Message);
                    return null;
                }

                sourceBroadcaster.SetSourceStream(stream, smStreamInfo.Name, cancellationToken);
                sourceBroadcaster.OnStreamBroadcasterStoppedEvent += OnStoppedEvent;

                bool test = sourceBroadcasters.TryAdd(smStreamInfo.Url, sourceBroadcaster);

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
    }
}
