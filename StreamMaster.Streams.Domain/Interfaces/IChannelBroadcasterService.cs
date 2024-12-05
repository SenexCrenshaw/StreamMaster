using StreamMaster.Domain.Events;
using StreamMaster.Streams.Domain.Events;

namespace StreamMaster.Streams.Domain.Interfaces
{
    public interface IChannelBroadcasterService
    {
        /// <summary>
        /// Unregisters a client asynchronously by its unique request ID.
        /// </summary>
        /// <param name="uniqueRequestId">The unique request ID of the client.</param>
        /// <param name="cancellationToken">A Token to monitor for cancellation requests.</param>
        /// <returns><c>true</c> if the client was unregistered successfully; otherwise, <c>false</c>.</returns>
        Task<bool> UnRegisterClientAsync(string uniqueRequestId, CancellationToken cancellationToken = default);

        Task StopChannelAsync(IChannelBroadcaster channelBroadcaster);
        Task StopChannelAsync(int channelBroadcasterId);

        //Task<bool> UnRegisterChannelAfterDelayAsync(IChannelBroadcaster channelBroadcaster, TimeSpan delay, CancellationToken cancellationToken);
        //Task<bool> UnRegisterChannelAsync(int channelBroadcasterId);
        /// <summary>
        /// Gets or creates a channel status channel asynchronously.
        /// </summary>
        /// <param name="config">The client configuration.</param>
        /// <param name="cancellationToken">The cancellation Token.</param>
        /// <returns>The channel status channel if created; otherwise, <c>null</c>.</returns>
        Task<IChannelBroadcaster> GetOrCreateChannelBroadcasterAsync(IClientConfiguration config, int streamGroupProfileId, CancellationToken cancellationToken);

        /// <summary>
        /// Gets aggregated metrics for all channel distributors.
        /// </summary>
        /// <returns>A dictionary of aggregated metrics for all channel distributors.</returns>
        IDictionary<int, IStreamHandlerMetrics> GetMetrics();

        /// <summary>
        /// Gets all channel statuses.
        /// </summary>
        /// <returns>A list of all channel statuses.</returns>
        List<IChannelBroadcaster> GetChannelBroadcasters();

        /// <summary>
        /// Occurs when a channel status is stopped.
        /// </summary>
        event AsyncEventHandler<ChannelBroascasterStopped>? OnChannelBroadcasterStoppedEvent;
    }
}
