using StreamMaster.Domain.Mappings;
using StreamMaster.Domain.Models;

using StreamMaster.Domain.Attributes;

namespace StreamMaster.Domain.Dto;

[RequireAll]
public class EPGFileDto : BaseFileDto, IMapFrom<EPGFile>
{
    public string Color { get; set; }
    public int ChannelCount { get; set; }

    public DateTime EPGStartDate { get; set; }

    public DateTime EPGStopDate { get; set; }
    public int ProgrammeCount { get; set; }
}
