namespace StreamMaster.Streams.Domain.Events;

public class VideoCombinerStopped(int Id, string Name)
{
    public int Id { get; set; } = Id;
    public string Name { get; set; } = Name;
}

public class StreamBroadcasterStopped(string Id, string Name)
{
    public string Id { get; set; } = Id;
    public string Name { get; set; } = Name;
}

public class ChannelBroadcasterStopped(int Id, string Name)
{
    public int Id { get; set; } = Id;
    public string Name { get; set; } = Name;
}
