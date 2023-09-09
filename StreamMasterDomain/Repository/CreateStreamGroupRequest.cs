using MediatR;

using StreamMasterDomain.Attributes;

namespace StreamMasterDomain.Repository;

[RequireAll]
public record CreateStreamGroupRequest(string Name, int StreamGroupNumber) : IRequest
{
}
