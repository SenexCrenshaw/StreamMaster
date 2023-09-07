using MediatR;

namespace StreamMasterDomain.Dto;
public record UpdateStreamGroupRequest(int StreamGroupId, string? Name) : IRequest<StreamGroupDto?>
{
}
