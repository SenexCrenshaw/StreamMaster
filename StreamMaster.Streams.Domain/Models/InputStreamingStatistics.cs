namespace StreamMaster.Streams.Domain.Models;

public class InputStreamingStatistics : IInputStreamingStatistics
{
    public InputStreamingStatistics(StreamInfo StreamInfo)
    {
        this.StreamInfo = StreamInfo;

        BytesRead = 0;
        BytesWritten = 0;
        StartTime = DateTimeOffset.UtcNow;
        Logo = StreamInfo.Logo;
    }

    public InputStreamingStatistics() { }

    public StreamInfo StreamInfo;

    public double BitsPerSecond
    {
        get
        {
            TimeSpan elapsedTime = DateTimeOffset.UtcNow - StartTime;
            double elapsedTimeInSeconds = elapsedTime.TotalSeconds;
            return elapsedTimeInSeconds > 0 ? (BytesRead + BytesWritten) * 8 / elapsedTimeInSeconds : 0;
        }
    }

    private string GetElapsedTimeFormatted()
    {
        TimeSpan elapsedTime = DateTimeOffset.UtcNow - StartTime;
        return $"{elapsedTime.Days} {elapsedTime.Hours:00}:{elapsedTime.Minutes:00}:{elapsedTime.Seconds:00}";
    }

    public int Rank => StreamInfo.Rank;
    public string? StreamUrl => StreamInfo.StreamUrl;
    public long BytesRead { get; set; }
    public long BytesWritten { get; set; }

    public string ElapsedTime => GetElapsedTimeFormatted();
    public DateTimeOffset StartTime { get; set; }
    public string Id => StreamInfo.VideoStreamId;
    public string ChannelName => StreamInfo.ChannelName;
    public string ChannelId => StreamInfo.ChannelId;
    public string? Logo { get; set; }
    public int Clients { get; set; } = 0;

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
        BytesRead++;
    }

    public void IncrementBytesWritten()
    {
        BytesWritten++;
    }

    public void DecrementClient()
    {
        --Clients;
    }

    public void IncrementClient()
    {
        ++Clients;
    }
}
