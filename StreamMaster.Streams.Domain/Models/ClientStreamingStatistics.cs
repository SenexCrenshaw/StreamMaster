namespace StreamMaster.Streams.Domain.Models;

public class ClientStreamingStatistics : StreamingStatistics
{
    public ClientStreamingStatistics(string ClientAgent, string ClientIPAddress) : base(ClientAgent, ClientIPAddress)
    {
    }

    public Guid ClientId { get; set; }
}
