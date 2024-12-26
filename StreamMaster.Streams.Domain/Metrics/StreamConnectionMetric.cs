namespace StreamMaster.Streams.Domain.Metrics;

public class StreamConnectionMetric(string? StreamUrl = null)
{
    public TimeSpan AvgConnectionTime { get; set; }
    public DateTime? LastSuccessConnect { get; set; }
    public DateTime? LastErrorTime { get; set; }
    public DateTime? LastRetryTime { get; set; }
    public int RetryCount { get; set; }
    public int TotalConnectionAttempts { get; set; }
    public string StreamUrl { get; } = StreamUrl ?? "";
    public DateTime LastStartTime { get; set; } = DateTime.UtcNow;
}
