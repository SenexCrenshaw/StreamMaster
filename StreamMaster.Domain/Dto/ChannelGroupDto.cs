using StreamMaster.Domain.Mappings;
using StreamMaster.Domain.Models;

using StreamMaster.Domain.Attributes;

namespace StreamMaster.Domain.Dto;

[RequireAll]
public class ChannelGroupDto : ChannelGroupArg, IMapFrom<ChannelGroup>
{
    public int Id { get; set; }
}