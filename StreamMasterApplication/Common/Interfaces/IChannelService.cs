namespace StreamMasterApplication.Common.Interfaces;

/// <summary>
/// Provides methods for managing and querying channels.
/// </summary>
public interface IChannelService
{
    List<IClientStreamerConfiguration> GetClientStreamerConfigurationFromIds(List<Guid> clientIds);

    List<IChannelStatus> GetChannelStatusesFromVideoStreamId(string VideoStreamId);
    Task<IClientStreamerConfiguration?> GetClientStreamerConfiguration(Guid clientId, CancellationToken cancellationToken = default);

    //void UpdateChannelStatusVideoStreamId(string videoStreamId);
    /// <summary>
    /// Retrieves the statuses for all channels.
    /// </summary>
    /// <returns>A list of IChannelStatus objects representing the status of each channel.</returns>
    List<IChannelStatus> GetChannelStatuses();

    /// <summary>
    /// Retrieves the status of a specific channel by its video stream ID.
    /// </summary>
    /// <param name="channelVideoStreamId">The ID of the video stream associated with the channel.</param>
    /// <returns>An IChannelStatus object if the channel exists; otherwise, returns null.</returns>
    IChannelStatus? GetChannelStatus(string channelVideoStreamId);

    /// <summary>
    /// Gets the count of global streams.
    /// </summary>
    /// <returns>An integer representing the number of global streams.</returns>
    int GetGlobalStreamsCount();

    /// <summary>
    /// Checks if a channel with the specified video stream ID exists.
    /// </summary>
    /// <param name="channelVideoStreamId">The ID of the video stream to check.</param>
    /// <returns>True if the channel exists; otherwise, false.</returns>
    bool HasChannel(string channelVideoStreamId);

    /// <summary>
    /// Registers a new channel with the given video stream ID and name.
    /// </summary>    
    /// <param name="ChannelVideoStream">The video stream to associate with the channel.</param>
    /// <param name="ChannelName">The channel name</param>
    /// <returns>An IChannelStatus object representing the newly registered channel.</returns>
    IChannelStatus RegisterChannel(VideoStreamDto ChannelVideoStream, string ChannelName);

    /// <summary>
    /// Unregisters a channel by its video stream ID.
    /// </summary>
    /// <param name="channelVideoStreamId">The ID of the video stream associated with the channel to unregister.</param>
    void UnRegisterChannel(string channelVideoStreamId);
}