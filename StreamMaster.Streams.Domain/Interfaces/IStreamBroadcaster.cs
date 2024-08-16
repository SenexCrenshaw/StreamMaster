﻿using StreamMaster.Domain.Models;
using StreamMaster.Streams.Domain.Events;

using System.Collections.Concurrent;
using System.Threading.Channels;
using System.Xml.Serialization;

namespace StreamMaster.Streams.Domain.Interfaces;

public interface IStreamBroadcaster : IBroadcasterBase
{
    /// <summary>
    /// Gets the unique identifier for the channel.
    /// </summary>
    string Id { get; }

    /// <summary>
    /// Occurs when the channel director is stopped.
    /// </summary>
    event EventHandler<StreamBroadcasterStopped> OnStoppedEvent;

    public SMStreamInfo SMStreamInfo { get; }
}



/// <summary>
/// Defines the properties and methods for broadcasting channels in StreamMaster.
/// </summary>
public interface IBroadcasterBase : IStreamStats, ISourceName
{
    void AddClientStreamer(string UniqueRequestId, IClientConfiguration config);

    /// <summary>
    /// Removes a client channel.
    /// </summary>
    /// <param name="key">The key for the client channel.</param>
    /// <returns><c>true</c> if the client channel was removed; otherwise, <c>false</c>.</returns>
    bool RemoveClientChannel(int key);

    bool RemoveClientStreamer(string UniqueRequestId);
    void AddClientChannel(int key, ChannelWriter<byte[]> channel);

    List<IClientConfiguration> GetClientStreamerConfigurations();
    int ClientCount { get; }

    //ConcurrentDictionary<string, IClientConfiguration> ClientStreamerConfigurations { get; set; }

    /// <summary>
    /// Gets the name of the channel.
    /// </summary>
    string Name { get; }

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
    /// Gets the client channels.
    /// </summary>
    [XmlIgnore]
    ConcurrentDictionary<string, ChannelWriter<byte[]>> ClientChannels { get; }

    /// <summary>
    /// Gets the client streams.
    /// </summary>
    [XmlIgnore]
    ConcurrentDictionary<string, Stream> ClientStreams { get; }


    /// <summary>
    /// Sets the source channel.
    /// </summary>
    /// <param name="sourceChannelReader">The source channel reader.</param>
    /// <param name="sourceChannelName">The name of the source channel.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    void SetSourceChannel(ChannelReader<byte[]> sourceChannelReader, string sourceChannelName, CancellationToken cancellationToken);

    /// <summary>
    /// Sets the source stream.
    /// </summary>
    /// <param name="sourceStream">The source stream.</param>
    /// <param name="channelName">The name of the channel.</param>
    /// <param name="streamName">The name of the stream.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    void SetSourceStream(Stream sourceStream, string channelName, string streamName, CancellationToken cancellationToken);

    /// <summary>
    /// Stops the channel broadcaster.
    /// </summary>
    void Stop();

    /// <summary>
    /// Adds a client channel.
    /// </summary>
    /// <param name="key">The key for the client channel.</param>
    /// <param name="Channel">The client channel writer.</param>
    void AddClientChannel(string key, ChannelWriter<byte[]> Channel);

    /// <summary>
    /// Removes a client channel.
    /// </summary>
    /// <param name="key">The key for the client channel.</param>
    /// <returns><c>true</c> if the client channel was removed; otherwise, <c>false</c>.</returns>
    bool RemoveClientChannel(string key);


    /// <summary>
    /// Gets a value indicating whether the broadcaster has failed.
    /// </summary>
    bool IsFailed { get; }
}