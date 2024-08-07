using StreamMaster.Domain.Events;

using StreamMaster.Streams.Domain.Events;

namespace StreamMaster.Streams.Domain.Interfaces
{
    /// <summary>
    /// Defines the methods and events for managing channel statuses in StreamMaster.
    /// </summary>
    public interface IChannelStatusService
    {
        /// <summary>
        /// Gets aggregated metrics for all channel distributors.
        /// </summary>
        /// <returns>A dictionary of aggregated metrics for all channel distributors.</returns>
        IDictionary<int, IStreamHandlerMetrics> GetMetrics();

        /// <summary>
        /// Occurs when a channel status is stopped.
        /// </summary>
        event AsyncEventHandler<ChannelStatusStopped>? OnChannelStatusStoppedEvent;

        /// <summary>
        /// Gets or creates a channel status channel asynchronously.
        /// </summary>
        /// <param name="config">The client configuration.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The channel status channel if created; otherwise, <c>null</c>.</returns>
        Task<IChannelStatus> GetOrCreateChannelStatusAsync(IClientConfiguration config, CancellationToken cancellationToken);

        /// <summary>
        /// Stops and unregisters a channel status by its integer key.
        /// </summary>
        /// <param name="key">The integer key of the channel status.</param>
        /// <returns><c>true</c> if the channel status was stopped and unregistered; otherwise, <c>false</c>.</returns>
        bool StopAndUnRegisterChannelStatus(int key);

        /// <summary>
        /// Gets all channel statuses.
        /// </summary>
        /// <returns>A dictionary of all channel statuses.</returns>
        //IDictionary<int, IChannelStatus> GetChannelStatuses();
        List<IChannelStatus> GetChannelStatuses();
    }
}
