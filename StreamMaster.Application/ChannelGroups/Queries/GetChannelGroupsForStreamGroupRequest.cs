using StreamMaster.Domain.Cache;
using StreamMaster.Domain.Dto;
using StreamMaster.Domain.Models;
using StreamMaster.Domain.Repository;
using StreamMaster.Domain.Services;

namespace StreamMaster.Application.ChannelGroups.Queries;

public record GetChannelGroupsForStreamGroupRequest(int StreamGroupId) : IRequest<List<ChannelGroupDto>>;



[LogExecutionTimeAspect]
public class GetChannelGroupsForStreamGroupRequestHandler(ILogger<GetChannelGroupsForStreamGroupRequest> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache) : BaseMediatorRequestHandler(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache), IRequestHandler<GetChannelGroupsForStreamGroupRequest, List<ChannelGroupDto>>
{
    public async Task<List<ChannelGroupDto>> Handle(GetChannelGroupsForStreamGroupRequest request, CancellationToken cancellationToken)
    {
        List<ChannelGroup> ret = await Repository.ChannelGroup.GetChannelGroupsForStreamGroup(request.StreamGroupId, cancellationToken);

        List<ChannelGroupDto> dtos = Mapper.Map<List<ChannelGroupDto>>(ret);
        MemoryCache.UpdateChannelGroupsWithActives(dtos);
        return dtos;

    }

}