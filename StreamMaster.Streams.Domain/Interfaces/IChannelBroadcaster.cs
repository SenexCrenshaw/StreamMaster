using System.Collections.Concurrent;
using System.IO.Pipelines;

using StreamMaster.Streams.Domain.Events;

namespace StreamMaster.Streams.Domain.Interfaces;

/// <summary>
/// Provides methods and properties to manage the status and configuration of a channel.
/// </summary>
public interface IChannelBroadcaster : IStreamStatus
{
    void Stop();
    bool ClientConfigurationsEmpty { get; }
    bool RemoveClient(string clientId);
    ConcurrentDictionary<string, IClientConfiguration> Clients { get; }
    Task AddChannelStreamerAsync(IClientConfiguration clientConfiguration);
    Pipe Pipe { get; set; }
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