namespace StreamMaster.Application.ChannelGroups.Queries;

public record GetChannelGroup(int Id) : IRequest<ChannelGroupDto?>;

internal class GetChannelGroupHandler(ILogger<GetChannelGroup> logger, IRepositoryWrapper Repository, IMapper Mapper, IMemoryCache MemoryCache)
    : IRequestHandler<GetChannelGroup, ChannelGroupDto?>
{
    public async Task<ChannelGroupDto?> Handle(GetChannelGroup request, CancellationToken cancellationToken)
    {
        ChannelGroup? channelGroup = await Repository.ChannelGroup.GetChannelGroupById(request.Id);
        if (channelGroup == null)
        {
            return null;
        }

        ChannelGroupDto? dto = Mapper.Map<ChannelGroupDto?>(channelGroup);
        MemoryCache.UpdateChannelGroupWithActives(dto);
        return dto;
    }
}