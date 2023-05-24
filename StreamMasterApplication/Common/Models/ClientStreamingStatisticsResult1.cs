namespace StreamMasterApplication.Common.Models;

public class ClientStreamingStatisticsResult
{
    public Guid ClientId { get; set; }
    public StreamingStatistics? Statistics { get; set; }
}
