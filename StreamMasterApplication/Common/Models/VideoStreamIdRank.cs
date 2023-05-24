using StreamMasterDomain.Attributes;

namespace StreamMasterApplication.Common.Models;

[RequireAll]
public class VideoStreamIdRank
{
    public int Rank { get; set; }
    public int VideoStreamId { get; set; }
}
