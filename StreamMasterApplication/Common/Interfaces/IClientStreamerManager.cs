namespace StreamMasterApplication.Common.Interfaces
{
    /// <summary>
    /// Manages client streamers and their configurations.
    /// </summary>
    public interface IClientStreamerManager
    {
        /// <summary>
        /// Moves client streamers from one stream handler to another.
        /// </summary>
        void MoveClientStreamers(IEnumerable<Guid> ClientIds, IStreamHandler newStreamHandler);

        /// <summary>
        /// Gets client streamer configurations by channel video stream ID.
        /// </summary>
        List<IClientStreamerConfiguration> GetClientStreamerConfigurationsByChannelVideoStreamId(string ChannelVideoStreamId);

        /// <summary>
        /// Asynchronously sets the client buffer delegate.
        /// </summary>
        Task SetClientBufferDelegate(Guid clientId, ICircularRingBuffer RingBuffer, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the count of clients for a specific channel video stream ID.
        /// </summary>
        int ClientCount(string ChannelVideoStreamId);

        /// <summary>
        /// Asynchronously cancels a client.
        /// </summary>
        Task<bool> CancelClient(Guid clientId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Disposes of the object, releasing all allocated resources.
        /// </summary>
        void Dispose();

        /// <summary>
        /// Asynchronously fails a client, triggering any necessary cleanup.
        /// </summary>
        Task FailClient(Guid clientId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously gets the configuration for a specific client.
        /// </summary>
        Task<IClientStreamerConfiguration?> GetClientStreamerConfiguration(Guid clientId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all client streamer configurations.
        /// </summary>
        ICollection<IClientStreamerConfiguration> GetClientStreamerConfigurations { get; }

        /// <summary>
        /// Gets client streamer configurations based on a list of client IDs.
        /// </summary>
        List<IClientStreamerConfiguration> GetClientStreamerConfigurationFromIds(List<Guid> clientIds);

        /// <summary>
        /// Checks if a client exists for a specific video stream ID.
        /// </summary>
        bool HasClient(string VideoStreamId, Guid ClientId);

        /// <summary>
        /// Registers a new client streamer configuration.
        /// </summary>
        void RegisterClient(IClientStreamerConfiguration clientStreamerConfiguration);

        /// <summary>
        /// Unregisters a client streamer by its ID.
        /// </summary>
        void UnRegisterClient(Guid clientId);
    }
}
