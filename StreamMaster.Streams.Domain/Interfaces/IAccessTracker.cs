using StreamMaster.Domain.Models;

namespace StreamMaster.Streams.Domain.Interfaces;

public interface IAccessTracker
{
    /// <summary>
    /// Updates the last access time for a given video stream. If an inactive threshold is specified, it updates that as well.
    /// </summary>
    /// <param name="key">The tracking key.</param>
    /// <param name="smStreamId">The ID of the stream.</param>
    /// <param name="inactiveThreshold">Optional. The inactivity threshold after which the stream should be considered inactive.</param>
    StreamAccessInfo UpdateAccessTime(string key, string smStreamId, TimeSpan? inactiveThreshold = null);

    /// <summary>
    /// Retrieves the IDs of streams that have been inactive longer than their specified inactivity threshold.
    /// </summary>
    /// <returns>A collection of stream IDs that are considered inactive.</returns>
    IEnumerable<StreamAccessInfo> GetInactiveStreams();

    /// <summary>
    /// Removes a video stream from tracking.
    /// </summary>
    /// <param name="key">The tracking key.</param>
    void RemoveAccessTime(string key);

    /// <summary>
    /// Gets or sets the global check interval for monitoring streams. This determines how frequently the system checks for inactive streams.
    /// </summary>
    TimeSpan CheckInterval { get; set; }
}
