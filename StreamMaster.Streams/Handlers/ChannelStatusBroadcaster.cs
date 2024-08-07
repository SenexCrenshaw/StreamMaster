
using StreamMaster.Streams.Domain.Events;

namespace StreamMaster.Streams.Handlers;

public class ChannelStatusBroadcaster : ChannelBroadcasterBase, IChannelStatusBroadcaster
{
    public ChannelStatusBroadcaster() { }

    /// <inheritdoc/>
    public event EventHandler<ChannelStatusStopped>? OnChannelStatusStoppedEvent;

    /// <inheritdoc/>
    public int Id { get; }
    public override string StringId()
    {
        return Id.ToString();
    }

    public ChannelStatusBroadcaster(ILogger<IChannelBroadcaster> logger, SMChannelDto smChannelDto) : base(logger)
    {
        Id = smChannelDto.Id;
        Name = smChannelDto.Name;
    }


    /// <inheritdoc/>
    public override void OnStreamingStopped()
    {
        OnChannelStatusStoppedEvent?.Invoke(this, new ChannelStatusStopped(Id, Name));
    }
}
