using StreamMasterDomain.Attributes;
using StreamMasterDomain.Mappings;

namespace StreamMasterDomain.Dto;

[RequireAll]
public class M3UFilesDto : BaseFileDto, IMapFrom<M3UFile>
{

    public int MaxStreamCount { get; set; }

    public int StationCount { get; set; }
}
