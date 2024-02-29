namespace StreamMaster.Application.ChannelGroups.Queries;

public record GetChannelGroupByName(string Name) : IRequest<ChannelGroupDto?>;

internal class GetChannelGroupByNameHandler(ILogger<GetChannelGroupByName> logger,
                                            IRepositoryWrapper Repository,
                                            IMapper Mapper,
                                            IMemoryCache MemoryCache)
    : IRequestHandler<GetChannelGroupByName, ChannelGroupDto?>
{
    public async Task<ChannelGroupDto?> Handle(GetChannelGroupByName request, CancellationToken cancellationToken)
    {
        ChannelGroup? channelGroup = await Repository.ChannelGroup.GetChannelGroupByName(request.Name).ConfigureAwait(false);
        ChannelGroupDto? dto = Mapper.Map<ChannelGroupDto?>(channelGroup);
        MemoryCache.UpdateChannelGroupWithActives(dto);
        return dto;
    }
}
