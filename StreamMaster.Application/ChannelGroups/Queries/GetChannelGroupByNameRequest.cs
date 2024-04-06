namespace StreamMaster.Application.ChannelGroups.Queries;


public record GetChannelGroupByNameRequest(string Name) : IRequest<DataResponse<ChannelGroupDto?>>;

internal class GetChannelGroupByNameHandler(IRepositoryWrapper Repository, IMapper Mapper, IMemoryCache MemoryCache)
    : IRequestHandler<GetChannelGroupByNameRequest, DataResponse<ChannelGroupDto?>>
{
    public async Task<DataResponse<ChannelGroupDto?>> Handle(GetChannelGroupByNameRequest request, CancellationToken cancellationToken)
    {
        ChannelGroup? channelGroup = await Repository.ChannelGroup.GetChannelGroupByName(request.Name).ConfigureAwait(false);
        if (channelGroup == null)
        {
            return DataResponse<ChannelGroupDto?>.NotFound;
        }
        ChannelGroupDto dto = Mapper.Map<ChannelGroupDto>(channelGroup);
        MemoryCache.UpdateChannelGroupWithActives(dto);
        return DataResponse<ChannelGroupDto?>.Success(dto);
    }
}
