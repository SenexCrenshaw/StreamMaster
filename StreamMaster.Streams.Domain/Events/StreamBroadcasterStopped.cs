namespace StreamMaster.Streams.Domain.Events;

public class VideoCombinerStopped(int Id, string Name)
{
    public int Id { get; set; } = Id;
    public string Name { get; set; } = Name;
}

public class StreamBroadcasterStopped(string Id, string Name, bool IsCancelled)
{
    public string Id { get; set; } = Id;
    public string Name { get; set; } = Name;
    public bool IsCancelled { get; set; } = IsCancelled;
}

public class ChannelBroadcasterStopped(int Id, string Name)
{
    public int Id { get; set; } = Id;
    public string Name { get; set; } = Name;
}
