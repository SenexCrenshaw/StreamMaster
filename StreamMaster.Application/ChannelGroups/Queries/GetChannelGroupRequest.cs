namespace StreamMaster.Application.ChannelGroups.Queries;

public record GetChannelGroupRequest(int Id) : IRequest<APIResponse<ChannelGroupDto?>>;

internal class GetChannelGroupHandler(IRepositoryWrapper Repository, IMapper Mapper, IMemoryCache MemoryCache)
    : IRequestHandler<GetChannelGroupRequest, APIResponse<ChannelGroupDto?>>
{
    public async Task<APIResponse<ChannelGroupDto?>> Handle(GetChannelGroupRequest request, CancellationToken cancellationToken)
    {
        ChannelGroup? channelGroup = await Repository.ChannelGroup.GetChannelGroupById(request.Id);
        if (channelGroup == null)
        {
            return APIResponse<ChannelGroupDto?>.NotFound;
        }

        ChannelGroupDto dto = Mapper.Map<ChannelGroupDto>(channelGroup);
        MemoryCache.UpdateChannelGroupWithActives(dto);
        return APIResponse<ChannelGroupDto?>.Success(dto);
    }
}