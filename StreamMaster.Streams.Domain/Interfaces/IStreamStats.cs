namespace StreamMaster.Streams.Domain.Interfaces;
public interface IStreamStats
{
    double GetAverageLatency();
    long GetBytesRead();
    double GetKbps();
    DateTime GetStartTime();
}