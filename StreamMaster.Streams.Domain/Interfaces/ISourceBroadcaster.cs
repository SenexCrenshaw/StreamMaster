using StreamMaster.Domain.Models;
using StreamMaster.Streams.Domain.Events;

namespace StreamMaster.Streams.Domain.Interfaces;

public interface ISourceBroadcaster : IBroadcasterBase
{
    /// <summary>
    /// Gets the unique identifier for the channel.
    /// </summary>
    string Id { get; }

    /// <summary>
    /// Occurs when the channel director is stopped.
    /// </summary>
    event EventHandler<StreamBroadcasterStopped> OnStreamBroadcasterStoppedEvent;

    public SMStreamInfo SMStreamInfo { get; }
}
