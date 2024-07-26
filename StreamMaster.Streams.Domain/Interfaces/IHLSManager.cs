namespace StreamMaster.Streams.Domain.Interfaces;

/// <summary>
/// Interface for managing HLS (HTTP Live Streaming) handlers.
/// </summary>
public interface IHLSManager : IDisposable
{
    /// <summary>
    /// Retrieves an HLS handler for the specified stream ID.
    /// </summary>
    /// <param name="smStreamId">The ID of the stream.</param>
    /// <returns>An instance of <see cref="IHLSHandler"/> if found; otherwise, null.</returns>
    IHLSHandler? Get(string smStreamId);

    Task<IM3U8ChannelStatus?> TryAddAsync(SMChannelDto smChannel, CancellationToken cancellationToken);

    /// <summary>
    /// Stops the HLS handler for the specified stream ID.
    /// </summary>
    /// <param name="smStreamId">The ID of the stream.</param>
    void Stop(string smStreamId);
}
