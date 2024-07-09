
using StreamMaster.Domain.Models;
using StreamMaster.Streams.Domain.Statistics;

namespace StreamMaster.Streams.Domain.Interfaces;

/// <summary>
/// Provides methods for managing and querying stream handlers.
/// </summary>
public interface IStreamManager
{

    bool IsHealthy();
    IDictionary<string, StreamHandlerMetrics> GetAggregatedMetrics();
    void AddClientsToHandler(List<ClientStreamerConfiguration> clientIds, IStreamHandler streamHandler);
    void AddClientToHandler(ClientStreamerConfiguration streamerConfiguration, IStreamHandler streamHandler);
    VideoInfo GetVideoInfo(string streamUrl);

    event EventHandler<StreamHandlerStopped> OnStreamingStoppedEvent;
    /// <summary>
    /// Disposes of the resources used by the StreamManager.
    /// </summary>
    void Dispose();

    /// <summary>
    /// Asynchronously gets or creates a stream handler based on the given child video stream DTO and rank.
    /// </summary>
    /// <param name="childVideoStreamDto">The child video stream data transfer object.</param>
    /// <param name="rank">The rank of the stream.</param>
    /// <param name="cancellation">Optional cancellation token to cancel the operation.</param>
    /// <returns>A Task returning an IStreamHandler if successful; otherwise, returns null.</returns>
    Task<IStreamHandler?> GetOrCreateStreamHandler(IChannelStatus channelStatus, CancellationToken cancellation = default);

    /// <summary>
    /// Retrieves the stream information based on a given stream URL.
    /// </summary>
    /// <param name="streamUrl">The URL of the stream.</param>
    /// <returns>An IStreamHandler if the stream exists; otherwise, returns null.</returns>
    IStreamHandler? GetStreamHandlerFromStreamUrl(string streamUrl);

    /// <summary>
    /// Retrieves a stream handler based on a given video stream ID.
    /// </summary>
    /// <param name="VideoStreamId">The ID of the video stream.</param>
    /// <returns>An IStreamHandler if the stream exists; otherwise, returns null.</returns>
    IStreamHandler? GetStreamHandler(string? VideoStreamId);

    IStreamHandler? GetStreamHandlerByClientId(Guid ClientId);

    ///// <summary>
    ///// Retrieves the information for all streams.
    ///// </summary>
    ///// <returns>A collection of IStreamHandler objects.</returns>
    //ICollection<IStreamHandler> GetStreamHandlers();
    //Task MoveClientStreamers(IStreamHandler oldStreamHandler, IStreamHandler newStreamHandler, CancellationToken cancellationToken = default);
    /// <summary>
    /// Retrieves all stream handlers.
    /// </summary>
    /// <returns>A list of IStreamHandler objects.</returns>
    List<IStreamHandler> GetStreamHandlers();

    /// <summary>
    /// Gets the count of streams associated with a specific M3U file.
    /// </summary>
    /// <param name="m3uFileId">The ID of the M3U file.</param>
    /// <returns>An integer representing the number of streams.</returns>
    int GetStreamsCountForM3UFile(int m3uFileId);

    /// <summary>
    /// UnRegister a handler and stops a stream based on a given video stream ID.
    /// </summary>
    /// <param name="VideoStreamUrl">The URL of the video stream to stop.</param>
    /// <returns>Returns true if the stopped stream, false if CurrentVideoStreamId not found.</returns>
    bool StopAndUnRegisterHandler(string VideoStreamUrl);
    int UnRegisterClientStreamer(string url, Guid clientId, string SMChannelName);
}