using StreamMaster.Streams.Domain.Events;

namespace StreamMaster.Streams.Handlers;

public class SourceBroadcaster : BroadcasterBase, ISourceBroadcaster
{
    public SourceBroadcaster() { }

    public event EventHandler<StreamBroadcasterStopped>? OnStreamBroadcasterStoppedEvent;

    public SourceBroadcaster(ILogger<ISourceBroadcaster> logger, SMStreamInfo smStreamInfo) : base(logger)
    {
        SMStreamInfo = smStreamInfo;
        Name = SMStreamInfo.Name;

    }

    public override string StringId()
    {
        return Id;
    }

    /// <inheritdoc/>
    public string Id => SMStreamInfo.Url;

    public SMStreamInfo SMStreamInfo { get; }

    /// <inheritdoc/>
    public override void OnBaseStopped()
    {
        OnStreamBroadcasterStoppedEvent?.Invoke(this, new StreamBroadcasterStopped(Id, Name));
    }


}
