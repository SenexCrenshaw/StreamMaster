using MediatR;

namespace StreamMasterDomain.Dto;

public record UpdateChannelGroupRequest(string ChannelGroupName, string? NewGroupName, bool? IsHidden, int? Rank, string? Regex) : IRequest<ChannelGroupDto?>
{
}
