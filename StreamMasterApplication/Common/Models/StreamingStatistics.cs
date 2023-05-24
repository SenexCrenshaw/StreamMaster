namespace StreamMasterApplication.Common.Models;

public class StreamingStatistics
{
    public StreamingStatistics()
    {
        BytesRead = 0;
        BytesWritten = 0;
        StartTime = DateTimeOffset.UtcNow;
    }

    public double BitsPerSecond
    {
        get
        {
            double elapsedTimeInSeconds = ElapsedTime.TotalSeconds;
            return elapsedTimeInSeconds > 0 ? (BytesRead + BytesWritten) * 8 / elapsedTimeInSeconds : 0;
        }
    }

    public long BytesRead { get; set; }
    public long BytesWritten { get; set; }
    public TimeSpan ElapsedTime => DateTimeOffset.UtcNow - StartTime;
    public DateTimeOffset StartTime { get; set; }

    public void AddBytesRead(long bytesRead)
    {
        BytesRead += bytesRead;
    }

    public void AddBytesWritten(long bytesWritten)
    {
        BytesWritten += bytesWritten;
    }

    public void IncrementBytesRead()
    {
        BytesRead += 1;
    }

    public void IncrementBytesWritten()
    {
        BytesWritten += 1;
    }
}
