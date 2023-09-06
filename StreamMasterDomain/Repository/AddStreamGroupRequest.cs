using MediatR;

using StreamMasterDomain.Attributes;
using StreamMasterDomain.Dto;

namespace StreamMasterDomain.Repository;

[RequireAll]
public record AddStreamGroupRequest(
    string Name,
    int StreamGroupNumber,
    List<VideoStreamIsReadOnly>? VideoStreams,
    List<string>? ChannelGroupNames
    ) : IRequest
{
}
