namespace StreamMaster.Streams.Domain.Interfaces;

/// <summary>
/// Provides methods for switching video streams in a channel.
/// </summary>
public interface IStreamSwitcher
{
    /// <summary>
    /// Asynchronously switches to the next video stream for a given channel status.
    /// Optionally, an override video stream ID can be provided.
    /// </summary>
    /// <param name="channelStatus">The current status of the channel.</param>
    /// <param name="overrideNextVideoStreamId">Optional ID to override the next video stream to switch to.</param>
    /// <returns>A Task returning true if the switch was successful; otherwise, returns false.</returns>
    Task<bool> SwitchToNextVideoStreamAsync(string ChannelVideoStreamId, string? overrideNextVideoStreamId = null);
}
