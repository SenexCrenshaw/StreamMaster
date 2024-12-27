using StreamMaster.Domain.Events;
using StreamMaster.Domain.Models;
using StreamMaster.Streams.Domain.Events;
using StreamMaster.Streams.Domain.Metrics;

namespace StreamMaster.Streams.Domain.Interfaces;

/// <summary>
/// Defines the methods and events for managing source broadcasters in StreamMaster.
/// </summary>
public interface ISourceBroadcasterService
{
    StreamConnectionMetricData? GetStreamConnectionMetricData(string key);
    List<StreamConnectionMetricData> GetStreamConnectionMetrics();

    /// <summary>
    /// Occurs when a source broadcaster is stopped.
    /// </summary>
    event AsyncEventHandler<StreamBroadcasterStopped>? OnStreamBroadcasterStopped;

    /// <summary>
    /// Gets a source broadcaster by its string key.
    /// </summary>
    /// <param name="key">The string key of the source broadcaster.</param>
    /// <returns>The source broadcaster if found; otherwise, <c>null</c>.</returns>
    ISourceBroadcaster? GetStreamBroadcaster(string? key);

    /// <summary>
    /// Gets or creates a source broadcaster asynchronously.
    /// </summary>
    /// <param name="smStreamInfo">The channel broadcaster associated with the source broadcaster.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The source broadcaster if created or found; otherwise, <c>null</c>.</returns>
    Task<ISourceBroadcaster?> GetOrCreateStreamBroadcasterAsync(SMStreamInfo smStreamInfo, CancellationToken cancellationToken);

    Task<ISourceBroadcaster?> GetOrCreateMultiViewStreamBroadcasterAsync(IChannelBroadcaster channelBroadcaster, CancellationToken cancellationToken);

    /// <summary>
    /// Stops and unregisters a source broadcaster by its string key.
    /// </summary>
    /// <param name="key">The string key of the source broadcaster.</param>
    /// <returns>A task representing the asynchronous operation. Returns <c>true</c> if the source broadcaster was stopped and unregistered; otherwise, <c>false</c>.</returns>
    Task<bool> StopAndUnRegisterSourceBroadCasterAsync(string key);

    /// <summary>
    /// Gets all source broadcasters.
    /// </summary>
    /// <returns>A list of all source broadcasters.</returns>
    List<ISourceBroadcaster> GetStreamBroadcasters();

    /// <summary>
    /// Unregisters a channel broadcaster by its unique identifier.
    /// </summary>
    /// <param name="channelBroadcasterId">The unique identifier of the channel broadcaster to unregister.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UnRegisterChannelBroadcasterAsync(int channelBroadcasterId);
}
