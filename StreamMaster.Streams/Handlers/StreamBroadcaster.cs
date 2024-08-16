using StreamMaster.Streams.Domain.Events;

namespace StreamMaster.Streams.Handlers;

public class StreamBroadcaster : BroadcasterBase, IStreamBroadcaster
{
    private readonly ILogger<IStreamBroadcaster> logger;

    public StreamBroadcaster() { }

    public event EventHandler<StreamBroadcasterStopped>? OnStoppedEvent;

    public StreamBroadcaster(ILogger<IStreamBroadcaster> logger, SMStreamInfo smStreamInfo) : base(logger)
    {
        SMStreamInfo = smStreamInfo;
        Name = SMStreamInfo.Name;
        this.logger = logger;
    }

    public override string StringId()
    {
        return Id;
    }

    /// <inheritdoc/>
    public string Id => SMStreamInfo.Url;

    public SMStreamInfo SMStreamInfo { get; }

    /// <inheritdoc/>
    public override void OnStreamingStopped()
    {
        OnStoppedEvent?.Invoke(this, new StreamBroadcasterStopped(Id, Name));
    }
}
