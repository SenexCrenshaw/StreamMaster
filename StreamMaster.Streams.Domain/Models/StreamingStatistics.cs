namespace StreamMaster.Streams.Domain.Models;

public class StreamingStatistics
{
    public string ChannelName { get; set; }
    public StreamingStatistics(IClientStreamerConfiguration streamerConfiguration)
    {
        BytesRead = 0;
        StartTime = DateTime.UtcNow;
        ClientAgent = streamerConfiguration.ClientUserAgent;
        ClientIPAddress = streamerConfiguration.ClientIPAddress;
        ChannelName = streamerConfiguration.ChannelName;
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
