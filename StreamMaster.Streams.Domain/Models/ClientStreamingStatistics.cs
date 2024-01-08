using System.Text.Json.Serialization;

namespace StreamMaster.Streams.Domain.Models;

public class ClientStreamingStatistics
{


    [JsonIgnore]
    public IClientStreamerConfiguration StreamerConfiguration { get; set; }

    public ClientStreamingStatistics(IClientStreamerConfiguration StreamerConfiguration)
    {

        BytesRead = 0;
        StartTime = DateTime.UtcNow;
        this.StreamerConfiguration = StreamerConfiguration;
        ClientIPAddress = StreamerConfiguration.ClientIPAddress;
    }

    public double ReadBitsPerSecond
    {
        get
        {
            TimeSpan elapsedTime = DateTimeOffset.UtcNow - StartTime;
            double elapsedTimeInSeconds = elapsedTime.TotalSeconds;
            return elapsedTimeInSeconds > 0 ? BytesRead * 8 / elapsedTimeInSeconds : 0;
        }
    }

    public long BytesRead { get; set; }
    public string ChannelName => StreamerConfiguration.ChannelName;

    public string VideoStreamName => StreamerConfiguration.VideoStreamName;
    public Guid ClientId => StreamerConfiguration.ClientId;
    public string ClientAgent => StreamerConfiguration.ClientUserAgent;
    public string ClientIPAddress { get; set; }
    public string ElapsedTime => GetElapsedTimeFormatted();
    public DateTimeOffset StartTime { get; set; }

    public void AddBytesRead(long bytesRead)
    {
        BytesRead += bytesRead;
    }

    public void IncrementBytesRead()
    {
        BytesRead++;
    }

    private string GetElapsedTimeFormatted()
    {
        TimeSpan elapsedTime = DateTimeOffset.UtcNow - StartTime;
        return $"{elapsedTime.Days} {elapsedTime.Hours:00}:{elapsedTime.Minutes:00}:{elapsedTime.Seconds:00}";
    }


}
