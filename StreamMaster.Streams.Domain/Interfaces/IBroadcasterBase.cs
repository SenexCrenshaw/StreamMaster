using System.Collections.Concurrent;
using System.Xml.Serialization;

namespace StreamMaster.Streams.Domain.Interfaces;

/// <summary>
/// Defines the properties and methods for broadcasting Channels in StreamMaster.
/// </summary>
public interface IBroadcasterBase : IStreamStats, ISourceName
{
    //Channels
    void AddChannelStreamer(int key, TrackedChannel channel);
    bool RemoveChannelStreamer(int key);

    //Plugins
    void AddChannelStreamer(string UniqueRequestId, TrackedChannel channel);
    bool RemoveChannelStreamer(string UniqueRequestId);

    //ClientChannels
    void AddClientStreamer(string UniqueRequestId, IClientConfiguration config);
    bool RemoveClientStreamer(string UniqueRequestId);

    List<IClientConfiguration> GetClientStreamerConfigurations();
    int ClientCount { get; }

    /// <summary>
    /// Gets the name of the channel.
    /// </summary>
    string Name { get; }

    TrackedChannel Channel { get; }
    /// <summary>
    /// Gets the count of items in the channel.
    /// </summary>
    long ChannelItemBackLog { get; }

    /// <summary>
    /// Determines whether the channel is empty.
    /// </summary>
    /// <returns><c>true</c> if the channel is empty; otherwise, <c>false</c>.</returns>
    bool IsChannelEmpty();

    /// <summary>
    /// Gets the client Channels.
    /// </summary>
    [XmlIgnore]
    ConcurrentDictionary<string, TrackedChannel> ClientChannels { get; }

    ///// <summary>
    ///// Gets the client streams.
    ///// </summary>
    //[XmlIgnore]
    //ConcurrentDictionary<string, Stream> ClientStreams { get; }

    /// <summary>
    /// Sets the source channel.
    /// </summary>
    /// <param name="sourceChannelReader">The source channel reader.</param>
    /// <param name="sourceChannelName">The name of the source channel.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    void SetSourceChannel(TrackedChannel sourceChannelReader, string sourceChannelName, CancellationToken cancellationToken);

    //void SetSourceChannel(Channel<byte[]> sourceChannel, string sourceChannelName, CancellationToken cancellationToken);

    /// <summary>
    /// Sets the source stream.
    /// </summary>
    /// <param name="sourceStream">The source stream.</param>
    /// <param name="streamName">The name of the stream.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    void SetSourceStream(Stream sourceStream, string streamName, CancellationToken cancellationToken);

    /// <summary>
    /// Stops the channel broadcaster.
    /// </summary>
    void Stop();

    /// <summary>
    /// Gets a value indicating whether the broadcaster has failed.
    /// </summary>
    bool IsFailed { get; }
}
