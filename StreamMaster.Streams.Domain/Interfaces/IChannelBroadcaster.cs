using StreamMaster.Streams.Domain.Events;

namespace StreamMaster.Streams.Domain.Interfaces;

/// <summary>
/// Provides methods and properties to manage the status and configuration of a channel.
/// </summary>
public interface IChannelBroadcaster : IBroadcasterBase, IStreamStatus
{
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
    void SetSourceChannelBroadcaster(ISourceBroadcaster ChannelBroadcaster);

    bool IsGlobal { get; set; }
}