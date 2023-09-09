using StreamMasterDomain.Attributes;
using StreamMasterDomain.Mappings;

namespace StreamMasterDomain.Dto;

[RequireAll]
public class ChannelGroupDto : ChannelGroupArg, IMapFrom<ChannelGroup>
{
    //[Required]
    public int StreamCount { get; set; }
}