using StreamMaster.Domain.Enums;
using StreamMaster.Domain.Models;

namespace StreamMaster.Streams.Domain.Models;

public class StreamInfo
{
    public string? Logo => SMChannel?.Logo;

    public int Rank { get; set; }
    public StreamingProxyTypes StreamingProxyType => SMChannel.StreamingProxyType;

    public string? StreamUrl { get; set; }

    public string VideoStreamId { get; set; }
    public string VideoStreamName { get; set; }

    public SMChannel SMChannel { get; set; }
}
