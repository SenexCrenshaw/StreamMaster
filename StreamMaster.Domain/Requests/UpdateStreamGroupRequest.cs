using MediatR;

using StreamMaster.Domain.API;

namespace StreamMaster.Domain.Requests
{
    public record UpdateStreamGroupRequest(int StreamGroupId, string? Name, bool? AutoSetChannelNumbers, List<string>? StreamGroupProfiles)
     : IRequest<APIResponse>
    { }

}
