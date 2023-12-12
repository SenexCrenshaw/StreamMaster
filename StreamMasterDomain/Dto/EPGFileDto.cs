using StreamMasterDomain.Attributes;
using StreamMasterDomain.Mappings;
using StreamMasterDomain.Models;

namespace StreamMasterDomain.Dto;

[RequireAll]
public class EPGFileDto : BaseFileDto, IMapFrom<EPGFile>
{
    public string Color { get; set; }
    public int ChannelCount { get; set; }

    public DateTime EPGStartDate { get; set; }

    public DateTime EPGStopDate { get; set; }
    public int ProgrammeCount { get; set; }
}
