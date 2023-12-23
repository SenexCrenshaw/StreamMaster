using MediatR;

namespace StreamMasterDomain.Dto;
public record UpdateStreamGroupRequest(int StreamGroupId, string? Name, bool? AutoSetChannelNumbers) : IRequest<StreamGroupDto?>
{
}
