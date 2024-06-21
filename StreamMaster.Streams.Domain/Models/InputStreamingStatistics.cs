using StreamMaster.Domain.Extensions;

namespace StreamMaster.Streams.Domain.Models;

public class InputStreamingStatistics : IInputStreamingStatistics
{
    private readonly StreamInfo streamInfo;
    private long bytesRead;
    private long bytesWritten;
    private int clients;

    public InputStreamingStatistics(StreamInfo streamInfo)
    {
        this.streamInfo = streamInfo ?? throw new ArgumentNullException(nameof(streamInfo));
        bytesRead = 0;
        bytesWritten = 0;
        StartTime = SMDT.UtcNow;

    }

    public StreamInfo StreamInfo => streamInfo;

    public double BitsPerSecond
    {
        get
        {
            TimeSpan elapsedTime = SMDT.UtcNow - StartTime;
            double elapsedTimeInSeconds = elapsedTime.TotalSeconds;
            return elapsedTimeInSeconds > 0 ? (bytesRead + bytesWritten) * 8 / elapsedTimeInSeconds : 0;
        }
    }

    public int Rank => streamInfo.Rank;
    public string? StreamUrl => streamInfo.SMStream.Url;
    public long BytesRead => Interlocked.Read(ref bytesRead);
    public long BytesWritten => Interlocked.Read(ref bytesWritten);
    public DateTimeOffset StartTime { get; }
    public string Id => streamInfo.SMStream.Id;
    public string ChannelName => streamInfo.SMChannel.Name;
    public int ChannelId => streamInfo.SMChannel.Id;

    public string? ChannelLogo => streamInfo.SMChannel.Logo;

    public string StreamName => streamInfo.SMStream.Name;
    public string StreamId => streamInfo.SMStream.Id;
    public string? StreamLogo => streamInfo.SMStream.Logo;

    public int Clients => Interlocked.CompareExchange(ref clients, 0, 0);

    public string ElapsedTime => GetElapsedTimeFormatted();

    private string GetElapsedTimeFormatted()
    {
        TimeSpan elapsedTime = SMDT.UtcNow - StartTime;
        return $"{elapsedTime.Days} {elapsedTime.Hours:00}:{elapsedTime.Minutes:00}:{elapsedTime.Seconds:00}";
    }

    public void AddBytesRead(long bytesRead)
    {
        Interlocked.Add(ref this.bytesRead, bytesRead);
    }

    public void AddBytesWritten(long bytesWritten)
    {
        Interlocked.Add(ref this.bytesWritten, bytesWritten);
    }

    public void IncrementBytesRead()
    {
        Interlocked.Increment(ref this.bytesRead);
    }

    public void IncrementBytesWritten()
    {
        Interlocked.Increment(ref this.bytesWritten);
    }

    public void DecrementClient()
    {
        Interlocked.Decrement(ref this.clients);
    }

    public void IncrementClient()
    {
        Interlocked.Increment(ref this.clients);
    }
}
