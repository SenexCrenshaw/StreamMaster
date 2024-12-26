using System.Collections.Concurrent;

using StreamMaster.Domain.Events;
using StreamMaster.Domain.Models;
using StreamMaster.Streams.Domain.Events;
using StreamMaster.Streams.Domain.Metrics;

namespace StreamMaster.Streams.Domain.Interfaces;

/// <summary>
/// Represents a broadcaster that manages source streams and associated channel broadcasters.
/// </summary>
public interface ISourceBroadcaster
{
    bool IsStopped { get; }
    bool IsMultiView { get; set; }
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
    /// <param name="id">The unique identifier of the broadcaster.</param>
    /// <param name="streamDataToClients">The broadcaster to add.</param>
    void AddChannelBroadcaster(string id, IStreamDataToClients streamDataToClients);

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
    Task<long> SetSourceStreamAsync(SMStreamInfo smStreamInfo, CancellationToken cancellationToken);
    Task<long> SetSourceMultiViewStreamAsync(IChannelBroadcaster channelBroadcaster, CancellationToken cancellationToken);
    /// <summary>
    /// Gets the unique identifier for this source broadcaster.
    /// </summary>
    string Id { get; }

    /// <summary>
    /// Occurs when the source broadcaster stops streaming.
    /// </summary>
    event AsyncEventHandler<StreamBroadcasterStopped> OnStreamBroadcasterStopped;

    public SMStreamInfo SMStreamInfo { get; }
}
