using StreamMaster.Streams.Domain.Events;

namespace StreamMaster.Streams.Handlers;

public class ChannelBroadcaster : ChannelBroadcasterBase, IChannelBroadcaster
{
    public ChannelBroadcaster() { }

    public event EventHandler<ChannelDirectorStopped>? OnStoppedEvent;

    public ChannelBroadcaster(ILogger<IChannelBroadcaster> logger, SMStreamInfo smStreamInfo) : base(logger)
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
    public override void OnStreamingStopped()
    {
        OnStoppedEvent?.Invoke(this, new ChannelDirectorStopped(Id, Name));
    }
}
