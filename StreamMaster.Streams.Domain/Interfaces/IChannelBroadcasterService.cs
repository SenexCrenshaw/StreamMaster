namespace StreamMaster.Streams.Domain.Interfaces;

/// <summary>
/// Defines methods and events for managing channel broadcasters and their clients.
/// </summary>
public interface IChannelBroadcasterService : IDisposable
{

    /// <summary>
    /// Gets all channel broadcasters currently managed by the service.
    /// </summary>
    /// <returns>A list of all channel broadcasters.</returns>
    List<IChannelBroadcaster> GetChannelBroadcasters();

    /// <summary>
    /// Gets or creates a channel broadcaster asynchronously.
    /// </summary>
    /// <param name="config">The client configuration for the channel broadcaster.</param>
    /// <param name="streamGroupProfileId">The ID of the stream group profile associated with the channel.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The created or existing channel broadcaster.</returns>
    Task<IChannelBroadcaster> GetOrCreateChannelBroadcasterAsync(IClientConfiguration config, int streamGroupProfileId, CancellationToken cancellationToken);

    /// <summary>
    /// Stops a channel broadcaster asynchronously.
    /// </summary>
    /// <param name="channelBroadcaster">The channel broadcaster to stop.</param>
    Task StopChannelBroadcasterAsync(IChannelBroadcaster channelBroadcaster);

    /// <summary>
    /// Stops a channel broadcaster asynchronously by its ID.
    /// </summary>
    /// <param name="channelBroadcasterId">The unique identifier of the channel broadcaster to stop.</param>
    Task StopChannelBroadcasterAsync(int channelBroadcasterId);

    /// <summary>
    /// Unregisters a client asynchronously by its unique request ID.
    /// </summary>
    /// <param name="uniqueRequestId">The unique request ID of the client.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UnRegisterClientAsync(string uniqueRequestId, CancellationToken cancellationToken = default);
}
