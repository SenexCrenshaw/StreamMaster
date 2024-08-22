using StreamMaster.Streams.Domain.Events;

namespace StreamMaster.Streams.Handlers;

public class SourceBroadcaster : BroadcasterBase, ISourceBroadcaster
{
    private readonly ILogger<ISourceBroadcaster> logger;

    public SourceBroadcaster() : base(null)
    {

    }

    public SourceBroadcaster(ILogger<ISourceBroadcaster> logger, SMStreamInfo smStreamInfo) : base(logger)
    {
        this.logger = logger;
        this.SMStreamInfo = smStreamInfo;
    }

    public event EventHandler<StreamBroadcasterStopped>? OnStreamBroadcasterStoppedEvent;

    public override string StringId()
    {
        return Id;
    }

    /// <inheritdoc/>
    public string Id => SMStreamInfo.Url;

    public SMStreamInfo SMStreamInfo { get; }

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
