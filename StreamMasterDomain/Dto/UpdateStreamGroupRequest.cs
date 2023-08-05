using MediatR;

using StreamMasterDomain.Attributes;

namespace StreamMasterDomain.Dto;
public record UpdateStreamGroupRequest(
    int StreamGroupId,
    string? Name,
    int? StreamGroupNumber,
    List<VideoStreamIsReadOnly>? VideoStreams,
    List<string>? ChannelGroupNames
    ) : IRequest<StreamGroupDto?>
{
}
