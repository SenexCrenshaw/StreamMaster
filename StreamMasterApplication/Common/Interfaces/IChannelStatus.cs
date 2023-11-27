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
    string ChannelVideoStreamId { get; set; }

    ///// <summary>
    ///// Gets or sets the ID of the video stream associated with this channel.
    ///// </summary>
    //string CurrentVideoStreamId { get; set; }

    ///// <summary>
    ///// Gets or sets the name of the video stream associated with this channel.
    ///// </summary>
    //string CurrentVideoStreamName { get; set; }
    string ChannelName { get; set; }

    /// <summary>
    /// Gets or sets the name of the video stream associated with this channel.
    /// </summary>
    VideoStreamDto CurrentVideoStream { get; set; }
}