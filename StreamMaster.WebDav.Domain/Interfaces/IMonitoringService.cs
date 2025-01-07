namespace StreamMaster.WebDav.Domain.Interfaces;

/// <summary>
/// Interface for tracking performance metrics and operational logs.
/// </summary>
public interface IMonitoringService
{
    /// <summary>
    /// Tracks the duration and result of an API call.
    /// </summary>
    /// <param name="endpoint">The API endpoint being called.</param>
    /// <param name="duration">The duration of the call.</param>
    /// <param name="success">Indicates if the call was successful.</param>
    void TrackApiCall(string endpoint, TimeSpan duration, bool success);

    /// <summary>
    /// Tracks a cache hit for a file.
    /// </summary>
    /// <param name="path">The path of the file that was found in the cache.</param>
    void TrackCacheHit(string path);

    /// <summary>
    /// Tracks a cache miss for a file.
    /// </summary>
    /// <param name="path">The path of the file that was not found in the cache.</param>
    void TrackCacheMiss(string path);

    /// <summary>
    /// Tracks a file read operation.
    /// </summary>
    /// <param name="path">The path of the file being read.</param>
    /// <param name="isVirtual">Indicates if the file is virtual or cached.</param>
    void TrackFileRead(string path, bool isVirtual);

    /// <summary>
    /// Tracks a file write operation.
    /// </summary>
    /// <param name="path">The path of the file being written.</param>
    void TrackFileWrite(string path);
}