using StreamMaster.Domain.Attributes;

namespace StreamMaster.Application.Common.Models;

[RequireAll]
public class VideoStreamIdRank
{
    public int Rank { get; set; }
    public int VideoStreamId { get; set; }
}
