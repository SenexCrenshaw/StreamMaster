using MediatR;

using StreamMaster.Domain.Attributes;

namespace StreamMaster.Domain.Requests;

[RequireAll]
public record CreateStreamGroupRequest(string Name) : IRequest
{
}
