using StreamMaster.Domain.Attributes;
using StreamMaster.Domain.Models;

namespace StreamMaster.Domain.Dto;

[RequireAll]
public class EPGFileDto : BaseFileDto, IMapFrom<EPGFile>
{
    public int EPGNumber { get; set; }
    public string Color { get; set; }
    public int ChannelCount { get; set; }
    public DateTime EPGStartDate { get; set; }
    public DateTime EPGStopDate { get; set; }
    public int ProgrammeCount { get; set; }
}
