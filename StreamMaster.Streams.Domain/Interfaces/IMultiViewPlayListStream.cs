namespace StreamMaster.Streams.Domain.Interfaces;

/// <summary>
/// Represents a handler for managing multi-view playlist streams.
/// </summary>
public interface IMultiViewPlayListStream
{
    /// <summary>
    /// Handles the creation of a stream for the specified channel broadcaster.
    /// </summary>
    /// <param name="channelBroadcaster">The channel broadcaster associated with the multi-view playlist stream.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task representing the asynchronous operation, returning a <see cref="GetStreamResult"/> containing:
    /// - The stream if successfully created.
    /// - The process ID associated with the stream.
    /// - An error object if the operation fails.
    /// </returns>
    Task<GetStreamResult> HandleStream(
        IChannelBroadcaster channelBroadcaster,
        CancellationToken cancellationToken);
}
