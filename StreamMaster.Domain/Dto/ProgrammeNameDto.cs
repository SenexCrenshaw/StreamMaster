using StreamMaster.Domain.Mappings;
using StreamMaster.SchedulesDirect.Domain.JsonClasses;

using StreamMaster.Domain.Attributes;

namespace StreamMaster.Domain.Dto;

[RequireAll]
public class ProgrammeNameDto : IMapFrom<Programme>
{
    public string Id => Channel;
    public string Channel { get; set; }
    public string ChannelName { get; set; }
    public string DisplayName { get; set; }
}
