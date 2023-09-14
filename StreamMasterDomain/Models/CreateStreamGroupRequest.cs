using MediatR;

using StreamMasterDomain.Attributes;

namespace StreamMasterDomain.Models;

[RequireAll]
public record CreateStreamGroupRequest(string Name) : IRequest
{
}
