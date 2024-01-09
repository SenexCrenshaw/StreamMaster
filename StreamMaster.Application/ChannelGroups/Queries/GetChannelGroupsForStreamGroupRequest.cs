namespace StreamMaster.Application.ChannelGroups.Queries;

public record GetChannelGroupsForStreamGroupRequest(int StreamGroupId) : IRequest<List<ChannelGroupDto>>;



[LogExecutionTimeAspect]
public class GetChannelGroupsForStreamGroupRequestHandler(ILogger<GetChannelGroupsForStreamGroupRequest> logger, IRepositoryWrapper Repository, IMapper Mapper, IMemoryCache MemoryCache)
    : IRequestHandler<GetChannelGroupsForStreamGroupRequest, List<ChannelGroupDto>>
{
    public async Task<List<ChannelGroupDto>> Handle(GetChannelGroupsForStreamGroupRequest request, CancellationToken cancellationToken)
    {
        List<ChannelGroup> ret = await Repository.ChannelGroup.GetChannelGroupsForStreamGroup(request.StreamGroupId, cancellationToken);

        List<ChannelGroupDto> dtos = Mapper.Map<List<ChannelGroupDto>>(ret);
        MemoryCache.UpdateChannelGroupsWithActives(dtos);
        return dtos;

    }

}