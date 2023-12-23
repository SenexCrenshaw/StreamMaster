using MediatR;

using StreamMasterDomain.Attributes;

namespace StreamMasterDomain.Requests;

[RequireAll]
public record CreateStreamGroupRequest(string Name) : IRequest
{
}
