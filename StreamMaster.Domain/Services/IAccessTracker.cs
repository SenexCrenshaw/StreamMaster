namespace StreamMaster.Domain.Services;

public interface IAccessTracker
{
    /// <summary>
    /// Updates the last access time for a given video stream. If an inactive threshold is specified, it updates that as well.
    /// </summary>
    /// <param name="videoStreamId">The ID of the video stream.</param>
    /// <param name="inactiveThreshold">Optional. The inactivity threshold after which the stream should be considered inactive.</param>
    void UpdateAccessTime(int smChannelId, TimeSpan? inactiveThreshold = null);

    /// <summary>
    /// Retrieves the IDs of video streams that have been inactive longer than their specified inactivity threshold.
    /// </summary>
    /// <returns>A collection of video stream IDs that are considered inactive.</returns>
    IEnumerable<int> GetInactiveStreams();

    /// <summary>
    /// Removes a video stream from tracking.
    /// </summary>
    /// <param name="videoStreamId">The ID of the video stream to remove.</param>
    void Remove(int smChannelId);

    /// <summary>
    /// Gets or sets the global check interval for monitoring streams. This determines how frequently the system checks for inactive streams.
    /// </summary>
    TimeSpan CheckInterval { get; set; }
}
