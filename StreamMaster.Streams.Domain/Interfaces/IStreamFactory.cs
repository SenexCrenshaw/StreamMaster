using StreamMaster.Domain.Models;

namespace StreamMaster.Streams.Domain.Interfaces;

/// <summary>
/// Represents a factory for creating and managing streams.
/// </summary>
public interface IStreamFactory
{
    /// <summary>
    /// Creates a stream for the specified channel broadcaster.
    /// </summary>
    /// <param name="SMStreamInfo">SMStream Info for which to create the stream.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task that represents the asynchronous operation, returning a <see cref="GetStreamResult"/> containing the stream, process ID, and error information.
    /// </returns>
    Task<GetStreamResult> GetStream(SMStreamInfo SMStreamInfo, CancellationToken cancellationToken);

    Task<GetStreamResult> GetMultiViewPlayListStream(IChannelBroadcaster channelBroadcaster, CancellationToken cancellationToken);
}
