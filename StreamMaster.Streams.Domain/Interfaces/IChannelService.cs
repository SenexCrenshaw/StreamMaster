namespace StreamMaster.Streams.Domain.Interfaces
{
    /// <summary>
    /// Provides methods for managing channels in StreamMaster, including operations for creating, retrieving, and modifying channel statuses.
    /// </summary>
    public interface IChannelService : IDisposable
    {
        /// <summary>
        /// Closes the specified channel asynchronously.
        /// </summary>
        /// <param name="channelId">The channel Id.</param>
        Task StopChannel(int channelId);

        /// <summary>
        /// Gets or creates a channel status asynchronously based on the specified client configuration.
        /// </summary>
        /// <param name="config">The client configuration.</param>
        /// <param name="streamGroupProfileId">The stream group profile ID.</param>
        /// <returns>The channel status if created; otherwise, <c>null</c>.</returns>
        Task<IChannelBroadcaster?> GetOrCreateChannelBroadcasterAsync(IClientConfiguration config, int streamGroupProfileId);

        /// <summary>
        /// Retrieves the client streamer configuration asynchronously by its unique request ID.
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
        /// Retrieves the channel status by its SM channel ID.
        /// </summary>
        /// <param name="smChannelId">The SM channel ID.</param>
        /// <returns>The channel status if found; otherwise, <c>null</c>.</returns>
        IChannelBroadcaster? GetChannelBroadcaster(int smChannelId);

        /// <summary>
        /// Retrieves the channel statuses associated with a specific stream URL.
        /// </summary>
        /// <param name="videoUrl">The video stream URL.</param>
        /// <returns>A list of channel statuses associated with the given URL.</returns>
        List<IChannelBroadcaster> GetChannelStatusFromStreamUrl(string videoUrl);

        /// <summary>
        /// Retrieves the channel status by its SM channel ID.
        /// </summary>
        /// <param name="smChannelId">The SM channel ID.</param>
        /// <returns>The channel status if found; otherwise, <c>null</c>.</returns>
        IChannelBroadcaster? GetChannelStatusFromSMChannelId(int smChannelId);

        /// <summary>
        /// Retrieves all channel statuses.
        /// </summary>
        /// <returns>A list of all channel statuses.</returns>
        List<IChannelBroadcaster> GetChannelStatuses();

        /// <summary>
        /// Retrieves the channel statuses associated with a specific SM stream ID.
        /// </summary>
        /// <param name="smStreamId">The SM stream ID.</param>
        /// <returns>A list of channel statuses associated with the given SM stream ID.</returns>
        List<IChannelBroadcaster> GetChannelStatusesFromSMStreamId(string smStreamId);

        /// <summary>
        /// Gets the total count of global streams.
        /// </summary>
        /// <returns>The total count of global streams.</returns>
        int GetGlobalStreamsCount();

        /// <summary>
        /// Checks whether a channel exists by its SM channel ID.
        /// </summary>
        /// <param name="smChannelId">The SM channel ID.</param>
        /// <returns><c>true</c> if the channel exists; otherwise, <c>false</c>.</returns>
        bool HasChannel(int smChannelId);

        ///// <summary>
        ///// Sets up a channel asynchronously based on the specified SM channel DTO.
        ///// </summary>
        ///// <param name="smChannel">The SM channel DTO.</param>
        ///// <returns>The channel status if set up successfully; otherwise, <c>null</c>.</returns>
        //Task<IChannelBroadcaster?> SetupChannelAsync(SMChannelDto smChannel);

        /// <summary>
        /// Switches the channel to the next video stream asynchronously.
        /// </summary>
        /// <param name="channelStatus">The status of the channel to switch.</param>
        /// <param name="overrideNextVideoStreamId">The ID of the next video stream to switch to, if overridden.</param>
        /// <returns><c>true</c> if the switch was successful; otherwise, <c>false</c>.</returns>
        Task<bool> SwitchChannelToNextStreamAsync(IChannelBroadcaster channelStatus, string? overrideNextVideoStreamId = null);

        /// <summary>
        /// Unregisters a client asynchronously by its unique request ID.
        /// </summary>
        /// <param name="uniqueRequestId">The unique request ID of the client.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns><c>true</c> if the client was unregistered successfully; otherwise, <c>false</c>.</returns>
        Task<bool> UnRegisterClientAsync(string uniqueRequestId, CancellationToken cancellationToken = default);
    }
}
