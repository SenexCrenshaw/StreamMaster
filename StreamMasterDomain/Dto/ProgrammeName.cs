using StreamMasterDomain.Attributes;
using StreamMasterDomain.Mappings;
using StreamMasterDomain.Repository.EPG;

namespace StreamMasterDomain.Dto;

[RequireAll]
public class ProgrammeNameDto : IMapFrom<Programme>
{
    public string Id => ChannelName;
    public string Channel { get; set; }
    public string ChannelName { get; set; }
    public string DisplayName { get; set; }
}
