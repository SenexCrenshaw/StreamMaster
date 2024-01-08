namespace StreamMaster.Streams.Domain.Models;

public class ClientStreamingStatistics : StreamingStatistics
{
    public ClientStreamingStatistics(IClientStreamerConfiguration streamerConfiguration) : base(streamerConfiguration)
    {
    }


    public Guid ClientId { get; set; }
}
