using AutoMapper;

using StreamMaster.Domain.Events;
using StreamMaster.Streams.Domain.Events;

namespace StreamMaster.Streams.Domain.Interfaces;
public interface IVideoCombinerService
{ /// <summary>
  /// Occurs when a channel director is stopped.
  /// </summary>
    event AsyncEventHandler<VideoCombinerStopped>? OnVideoCombinerStoppedEvent;

    /// <summary>
    /// Gets a channel distributor by its string key.
    /// </summary>
    /// <param name="key">The int key of the channel distributor.</param>
    /// <returns>The channel distributor if found; otherwise, <c>null</c>.</returns>
    IVideoCombiner? GetVideoCombiner(int key);

    /// <summary>
    /// Gets aggregated metrics for all channel distributors.
    /// </summary>
    /// <returns>A dictionary of aggregated metrics for all channel distributors.</returns>
    IDictionary<string, IStreamHandlerMetrics> GetMetrics();


    Task<IVideoCombiner?> GetOrCreateVideoCombinerAsync(IClientConfiguration config, IMapper mapper, IChannelService channelService, int SMChannelId1, int SMChannelId2, int SMChannelId3, int SMChannelId4, int streamGroupProfileId, CancellationToken cancellationToken);

    /// <summary>
    /// Stops and unregisters a channel distributor by its string key.
    /// </summary>
    /// <param name="key">The string key of the channel distributor.</param>
    /// <returns><c>true</c> if the channel distributor was stopped and unregistered; otherwise, <c>false</c>.</returns>
    bool StopAndUnRegisterSourceBroadcaster(int key);

    /// <summary>
    /// Gets all channel distributors.
    /// </summary>
    /// <returns>A list of all channel distributors.</returns>
    List<IVideoCombiner> GetVideoCombiners();
    //void Stop(string channelBroadcasterId);

    Task UnRegisterChannelBroadcasterAsync(int channelBroadcasterId);
    //Task CombineVideosServiceAsync(IClientConfiguration config, int SMChannelId1, int SMChannelId2, int SMChannelId3, int SMChannelId4, int streamGroupProfileId, ChannelWriter<byte[]> channelWriter, CancellationToken cancellationToken);
}