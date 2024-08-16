using StreamMaster.Domain.Events;
using StreamMaster.Streams.Domain.Events;

namespace StreamMaster.Streams.Domain.Interfaces
{
    public interface IChannelBroadcasterService
    {
        /// <summary>
        /// Gets or creates a channel status channel asynchronously.
        /// </summary>
        /// <param name="config">The client configuration.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
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
        event AsyncEventHandler<ChannelBroascasterStopped>? _OnChannelBroadcasterStoppedEvent;

        /// <summary>
        /// Stops and unregisters a channel status by its integer key.
        /// </summary>
        /// <param name="key">The integer key of the channel status.</param>
        /// <returns><c>true</c> if the channel status was stopped and unregistered; otherwise, <c>false</c>.</returns>
        bool StopAndUnRegisterChannelBroadcaster(int key);
    }
}
