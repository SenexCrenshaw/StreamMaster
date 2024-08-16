namespace StreamMaster.Streams.Domain.Interfaces
{
    /// <summary>
    /// Provides methods for managing video streams and clients in a channel.
    /// This interface also implements IDisposable to free resources.
    /// </summary>
    public interface IChannelManager : IDisposable
    {
        /// <summary>
        /// Cancels all active channels.
        /// </summary>
        void CancelAllChannels();

        /// <summary>
        /// Cancels a specific channel by its ID.
        /// </summary>
        /// <param name="SMChannelId">The ID of the channel to cancel.</param>
        Task CancelChannelAsync(int SMChannelId);

        /// <summary>
        /// Fails a client by its unique identifier.
        /// </summary>
        /// <param name="uniqueRequestId">The unique identifier of the client to fail.</param>
        Task CancelClientAsync(string uniqueRequestId);

        /// <summary>
        /// Asynchronously changes the video stream of a channel.
        /// </summary>
        /// <param name="playingSMStreamId">The ID of the currently playing video stream.</param>
        /// <param name="newSMStreamId">The ID of the new video stream to switch to.</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        Task ChangeVideoStreamChannelAsync(string playingSMStreamId, string newSMStreamId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously gets a stream based on the given client streamer configuration.
        /// </summary>
        /// <param name="config">The configuration settings for the client streamer.</param>
        /// <param name="streamGroupProfileId">The ID of the stream group profile.</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
        /// <returns>A Task returning the stream. Returns null if the stream could not be obtained.</returns>
        Task<Stream?> GetChannelStreamAsync(IClientConfiguration config, int streamGroupProfileId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously removes a client based on the given client streamer configuration.
        /// </summary>
        /// <param name="config">The configuration settings for the client to be removed.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        Task RemoveClientAsync(IClientConfiguration config);

        /// <summary>
        /// Moves to the next video stream in the channel by its ID.
        /// </summary>
        /// <param name="SMChannelId">The ID of the channel.</param>
        void MoveToNextStream(int SMChannelId);
    }
}
