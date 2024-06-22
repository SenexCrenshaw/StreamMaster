using MessagePack;

using Reinforced.Typings.Attributes;

using StreamMaster.Domain.Extensions;

namespace StreamMaster.Streams.Domain.Models;

[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public class InputStreamingStatistics : IInputStreamingStatistics
{
    //private readonly StreamInfo streamInfo;

    //public InputStreamingStatistics()
    //{
    //    this.streamInfo = streamInfo ?? throw new ArgumentNullException(nameof(streamInfo));
    //    BytesRead = 0;
    //    BytesWritten = 0;
    //    StartTime = SMDT.UtcNow;
    //    Rank = streamInfo.Rank;
    //    StreamUrl = streamInfo.SMStream.Url;
    //    ChannelName = streamInfo.SMChannel.Name;
    //    ChannelId = streamInfo.SMChannel.Id;
    //    Id = streamInfo.SMStream.Id;
    //    ChannelLogo = streamInfo.SMChannel.Logo;
    //    StreamName = streamInfo.SMStream.Name;
    //    StreamId = streamInfo.SMStream.Id;
    //    StreamLogo = streamInfo.SMStream.Logo;
    //}

    public InputStreamingStatistics() { }


    [IgnoreMember]
    public StreamInfo StreamInfo { get; set; }


    public double BitsPerSecond { get; set; }

    public int Rank { get; set; }

    public string? StreamUrl { get; set; }
    public long BytesRead { get; set; }
    public long BytesWritten { get; set; }

    public DateTimeOffset StartTime { get; set; }
    public string Id { get; set; }
    public string ChannelName { get; set; }
    public int ChannelId { get; set; }

    public string? ChannelLogo { get; set; }

    public string StreamName { get; set; }
    public string StreamId { get; set; }
    public string? StreamLogo { get; set; }

    public int Clients { get; set; }

    public string ElapsedTime { get; set; }

    public void UpdateValues()
    {

        TimeSpan elapsedTime = SMDT.UtcNow - StartTime;
        double elapsedTimeInSeconds = elapsedTime.TotalSeconds;
        BitsPerSecond = elapsedTimeInSeconds > 0 ? (BytesRead + BytesWritten) * 8 / elapsedTimeInSeconds : 0;

        ElapsedTime = $"{elapsedTime.Days} {elapsedTime.Hours:00}:{elapsedTime.Minutes:00}:{elapsedTime.Seconds:00}";
    }

    public void AddBytesRead(long bytesRead)
    {
        this.BytesRead += bytesRead;
    }

    public void AddBytesWritten(long bytesWritten)
    {
        BytesWritten += bytesWritten;
    }

    public void IncrementBytesRead()
    {
        ++BytesRead;
    }

    public void IncrementBytesWritten()
    {
        ++BytesWritten;
    }

    public void DecrementClient()
    {
        --Clients;
    }

    public void IncrementClient()
    {
        ++Clients;
    }

    public void SetStreamInfo(StreamInfo streamInfo)
    {
        StreamInfo = streamInfo;
        BytesRead = 0;
        BytesWritten = 0;
        StartTime = SMDT.UtcNow;
        Rank = streamInfo.Rank;
        StreamUrl = streamInfo.SMStream.Url;
        ChannelName = streamInfo.SMChannel.Name;
        ChannelId = streamInfo.SMChannel.Id;
        Id = streamInfo.SMStream.Id;
        ChannelLogo = streamInfo.SMChannel.Logo;
        StreamName = streamInfo.SMStream.Name;
        StreamId = streamInfo.SMStream.Id;
        StreamLogo = streamInfo.SMStream.Logo;
    }
}
