using StreamMaster.Domain.Events;
using StreamMaster.Streams.Domain.Events;

using System.Collections.Concurrent;

namespace StreamMaster.Streams.Handlers
{
    /// <summary>
    /// Service for managing the status of channels, including creation, monitoring, and disposal.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="ChannelStatusService"/> class.
    /// </remarks>
    /// <param name="logger">Logger for this service.</param>
    /// <param name="channelStatusLogger">Logger for channel status.</param>
    public class ChannelBroadcasterService(ILogger<ChannelBroadcasterService> logger, ILogger<IChannelBroadcaster> channelStatusLogger)
        : IChannelBroadcasterService
    {
        /// <inheritdoc/>
        public event AsyncEventHandler<ChannelBroascasterStopped>? _OnChannelBroadcasterStoppedEvent;

        private readonly ConcurrentDictionary<int, IChannelBroadcaster> _sourceChannelBroadcasters = new();
        private readonly SemaphoreSlim _getOrCreateSourceChannelBroadcasterSlim = new(1, 1);

        /// <inheritdoc/>
        public IDictionary<int, IStreamHandlerMetrics> GetMetrics()
        {
            Dictionary<int, IStreamHandlerMetrics> metrics = [];

            foreach (KeyValuePair<int, IChannelBroadcaster> kvp in _sourceChannelBroadcasters)
            {
                IChannelBroadcaster channelBroadcaster = kvp.Value;
                metrics[kvp.Key] = channelBroadcaster.Metrics;
            }

            return metrics;
        }

        /// <inheritdoc/>
        public async Task<IChannelBroadcaster> GetOrCreateChannelBroadcasterAsync(IClientConfiguration config, int streamGroupProfileId, CancellationToken cancellationToken)
        {
            await _getOrCreateSourceChannelBroadcasterSlim.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                if (_sourceChannelBroadcasters.TryGetValue(config.SMChannel.Id, out IChannelBroadcaster? channelBroadcaster))
                {
                    if (channelBroadcaster.IsFailed)
                    {
                        StopAndUnRegisterChannelBroadcaster(config.SMChannel.Id);
                    }
                    else
                    {
                        logger.LogInformation("Reusing channel broadcaster: {Id} {Name}", config.SMChannel.Id, config.SMChannel.Name);
                        return channelBroadcaster;
                    }
                }

                channelBroadcaster = new ChannelBroadcaster(channelStatusLogger, config.SMChannel)
                {
                    SMChannel = config.SMChannel,
                    StreamGroupProfileId = streamGroupProfileId
                };

                logger.LogInformation("Created new channel for: {Id} {Name}", config.SMChannel.Id, config.SMChannel.Name);

                channelBroadcaster.OnChannelBroadcasterStoppedEvent += OnStoppedEvent;
                _sourceChannelBroadcasters.TryAdd(config.SMChannel.Id, channelBroadcaster);

                return channelBroadcaster;
            }
            finally
            {
                _getOrCreateSourceChannelBroadcasterSlim.Release();
            }
        }

        /// <inheritdoc/>
        public bool StopAndUnRegisterChannelBroadcaster(int key)
        {
            if (_sourceChannelBroadcasters.TryRemove(key, out IChannelBroadcaster? channelBroadcaster))
            {
                channelBroadcaster.Stop();
                return true;
            }

            return false;
        }

        /// <inheritdoc/>
        public List<IChannelBroadcaster> GetChannelBroadcasters()
        {
            return [.. _sourceChannelBroadcasters.Values];
        }

        /// <inheritdoc/>
        private void OnStoppedEvent(object? sender, ChannelBroascasterStopped e)
        {
            if (sender is IChannelBroadcaster channelBroadcaster)
            {
                channelBroadcaster.Shutdown = true;
                StopAndUnRegisterChannelBroadcaster(e.Id);
                _OnChannelBroadcasterStoppedEvent?.Invoke(sender, e);

            }
        }
    }
}
