using StreamMasterDomain.Attributes;
using StreamMasterDomain.Mappings;
using StreamMasterDomain.Repository;

namespace StreamMasterDomain.Dto;

[RequireAll]
public class EPGFilesDto : BaseFileDto, IMapFrom<EPGFile>
{
    public int ChannelCount { get; set; }

    public DateTime EPGStartDate { get; set; }

    public DateTime EPGStopDate { get; set; }
    public int ProgrammeCount { get; set; }
}
