namespace StreamMaster.Streams.Domain.Interfaces;

/// <summary>
/// Provides methods and properties to manage the status and configuration of a channel.
/// </summary>
public interface IChannelStatus
{
    //int ClientCount { get; set; }
    VideoOutputProfileDto VideoProfile { get; set; }
    string OverrideVideoStreamId { get; set; }

    /// <summary>
    /// Sets the channel to a global state.
    /// </summary>
    void SetIsGlobal();
    void SetCurrentSMStream(SMStreamDto? smStream);

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
    /// Gets or sets the ID of the SM Channel
    /// </summary>
    //int Id { get; set; }

    ///// <summary>
    ///// Gets or sets the name of the video stream associated with this channel.
    ///// </summary>
    //string CurrentVideoStreamName { get; set; }
    //string ChannelName { get; set; }

    /// <summary>
    /// Gets or sets the name of the video stream associated with this channel.
    /// </summary>
    SMStreamDto SMStream { get; }
    SMChannelDto SMChannel { get; }

}