using StreamMaster.Streams.Domain.Events;

namespace StreamMaster.Streams.Handlers;

public class ChannelBroadcaster : ChannelBroadcasterBase, IChannelBroadcaster
{
    public ChannelBroadcaster() { }

    public event EventHandler<ChannelDirectorStopped>? OnStoppedEvent;

    public ChannelBroadcaster(ILogger<IChannelBroadcaster> logger, string id, string name) : base(logger)
    {
        Id = id;
        Name = name;

    }

    public override string StringId()
    {
        return Id;
    }

    /// <inheritdoc/>
    public string Id { get; } = string.Empty;

    /// <inheritdoc/>
    public override void OnStreamingStopped()
    {
        OnStoppedEvent?.Invoke(this, new ChannelDirectorStopped(Id, Name));
    }
}
