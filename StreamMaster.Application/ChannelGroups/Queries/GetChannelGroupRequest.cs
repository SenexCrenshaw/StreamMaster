namespace StreamMaster.Application.ChannelGroups.Queries;

public record GetChannelGroupRequest(int Id) : IRequest<ChannelGroupDto?>;

internal class GetChannelGroupHandler(IRepositoryWrapper Repository, IMapper Mapper, IMemoryCache MemoryCache)
    : IRequestHandler<GetChannelGroupRequest, ChannelGroupDto?>
{
    public async Task<ChannelGroupDto?> Handle(GetChannelGroupRequest request, CancellationToken cancellationToken)
    {
        ChannelGroup? channelGroup = await Repository.ChannelGroup.GetChannelGroupById(request.Id);
        if (channelGroup == null)
        {
            return null;
        }

        ChannelGroupDto dto = Mapper.Map<ChannelGroupDto>(channelGroup);
        MemoryCache.UpdateChannelGroupWithActives(dto);
        return dto;
    }
}