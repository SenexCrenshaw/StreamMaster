using StreamMaster.Streams.Domain.Events;

namespace StreamMaster.Streams.Handlers;

public class SourceBroadcaster(ILogger<ISourceBroadcaster> logger, SMStreamInfo smStreamInfo, IOptionsMonitor<Setting> _settings) : BroadcasterBase(logger, _settings), ISourceBroadcaster
{
    private int _isStopped;

    public event EventHandler<StreamBroadcasterStopped>? OnStreamBroadcasterStoppedEvent;

    public override string StringId()
    {
        return Id;
    }

    /// <inheritdoc/>
    public string Id => SMStreamInfo.Url;

    public override string Name => SMStreamInfo.Name;

    public SMStreamInfo SMStreamInfo { get; } = smStreamInfo;

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
