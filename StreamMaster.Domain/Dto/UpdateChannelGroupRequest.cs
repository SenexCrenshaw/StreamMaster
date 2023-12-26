using MediatR;

namespace StreamMaster.Domain.Dto;


public record UpdateChannelGroupRequest(int ChannelGroupId, string? NewGroupName, bool? IsHidden, bool? ToggleVisibility) : IRequest<ChannelGroupDto> { }
