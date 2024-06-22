using MessagePack;

using Reinforced.Typings.Attributes;

using StreamMaster.Domain.Extensions;

using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace StreamMaster.Streams.Domain.Models;
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public class ClientStreamingStatistics
{
    [JsonIgnore]
    [XmlIgnore]
    [IgnoreMember]
    public ClientStreamerConfiguration StreamerConfiguration { get; set; }


    public void UpdateValues()
    {

        TimeSpan elapsedTime = SMDT.UtcNow - StartTime;
        double elapsedTimeInSeconds = elapsedTime.TotalSeconds;
        ReadBitsPerSecond = elapsedTimeInSeconds > 0 ? BytesRead * 8 / elapsedTimeInSeconds : 0;

        ElapsedTime = GetElapsedTimeFormatted();



    }

    public void SetStreamerConfiguration(ClientStreamerConfiguration StreamerConfiguration)
    {
        this.StreamerConfiguration = StreamerConfiguration;
        BytesRead = 0;
        StartTime = SMDT.UtcNow;
        this.StreamerConfiguration = StreamerConfiguration;
        ClientIPAddress = StreamerConfiguration.ClientIPAddress;
        ChannelName = StreamerConfiguration.SMChannel.Name;
        ChannelId = StreamerConfiguration.SMChannel.Id;
        ClientAgent = StreamerConfiguration.ClientUserAgent;
        ClientId = StreamerConfiguration.ClientId;
    }

    public double ReadBitsPerSecond { get; set; }

    //public string StreamName { get; set; }
    //public string StreamId { get; set; }

    public long BytesRead { get; set; }
    public string ChannelName { get; set; } = string.Empty;
    public int ChannelId { get; set; }

    public Guid ClientId { get; set; }
    public string ClientAgent { get; set; } = string.Empty;
    public string ClientIPAddress { get; set; } = string.Empty;
    public string ElapsedTime { get; set; } = string.Empty;
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
