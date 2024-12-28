namespace StreamMaster.Streams.Domain.Metrics;

public class StreamConnectionMetricData(string? StreamUrl = null)
{
    public DateTime? LastSuccessConnectTime { get; set; }
    public DateTime? LastErrorTime { get; set; }
    public DateTime? LastConnectionAttemptTime { get; set; }
    public DateTime LastStartTime { get; set; } = DateTime.UtcNow;

    public int RetryCount { get; set; }
    public int TotalConnectionAttempts { get; set; } = 0;
    public string StreamUrl { get; } = StreamUrl ?? string.Empty;
}