namespace StreamMaster.Streams.Domain.Interfaces;

/// <summary>
/// Provides functionality to switch to the next available stream for a given channel status.
/// </summary>
public interface ISwitchToNextStreamService
{
    /// <summary>
    /// Sets the next stream for the specified channel status.
    /// </summary>
    /// <param name="channelStatus">The current status of the stream channel.</param>
    /// <param name="overrideSMStreamId">
    /// An optional stream ID to override the automatic selection.
    /// If specified, the service will attempt to switch to the provided stream ID.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation. Returns <c>true</c> if the switch was successful; otherwise, <c>false</c>.
    /// </returns>
    Task<bool> SetNextStreamAsync(IStreamStatus channelStatus, string? overrideSMStreamId = null);
}
