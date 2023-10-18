using StreamMasterApplication.Common.Models;

namespace StreamMasterApplication.Common.Interfaces;

/// <summary>
/// Provides methods for managing and querying channels.
/// </summary>
public interface IChannelService
{
    List<ClientStreamerConfiguration> GetClientStreamerConfigurationFromIds(List<Guid> clientIds);
    ClientStreamerConfiguration? GetClientStreamerConfiguration(Guid clientId);
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
    /// <param name="channelVideoStreamId">The ID of the video stream to associate with the channel.</param>
    /// <param name="videoStreamName">The name of the video stream to associate with the channel.</param>
    /// <returns>An IChannelStatus object representing the newly registered channel.</returns>
    IChannelStatus RegisterChannel(string channelVideoStreamId, string videoStreamName);

    /// <summary>
    /// Unregisters a channel by its video stream ID.
    /// </summary>
    /// <param name="channelVideoStreamId">The ID of the video stream associated with the channel to unregister.</param>
    void UnregisterChannel(string channelVideoStreamId);
}