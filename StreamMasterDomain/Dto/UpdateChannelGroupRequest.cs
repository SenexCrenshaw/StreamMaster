using MediatR;

namespace StreamMasterDomain.Dto;

#if HAS_REGEX
public record UpdateChannelGroupRequest(string ChannelGroupName, string? NewGroupName, bool? IsHidden, int? Rank, string? Regex) : IRequest{}
#else
public record UpdateChannelGroupRequest(string ChannelGroupName, string? NewGroupName, bool? IsHidden, int? Rank) : IRequest { }
#endif