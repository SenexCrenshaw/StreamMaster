using System.Buffers;
using System.Collections.Concurrent;

using StreamMaster.Streams.Domain.Events;

namespace StreamMaster.Streams.Domain.Interfaces;

public interface IStreamDataToClients
{
    Task StreamDataToClientsAsync(ReadOnlySequence<byte> buffer, CancellationToken cancellationToken);
}
/// <summary>
/// Provides methods and properties to manage the status and configuration of a channel.
/// </summary>
public interface IChannelBroadcaster : IStreamStatus, IStreamDataToClients
{
    void Stop();
    bool ClientConfigurationsEmpty { get; }
    bool RemoveClient(string clientId);
    ConcurrentDictionary<string, IClientConfiguration> Clients { get; }
    void AddChannelStreamer(IClientConfiguration clientConfiguration);
    //Pipe Pipe { get; set; }
    /// <summary>
    /// Gets the unique identifier for the channel.
    /// </summary>
    int Id { get; }
    /// <summary>
    /// Occurs when the channel director is stopped.
    /// </summary>
    event EventHandler<ChannelBroascasterStopped>? OnChannelBroadcasterStoppedEvent;

    //SMChannelDto SMChannel { get; }
    void SetIsGlobal();
    void SetSourceChannelBroadcaster(ISourceBroadcaster SourceChannelBroadcaster);

    bool IsGlobal { get; set; }
    bool IsFailed { get; }
}