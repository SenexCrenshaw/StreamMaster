namespace StreamMaster.Application.Common.Models;

public class StreamStatisticsResult
{
    public string Id { get; set; }
    public string CircularBufferId { get; set; }
    public string ClientAgent { get; set; }
    public double ClientBitsPerSecond { get; set; }
    public long ClientBytesRead { get; set; }
    public long ClientBytesWritten { get; set; }
    public string ClientElapsedTime => GetClientElapsedTimeFormatted();
    public Guid ClientId { get; set; }
    public DateTimeOffset ClientStartTime { get; set; }
    public double InputBitsPerSecond { get; set; }
    public long InputBytesRead { get; set; }
    public long InputBytesWritten { get; set; }
    public string InputElapsedTime => GetInputElapsedTimeFormatted();
    public DateTimeOffset InputStartTime { get; set; }
    public string? Logo { get; set; }

    public string GetInputElapsedTimeFormatted()
    {
        TimeSpan elapsedTime = DateTimeOffset.UtcNow - InputStartTime;
        return $"{elapsedTime.Days} {elapsedTime.Hours:00}:{elapsedTime.Minutes:00}:{elapsedTime.Seconds:00}";
    }

    public string GetClientElapsedTimeFormatted()
    {
        TimeSpan elapsedTime = DateTimeOffset.UtcNow - ClientStartTime;
        return $"{elapsedTime.Days} {elapsedTime.Hours:00}:{elapsedTime.Minutes:00}:{elapsedTime.Seconds:00}";
    }


    /// <summary>
    /// Gets or sets the type of the streaming proxy.
    /// </summary>
    public StreamingProxyTypes M3UStreamingProxyType { get; set; }

    public int Rank { get; set; }
    public string? StreamUrl { get; set; }
    public string VideoStreamId { get; set; }
    public string ChannelName { get; set; }
    public string VideoStreamName { get; set; }
    public string ClientIPAddress { get; set; }
    public string ChannelId { get; set; }
}

public class StreamInfo
{
    public string? Logo { get; set; }

    ///// <summary>
    ///// Gets or sets the identifier for the M3U stream.
    ///// </summary>
    //public string M3UStreamId { get; set; }

    ///// <summary>
    ///// Gets or sets the name of the M3U stream.
    ///// </summary>
    //public string M3UStreamName { get; set; } = string.Empty;

    public int Rank { get; set; }

    /// <summary>
    /// Gets or sets the type of the streaming proxy.
    /// </summary>
    public StreamingProxyTypes StreamingProxyType { get; set; }

    public string? StreamUrl { get; set; }

    public string VideoStreamId { get; set; }
    public string VideoStreamName { get; set; }

    public string ChannelId { get; set; }
    public string ChannelName { get; set; }
}
