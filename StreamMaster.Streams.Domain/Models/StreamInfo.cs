using StreamMaster.Domain.Enums;

namespace StreamMaster.Streams.Domain.Models;

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
