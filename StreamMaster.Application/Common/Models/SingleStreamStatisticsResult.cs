namespace StreamMaster.Application.Common.Models;

public class SingleStreamStatisticsResult
{
    public List<ClientStreamingStatistics>? ClientStatistics { get; set; }
    public StreamingStatistics? InputStreamStatistics { get; set; }
    public string? StreamUrl { get; set; }
}
