using StreamMaster.Domain.Attributes;
using StreamMaster.Domain.Mappings;

namespace StreamMaster.SchedulesDirect.Domain.Dto;

[RequireAll]
public class ProgrammeNameDto : IMapFrom<Programme>
{
    public string Id => Channel;
    public string Channel { get; set; }
    public string ChannelName { get; set; }
    public string DisplayName { get; set; }
}
