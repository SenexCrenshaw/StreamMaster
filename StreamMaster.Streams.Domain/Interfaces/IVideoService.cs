using Microsoft.AspNetCore.Http;

namespace StreamMaster.Streams.Domain.Interfaces;

/// <summary>
/// Service for managing video-related operations, including adding clients to channels.
/// </summary>
public interface IVideoService : IDisposable
{
    /// <summary>
    /// Adds a client to a specific channel and starts streaming.
    /// </summary>
    /// <param name="httpContext">The HTTP context of the client request.</param>
    /// <param name="streamGroupId">The optional ID of the stream group to associate with the client.</param>
    /// <param name="streamGroupProfileId">The optional ID of the stream group profile to use for streaming.</param>
    /// <param name="smChannelId">The optional ID of the channel to add the client to.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task representing the asynchronous operation. Returns a <see cref="StreamResult"/> indicating the result of the operation.
    /// </returns>
    Task<StreamResult> AddClientToChannelAsync(
        HttpContext httpContext,
        int? streamGroupId,
        int? streamGroupProfileId,
        int? smChannelId,
        CancellationToken cancellationToken);
}
