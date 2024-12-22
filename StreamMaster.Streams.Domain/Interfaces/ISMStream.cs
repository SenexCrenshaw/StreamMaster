using StreamMaster.Domain.Models;

namespace StreamMaster.Streams.Domain.Interfaces;

/// <summary>
/// Represents a handler for managing streams.
/// </summary>
public interface ISMStream
{
    /// <summary>
    /// Handles the stream creation using the provided stream information and user agent.
    /// </summary>
    /// <param name="smStreamInfo">The stream information.</param>
    /// <param name="clientUserAgent">The user agent of the client.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation, returning a <see cref="GetStreamResult"/>.</returns>
    Task<GetStreamResult> HandleStream(SMStreamInfo smStreamInfo, string clientUserAgent, CancellationToken cancellationToken);
}
