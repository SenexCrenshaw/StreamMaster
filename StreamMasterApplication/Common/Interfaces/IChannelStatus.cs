namespace StreamMasterApplication.Common.Interfaces;

/// <summary>
/// Provides methods and properties to manage the status and configuration of a channel.
/// </summary>
public interface IChannelStatus
{
    //List<Guid> GetChannelClientIds { get; }
    /// <summary>
    /// Sets the channel to a global state.
    /// </summary>
    void SetIsGlobal();

    /// <summary>
    /// Unregisters a client from the channel based on the client's unique identifier.
    /// </summary>
    /// <param name="clientId">The unique identifier of the client to be unregistered.</param>
    /// <exception cref="KeyNotFoundException">Thrown when the client with the provided clientId does not exist.</exception>
    //void UnRegisterClient(Guid clientId);

    /// <summary>
    /// Registers a new client with the given configuration.
    /// </summary>
    /// <param name="clientStreamerConfiguration">The configuration settings for the client streamer.</param>
    /// <exception cref="ArgumentNullException">Thrown when clientStreamerConfiguration is null.</exception>
    /// <exception cref="ArgumentException">Thrown when a client with the same ID already exists.</exception>
    //void RegisterClient(Guid clientId);

    /// <summary>
    /// Gets the list of client streamer configurations in the channel.
    /// </summary>
    //List<ClientStreamerConfiguration> GetChannelClientClientStreamerConfigurations { get; }

    ///// <summary>
    ///// Gets the current number of registered clients in the channel.
    ///// </summary>
    //int ClientCount { get; }

    /// <summary>
    /// Indicates whether a failover operation is currently in progress.
    /// </summary>
    bool FailoverInProgress { get; set; }

    /// <summary>
    /// Indicates whether the channel is in a global state.
    /// </summary>
    bool IsGlobal { get; set; }

    /// <summary>
    /// Gets or sets the rank of the channel.
    /// </summary>
    int Rank { get; set; }

    /// <summary>
    /// Gets or sets the ID of the parent video stream, if applicable.
    /// </summary>
    string ParentVideoStreamId { get; set; }

    /// <summary>
    /// Gets or sets the ID of the video stream associated with this channel.
    /// </summary>
    string VideoStreamId { get; set; }

    /// <summary>
    /// Gets or sets the name of the video stream associated with this channel.
    /// </summary>
    string VideoStreamName { get; set; }
}