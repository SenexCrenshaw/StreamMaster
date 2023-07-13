namespace StreamMasterApplication.Common.Models;

public class ClientStreamingStatistics : StreamingStatistics
{
    public ClientStreamingStatistics(string ClientAgent) : base(ClientAgent)
    {
    }

    public Guid ClientId { get; set; }
}
