using MediatR;

using StreamMaster.Domain.Attributes;

namespace StreamMaster.Domain.Requests;


[RequireAll]
public record GetStreamGroupEPG(int StreamGroupId) : IRequest<string>;