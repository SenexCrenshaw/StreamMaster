namespace StreamMasterApplication.Common.Models;

public class StreamingStatistics
{
    public StreamingStatistics(string ClientAgent, string ClientIPAddress)
    {
        BytesRead = 0;
        StartTime = DateTimeOffset.UtcNow;
        this.ClientAgent = ClientAgent;
        this.ClientIPAddress = ClientIPAddress;
    }

    public double ReadBitsPerSecond
    {
        get
        {
            double elapsedTimeInSeconds = ElapsedTime.TotalSeconds;
            return elapsedTimeInSeconds > 0 ? BytesRead * 8 / elapsedTimeInSeconds : 0;
        }
    }

    public long BytesRead { get; set; }

    public string ClientAgent { get; set; }
    public string ClientIPAddress { get; set; }
    public TimeSpan ElapsedTime => DateTimeOffset.UtcNow - StartTime;
    public DateTimeOffset StartTime { get; set; }

    public void AddBytesRead(long bytesRead)
    {
        BytesRead += bytesRead;
    }

    public void IncrementBytesRead()
    {
        BytesRead++;
    }

}
