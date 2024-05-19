namespace StreamMaster.Application.ChannelGroups.Queries;

public record GetChannelGroupRequest(int Id) : IRequest<DataResponse<ChannelGroupDto?>>;

internal class GetChannelGroupHandler(IRepositoryWrapper Repository, IMapper Mapper, IMemoryCache MemoryCache)
    : IRequestHandler<GetChannelGroupRequest, DataResponse<ChannelGroupDto?>>
{
    public async Task<DataResponse<ChannelGroupDto?>> Handle(GetChannelGroupRequest request, CancellationToken cancellationToken)
    {
        ChannelGroup? channelGroup = await Repository.ChannelGroup.GetChannelGroupById(request.Id);
        if (channelGroup == null)
        {
            return DataResponse<ChannelGroupDto?>.NotFound;
        }

        ChannelGroupDto dto = Mapper.Map<ChannelGroupDto>(channelGroup);

        return DataResponse<ChannelGroupDto?>.Success(dto);
    }
}