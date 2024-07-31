namespace StreamMaster.Streams.Domain.Interfaces
{
    public interface IStreamHandlerMetrics
    {
        double AverageLatency { get; set; }
        long BytesRead { get; set; }
        long BytesWritten { get; set; }
        int ErrorCount { get; set; }
        double Kbps { get; set; }
        DateTime StartTime { get; set; }
    }
}