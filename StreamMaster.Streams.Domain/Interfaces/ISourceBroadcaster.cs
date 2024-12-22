using System.Collections.Concurrent;

using StreamMaster.Domain.Models;
using StreamMaster.Streams.Domain.Events;
using StreamMaster.Streams.Domain.Statistics;

namespace StreamMaster.Streams.Domain.Interfaces;

/// <summary>
/// Represents a broadcaster that manages source streams and associated channel broadcasters.
/// </summary>
public interface ISourceBroadcaster
{
    /// <summary>
    /// Gets the metrics associated with the source stream.
    /// </summary>
    StreamHandlerMetrics? Metrics { get; }

    /// <summary>
    /// Gets the collection of channel broadcasters associated with this source broadcaster.
    /// </summary>
    ConcurrentDictionary<string, IStreamDataToClients> ChannelBroadcasters { get; }

    /// <summary>
    /// Adds a channel broadcaster to this source broadcaster.
    /// </summary>
    /// <param name="channelBroadcaster">The channel broadcaster to add.</param>
    void AddChannelBroadcaster(IChannelBroadcaster channelBroadcaster);

    /// <summary>
    /// Adds a channel broadcaster to this source broadcaster by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the channel broadcaster.</param>
    /// <param name="channelBroadcaster">The channel broadcaster to add.</param>
    void AddChannelBroadcaster(string id, IStreamDataToClients channelBroadcaster);

    /// <summary>
    /// Removes a channel broadcaster from this source broadcaster.
    /// </summary>
    /// <param name="channelBroadcasterId">The unique identifier of the channel broadcaster to remove.</param>
    /// <returns><c>true</c> if the channel broadcaster was removed; otherwise, <c>false</c>.</returns>
    bool RemoveChannelBroadcaster(int channelBroadcasterId);

    /// <summary>
    /// Removes a channel broadcaster from this source broadcaster.
    /// </summary>
    /// <param name="Id">The unique identifier of the channel broadcaster to remove.</param>
    /// <returns><c>true</c> if the channel broadcaster was removed; otherwise, <c>false</c>.</returns>
    bool RemoveChannelBroadcaster(string Id);

    /// <summary>
    /// Stops the source broadcaster and releases associated resources.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="Exception">Thrown if an error occurs during the stop process.</exception>
    Task StopAsync();

    /// <summary>
    /// Gets a value indicating whether the source broadcaster has failed.
    /// </summary>
    bool IsFailed { get; }

    /// <summary>
    /// Sets the source stream for a specific channel broadcaster.
    /// </summary>
    /// <param name="smStreamInfo">The channel broadcaster for which to set the source stream.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task SetSourceStreamAsync(SMStreamInfo smStreamInfo, CancellationToken cancellationToken);
    Task SetSourceMultiViewStreamAsync(IChannelBroadcaster channelBroadcaster, CancellationToken cancellationToken);
    /// <summary>
    /// Gets the unique identifier for this source broadcaster.
    /// </summary>
    string Id { get; }

    /// <summary>
    /// Occurs when the source broadcaster stops streaming.
    /// </summary>
    event EventHandler<StreamBroadcasterStopped> StreamBroadcasterStopped;
}
