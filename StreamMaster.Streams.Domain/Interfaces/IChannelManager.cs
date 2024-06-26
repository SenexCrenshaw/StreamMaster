using StreamMaster.Domain.Models;

namespace StreamMaster.Streams.Domain.Interfaces;

/// <summary>
/// Provides methods for managing video streams and clients in a channel.
/// This interface also implements IDisposable to free resources.
/// </summary>
public interface IChannelManager : IDisposable
{
    /// <summary>
    /// Gets the video information for a specific channel.
    /// </summary>
    /// <param name="SMChannelId">The ID of the channel.</param>
    /// <returns>The video information.</returns>
    VideoInfo GetVideoInfo(int SMChannelId);

    /// <summary>
    /// Asynchronously changes the video stream of a channel.
    /// </summary>
    /// <param name="playingSMStreamId">The ID of the currently playing video stream.</param>
    /// <param name="newSMStreamId">The ID of the new video stream to switch to.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    Task ChangeVideoStreamChannelAsync(string playingSMStreamId, string newSMStreamId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Fails a client by its unique identifier.
    /// </summary>
    /// <param name="clientId">The unique identifier of the client to fail.</param>
    void FailClient(Guid clientId);

    /// <summary>
    /// Asynchronously gets a stream based on the given client streamer configuration.
    /// </summary>
    /// <param name="config">The configuration settings for the client streamer.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A Task returning the stream. Returns null if the stream could not be obtained.</returns>
    Task<Stream?> GetChannelAsync(ClientStreamerConfiguration config, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously removes a client based on the given client streamer configuration.
    /// </summary>
    /// <param name="config">The configuration settings for the client to be removed.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    Task RemoveClientAsync(ClientStreamerConfiguration config);

    /// <summary>
    /// Simulates a stream failure for testing purposes.
    /// </summary>
    /// <param name="streamUrl">The URL of the stream to fail.</param>    
    void SimulateStreamFailure(string streamUrl);

    /// <summary>
    /// Simulates a stream failure for all streams for testing purposes.
    /// </summary>    
    void SimulateStreamFailureForAll();
}
