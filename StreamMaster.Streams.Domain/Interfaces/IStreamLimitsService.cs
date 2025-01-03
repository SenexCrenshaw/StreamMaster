namespace StreamMaster.Streams.Domain.Interfaces;

/// <summary>
/// Provides methods for enforcing and checking stream limits.
/// </summary>
public interface IStreamLimitsService
{
    /// <summary>
    /// Checks if the specified stream exceeds its limits based on the associated M3U file or configuration.
    /// </summary>
    /// <param name="smStreamDto">The stream to check.</param>
    /// <returns><c>true</c> if the stream is limited; otherwise, <c>false</c>.</returns>
    bool IsLimited(SMStreamDto smStreamDto);

    /// <summary>
    /// Checks if the specified stream by ID exceeds its limits based on the associated M3U file or configuration.
    /// </summary>
    /// <param name="smStreamDtoId">The ID of the stream to check.</param>
    /// <returns><c>true</c> if the stream is limited; otherwise, <c>false</c>.</returns>
    bool IsLimited(string smStreamDtoId);

    /// <summary>
    /// Gets the current and maximum allowed stream counts for the specified stream by ID.
    /// </summary>
    /// <param name="smStreamId">The ID of the stream to check.</param>
    /// <returns>
    /// A tuple containing:
    /// <list type="bullet">
    /// <item><description>The current stream count.</description></item>
    /// <item><description>The maximum allowed stream count.</description></item>
    /// </list>
    /// </returns>
    (int currentStreamCount, int maxStreamCount) GetStreamLimits(string smStreamId);
}
