using StreamMaster.Streams.Domain.Events;

namespace StreamMaster.Streams.Handlers;

public class SourceBroadcaster(ILogger<ISourceBroadcaster> logger, SMStreamInfo smStreamInfo)
    : BroadcasterBase(logger), ISourceBroadcaster
{
    public event EventHandler<StreamBroadcasterStopped>? OnStreamBroadcasterStoppedEvent;

    public override string StringId()
    {
        return Id;
    }

    /// <inheritdoc/>
    public string Id => SMStreamInfo.Url;

    public SMStreamInfo SMStreamInfo => smStreamInfo;

    /// <inheritdoc/>
    public override void Stop()
    {
        // Derived-specific logic before stopping
        logger.LogInformation("Source Broadcaster stopped: {Name}", Name);

        // Call base class stop logic
        base.Stop();

        // Additional cleanup or finalization
        OnStreamBroadcasterStoppedEvent?.Invoke(this, new StreamBroadcasterStopped(Id, Name));
    }
}
