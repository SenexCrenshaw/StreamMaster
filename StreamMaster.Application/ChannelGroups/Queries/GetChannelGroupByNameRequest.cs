namespace StreamMaster.Application.ChannelGroups.Queries;


public record GetChannelGroupByNameRequest(string Name) : IRequest<ChannelGroupDto?>;

internal class GetChannelGroupByNameHandler(IRepositoryWrapper Repository, IMapper Mapper, IMemoryCache MemoryCache)
    : IRequestHandler<GetChannelGroupByNameRequest, ChannelGroupDto?>
{
    public async Task<ChannelGroupDto?> Handle(GetChannelGroupByNameRequest request, CancellationToken cancellationToken)
    {
        ChannelGroup? channelGroup = await Repository.ChannelGroup.GetChannelGroupByName(request.Name).ConfigureAwait(false);
        if (channelGroup == null)
        {
            return null;
        }
        ChannelGroupDto dto = Mapper.Map<ChannelGroupDto>(channelGroup);
        MemoryCache.UpdateChannelGroupWithActives(dto);
        return dto;
    }
}
