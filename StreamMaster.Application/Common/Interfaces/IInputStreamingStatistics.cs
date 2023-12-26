namespace StreamMaster.Application.Common.Interfaces;

public interface IInputStreamingStatistics
{
    double BitsPerSecond { get; }
    long BytesRead { get; set; }
    long BytesWritten { get; set; }
    TimeSpan ElapsedTime { get; }
    DateTimeOffset StartTime { get; set; }

    void AddBytesRead(long bytesRead);
    void AddBytesWritten(long bytesWritten);
    void IncrementBytesRead();
    void IncrementBytesWritten();
}