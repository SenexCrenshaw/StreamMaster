using MediatR;

namespace StreamMasterDomain.Dto;

public record UpdateChannelGroupRequest(string GroupName, string? NewGroupName, bool? IsHidden, int? Rank, string? Regex) : IRequest<ChannelGroupDto?>
{
}
