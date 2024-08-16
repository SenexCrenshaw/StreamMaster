namespace StreamMaster.Streams.Domain.Events;

public class StreamBroadcasterStopped(string Id, string Name)
{
    public string Id { get; set; } = Id;
    public string Name { get; set; } = Name;
    //public string Url { get; set; } = Url;
}

public class ChannelBroascasterStopped(int Id, string Name)
{
    public int Id { get; set; } = Id;
    public string Name { get; set; } = Name;
    //public IChannelBroadcaster ChannelStatus { get; set; }
}

