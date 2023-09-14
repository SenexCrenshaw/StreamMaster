using MediatR;

namespace StreamMasterDomain.Dto;


public record UpdateChannelGroupRequest(int ChannelGroupId, string? NewGroupName, bool? IsHidden, int? Rank, bool? ToggleVisibility) : IRequest<ChannelGroupDto> { }
