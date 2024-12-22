namespace StreamMaster.Streams.Domain.Interfaces;

/// <summary>
/// Provides methods for managing channels in StreamMaster, including operations for creating, retrieving, and modifying channel statuses.
/// </summary>
public interface IChannelService : IDisposable
{
    /// <summary>
    /// Advances the specified channel to its next stream asynchronously.
    /// </summary>
    /// <param name="smChannelId">The SM channel ID.</param>
    Task MoveToNextStreamAsync(int smChannelId);

    /// <summary>
    /// Cancels all active channels asynchronously.
    /// </summary>
    Task CancelAllChannelsAsync();

    /// <summary>
    /// Changes the video stream channel for the specified stream IDs asynchronously.
    /// </summary>
    /// <param name="playingSMStreamId">The ID of the currently playing stream.</param>
    /// <param name="newSMStreamId">The ID of the new stream to switch to.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    Task ChangeVideoStreamChannelAsync(string playingSMStreamId, string newSMStreamId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a client to the specified channel asynchronously.
    /// </summary>
    /// <param name="clientConfiguration">The client configuration.</param>
    /// <param name="streamGroupProfileId">The stream group profile ID.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns><c>true</c> if the client was added successfully; otherwise, <c>false</c>.</returns>
    Task<bool> AddClientToChannelAsync(IClientConfiguration clientConfiguration, int streamGroupProfileId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Stops the specified channel asynchronously.
    /// </summary>
    /// <param name="channelId">The ID of the channel to stop.</param>
    Task StopChannelAsync(int channelId);

    /// <summary>
    /// Gets or creates a channel broadcaster asynchronously based on the specified client configuration.
    /// </summary>
    /// <param name="config">The client configuration.</param>
    /// <param name="streamGroupProfileId">The stream group profile ID.</param>
    /// <returns>The channel broadcaster if created; otherwise, <c>null</c>.</returns>
    Task<IChannelBroadcaster?> GetOrCreateChannelBroadcasterAsync(IClientConfiguration config, int streamGroupProfileId);

    /// <summary>
    /// Retrieves the client streamer configuration by its unique request ID.
    /// </summary>
    /// <param name="uniqueRequestId">The unique request ID of the client.</param>
    /// <returns>The client configuration if found; otherwise, <c>null</c>.</returns>
    IClientConfiguration? GetClientStreamerConfiguration(string uniqueRequestId);

    /// <summary>
    /// Retrieves all client streamer configurations.
    /// </summary>
    /// <returns>A list of all client streamer configurations.</returns>
    List<IClientConfiguration> GetClientStreamerConfigurations();

    /// <summary>
    /// Retrieves the channel broadcaster by its SM channel ID.
    /// </summary>
    /// <param name="smChannelId">The SM channel ID.</param>
    /// <returns>The channel broadcaster if found; otherwise, <c>null</c>.</returns>
    IChannelBroadcaster? GetChannelBroadcaster(int smChannelId);

    /// <summary>
    /// Retrieves the channel broadcasters associated with a specific video stream URL.
    /// </summary>
    /// <param name="videoUrl">The video stream URL.</param>
    /// <returns>A list of channel broadcasters associated with the given URL.</returns>
    List<IChannelBroadcaster> GetChannelStatusFromStreamUrl(string videoUrl);

    /// <summary>
    /// Retrieves the channel broadcaster by its SM channel ID.
    /// </summary>
    /// <param name="smChannelId">The SM channel ID.</param>
    /// <returns>The channel broadcaster if found; otherwise, <c>null</c>.</returns>
    IChannelBroadcaster? GetChannelStatusFromSMChannelId(int smChannelId);

    /// <summary>
    /// Retrieves all channel broadcasters.
    /// </summary>
    /// <returns>A list of all channel broadcasters.</returns>
    List<IChannelBroadcaster> GetChannelStatuses();

    /// <summary>
    /// Retrieves the channel broadcasters associated with a specific SM stream ID.
    /// </summary>
    /// <param name="smStreamId">The SM stream ID.</param>
    /// <returns>A list of channel broadcasters associated with the given SM stream ID.</returns>
    List<IChannelBroadcaster> GetChannelStatusesFromSMStreamId(string smStreamId);

    /// <summary>
    /// Gets the total count of global streams.
    /// </summary>
    /// <returns>The total count of global streams.</returns>
    int GetGlobalStreamsCount();

    /// <summary>
    /// Checks whether a channel with the specified SM channel ID exists.
    /// </summary>
    /// <param name="smChannelId">The SM channel ID.</param>
    /// <returns><c>true</c> if the channel exists; otherwise, <c>false</c>.</returns>
    bool HasChannel(int smChannelId);

    /// <summary>
    /// Switches the specified channel broadcaster to the next stream asynchronously.
    /// </summary>
    /// <param name="channelBroadcaster">The channel broadcaster to switch.</param>
    /// <param name="clientConfiguration">The client configuration, if applicable.</param>
    /// <param name="overrideSMStreamId">An optional override SM stream ID.</param>
    /// <returns><c>true</c> if the switch was successful; otherwise, <c>false</c>.</returns>
    Task<bool> SwitchChannelToNextStreamAsync(IChannelBroadcaster channelBroadcaster, IClientConfiguration? clientConfiguration, string? overrideSMStreamId = null);

    /// <summary>
    /// Unregisters a client asynchronously by its unique request ID.
    /// </summary>
    /// <param name="uniqueRequestId">The unique request ID of the client.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns><c>true</c> if the client was unregistered successfully; otherwise, <c>false</c>.</returns>
    Task UnRegisterClientAsync(string uniqueRequestId, CancellationToken cancellationToken = default);
}
