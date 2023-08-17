using StreamMasterDomain.Mappings;
using StreamMasterDomain.Repository.EPG;

namespace StreamMasterDomain.Dto;

public class ProgrammeNameDto : IMapFrom<Programme>
{
    public string Channel { get; set; }
    public string ChannelName { get; set; }
    public string DisplayName { get; set; }
}
