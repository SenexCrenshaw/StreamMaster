namespace StreamMaster.Streams.Domain.Statistics;

public class StreamHandlerMetrics
{
    public long BytesRead { get; set; }
    public long BytesWritten { get; set; }
    public double Kbps { get; set; }
    public DateTime StartTime { get; set; }
    public double AverageLatency { get; set; }
    public int ErrorCount { get; set; }
}
