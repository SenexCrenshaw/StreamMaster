namespace StreamMaster.Streams.Domain.Models;

public class ChannelDirectorStopped(string Id, string Name)
{
    public string Id { get; set; } = Id;
    public string Name { get; set; } = Name;
    //public string Url { get; set; } = Url;
}

public class ChannelStatusStopped(int Id, string Name)
{
    public int Id { get; set; } = Id;
    public string Name { get; set; } = Name;
    //public IChannelStatus ChannelStatus { get; set; }
}

