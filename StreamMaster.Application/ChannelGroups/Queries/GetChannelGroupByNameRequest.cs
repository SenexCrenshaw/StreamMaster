namespace StreamMaster.Application.ChannelGroups.Queries;


public record GetChannelGroupByNameRequest(string Name) : IRequest<APIResponse<ChannelGroupDto?>>;

internal class GetChannelGroupByNameHandler(IRepositoryWrapper Repository, IMapper Mapper, IMemoryCache MemoryCache)
    : IRequestHandler<GetChannelGroupByNameRequest, APIResponse<ChannelGroupDto?>>
{
    public async Task<APIResponse<ChannelGroupDto?>> Handle(GetChannelGroupByNameRequest request, CancellationToken cancellationToken)
    {
        ChannelGroup? channelGroup = await Repository.ChannelGroup.GetChannelGroupByName(request.Name).ConfigureAwait(false);
        if (channelGroup == null)
        {
            return APIResponse<ChannelGroupDto?>.NotFound;
        }
        ChannelGroupDto dto = Mapper.Map<ChannelGroupDto>(channelGroup);
        MemoryCache.UpdateChannelGroupWithActives(dto);
        return APIResponse<ChannelGroupDto?>.Success(dto);
    }
}
