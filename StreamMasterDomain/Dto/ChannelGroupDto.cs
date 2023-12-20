using StreamMasterDomain.Attributes;
using StreamMasterDomain.Models;

namespace StreamMasterDomain.Dto;

[RequireAll]
public class ChannelGroupDto : ChannelGroupArg, IMapFrom<ChannelGroup>
{
    public int Id { get; set; }
}