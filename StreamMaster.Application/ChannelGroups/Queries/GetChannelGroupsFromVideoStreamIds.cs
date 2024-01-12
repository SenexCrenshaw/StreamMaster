namespace StreamMaster.Application.ChannelGroups.Queries;

public record GetChannelGroupsFromVideoStreamIds(IEnumerable<string> VideoStreamIds) : IRequest<List<ChannelGroupDto>>;

internal class GetChannelGroupsFromVideoStreamIdsHandler(ILogger<GetChannelGroupsFromVideoStreamIds> logger, IRepositoryWrapper Repository, IMapper Mapper, IMemoryCache MemoryCache)
    : IRequestHandler<GetChannelGroupsFromVideoStreamIds, List<ChannelGroupDto>>
{
    public async Task<List<ChannelGroupDto>> Handle(GetChannelGroupsFromVideoStreamIds request, CancellationToken cancellationToken)
    {
        List<ChannelGroup> ret = await Repository.ChannelGroup.GetChannelGroupsFromVideoStreamIds(request.VideoStreamIds, cancellationToken).ConfigureAwait(false);
        List<ChannelGroupDto> dtos = Mapper.Map<List<ChannelGroupDto>>(ret);
        MemoryCache.UpdateChannelGroupsWithActives(dtos);
        return dtos;
    }
}