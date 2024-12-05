using StreamMaster.Streams.Domain.Events;

namespace StreamMaster.Streams.Handlers;

public class SourceBroadcaster : BroadcasterBase, ISourceBroadcaster
{
    private int _isStopped;

    public SourceBroadcaster() : base(null, null) { }

    public SourceBroadcaster(ILogger<ISourceBroadcaster> logger, SMStreamInfo smStreamInfo, IOptionsMonitor<Setting> _settings) : base(logger, _settings)
    {
        SMStreamInfo = smStreamInfo;
    }

    public event EventHandler<StreamBroadcasterStopped>? OnStreamBroadcasterStoppedEvent;

    public override string StringId()
    {
        return Id;
    }

    /// <inheritdoc/>
    public string Id => SMStreamInfo.Url;

    public override string Name => SMStreamInfo.Name;

    public required SMStreamInfo SMStreamInfo { get; set; }

    /// <inheritdoc/>
    public override void Stop()
    {
        if (Interlocked.CompareExchange(ref _isStopped, 1, 0) == 0)
        {
            // Derived-specific logic before stopping
            logger.LogInformation("Source Broadcaster stopped: {Name}", Name);

            // Call base class stop logic
            base.Stop();

            // Additional cleanup or finalization
            OnStreamBroadcasterStoppedEvent?.Invoke(this, new StreamBroadcasterStopped(Id, Name));
        }
    }
}
