using MediatR;

namespace StreamMasterDomain.Dto;


public record UpdateChannelGroupRequest(int ChannelGroupId, string? NewGroupName, bool? IsHidden, bool? ToggleVisibility) : IRequest<ChannelGroupDto> { }
