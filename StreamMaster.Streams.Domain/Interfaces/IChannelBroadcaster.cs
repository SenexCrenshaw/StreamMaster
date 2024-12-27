
using System.Collections.Concurrent;

namespace StreamMaster.Streams.Domain.Interfaces;
/// <summary>
/// Provides methods and properties to manage the status and configuration of a channel broadcaster.
/// </summary>
public interface IChannelBroadcaster : IStreamStatus, IStreamDataToClients
{
    /// <summary>
    /// Stops the channel broadcaster.
    /// </summary>
    void Stop();

    /// <summary>
    /// Gets a value indicating whether the client configurations are empty.
    /// </summary>
    bool ClientConfigurationsEmpty { get; }

    /// <summary>
    /// Removes a client configuration from the broadcaster.
    /// </summary>
    /// <param name="clientId">The unique identifier of the client to remove.</param>
    /// <returns>True if the client was successfully removed; otherwise, false.</returns>
    bool RemoveClient(string clientId);

    /// <summary>
    /// Gets the collection of client configurations for the channel.
    /// </summary>
    ConcurrentDictionary<string, IClientConfiguration> Clients { get; }

    /// <summary>
    /// Adds a client configuration to the channel broadcaster.
    /// </summary>
    /// <param name="clientConfiguration">The client configuration to add.</param>
    void AddChannelStreamer(IClientConfiguration clientConfiguration);

    /// <summary>
    /// Gets the unique identifier for the channel.
    /// </summary>
    int Id { get; }

    ///// <summary>
    ///// Occurs when the channel broadcaster is stopped.
    ///// </summary>
    //event EventHandler<ChannelBroadcasterStopped>? OnChannelBroadcasterStoppedEvent;

    /// <summary>
    /// Marks the channel broadcaster as global.
    /// </summary>
    void SetIsGlobal();

    /// <summary>
    /// Gets or sets a value indicating whether the broadcaster is global.
    /// </summary>
    bool IsGlobal { get; set; }

    /// <summary>
    /// Gets a value indicating whether the broadcaster has failed.
    /// </summary>
    bool IsFailed { get; }
}
