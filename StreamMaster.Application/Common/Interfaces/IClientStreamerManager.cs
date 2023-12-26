namespace StreamMaster.Application.Common.Interfaces
{
    /// <summary>
    /// Manages client streamers and their configurations.
    /// </summary>
    public interface IClientStreamerManager
    {
        Task AddClientToHandler(Guid clientId, IStreamHandler streamHandler);
        Task AddClientsToHandler(string ChannelVideoStreamId, IStreamHandler streamHandler);
        /// <summary>
        /// Gets client streamer configurations by channel video stream Url.
        /// </summary>
        List<IClientStreamerConfiguration> GetClientStreamerConfigurationsByChannelVideoStreamId(string ChannelVideoStreamId);

        /// <summary>
        /// Asynchronously sets the client buffer delegate.
        /// </summary>
        Task SetClientBufferDelegate(IClientStreamerConfiguration clientStreamerConfiguration, ICircularRingBuffer RingBuffer);

        /// <summary>
        /// Gets the count of clients for a specific channel video stream Url.
        /// </summary>
        int ClientCount(string ChannelVideoStreamUrl);

        /// <summary>
        /// Asynchronously cancels a client.
        /// </summary>
        Task<bool> CancelClient(Guid clientId, bool includeAbort);

        /// <summary>
        /// Disposes of the object, releasing all allocated resources.
        /// </summary>
        void Dispose();
        /// <summary>
        /// Asynchronously gets the configuration for a specific client.
        /// </summary>
        Task<IClientStreamerConfiguration?> GetClientStreamerConfiguration(Guid clientId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all client streamer configurations.
        /// </summary>
        ICollection<IClientStreamerConfiguration> GetAllClientStreamerConfigurations { get; }

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
        Task UnRegisterClient(Guid clientId);
    }
}
