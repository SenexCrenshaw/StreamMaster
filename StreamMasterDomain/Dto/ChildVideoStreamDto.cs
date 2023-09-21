using StreamMasterDomain.Attributes;
using StreamMasterDomain.Mappings;
using StreamMasterDomain.Models;
using System.ComponentModel.DataAnnotations;

namespace StreamMasterDomain.Dto;

[RequireAll]
public class ChildVideoStreamDto : BaseVideoStreamDto, IMapFrom<VideoStream>, IMapFrom<VideoStreamDto>
{
    [Required]
    public int MaxStreams { get; set; }

    [Required]
    public int Rank { get; set; }
}
