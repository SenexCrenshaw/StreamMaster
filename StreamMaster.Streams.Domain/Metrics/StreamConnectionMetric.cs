namespace StreamMaster.Streams.Domain.Metrics;

public class StreamConnectionMetricData(string? StreamUrl = null)
{
    public DateTime? LastSuccessConnect { get; set; }
    public DateTime? LastErrorTime { get; set; }
    public DateTime? LastRetryTime { get; set; }
    public int RetryCount { get; set; }
    public int TotalConnectionAttempts { get; set; } = 0;
    public string StreamUrl { get; } = StreamUrl ?? string.Empty;
    public DateTime LastStartTime { get; set; } = DateTime.UtcNow;
}