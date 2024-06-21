using StreamMaster.Domain.Extensions;

using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace StreamMaster.Streams.Domain.Models;

public class ClientStreamingStatistics
{
    [JsonIgnore]
    [XmlIgnore]
    public IClientStreamerConfiguration StreamerConfiguration { get; set; }

    public ClientStreamingStatistics(IClientStreamerConfiguration StreamerConfiguration)
    {
        BytesRead = 0;
        StartTime = SMDT.UtcNow;
        this.StreamerConfiguration = StreamerConfiguration;
        ClientIPAddress = StreamerConfiguration.ClientIPAddress;
    }

    public double ReadBitsPerSecond
    {
        get
        {
            TimeSpan elapsedTime = SMDT.UtcNow - StartTime;
            double elapsedTimeInSeconds = elapsedTime.TotalSeconds;
            return elapsedTimeInSeconds > 0 ? BytesRead * 8 / elapsedTimeInSeconds : 0;
        }
    }

    public long BytesRead { get; set; }
    public string ChannelName => StreamerConfiguration.SMChannel.Name;

    //public string VideoStreamName => StreamerConfiguration.snmc
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
