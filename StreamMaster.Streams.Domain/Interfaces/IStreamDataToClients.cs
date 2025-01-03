namespace StreamMaster.Streams.Domain.Interfaces;

/// <summary>
/// Provides functionality for streaming data to clients.
/// </summary>
public interface IStreamDataToClients
{
    /// <summary>
    /// Streams data to clients asynchronously.
    /// </summary>
    /// <param name="buffer">The data to stream.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task StreamDataToClientsAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken);
}
