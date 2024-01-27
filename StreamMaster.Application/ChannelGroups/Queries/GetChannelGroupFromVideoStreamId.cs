namespace StreamMaster.Application.ChannelGroups.Queries;

public record GetChannelGroupFromVideoStreamId(string VideoStreamId) : IRequest<ChannelGroupDto?>;

internal class GetChannelGroupFromVideoStreamIdHandler(ILogger<GetChannelGroupFromVideoStreamId> logger, IRepositoryWrapper Repository, IMapper Mapper, IMemoryCache MemoryCache)
    : IRequestHandler<GetChannelGroupFromVideoStreamId, ChannelGroupDto?>
{
    public async Task<ChannelGroupDto?> Handle(GetChannelGroupFromVideoStreamId request, CancellationToken cancellationToken)
    {
        ChannelGroup? channelGroup = await Repository.ChannelGroup.GetChannelGroupFromVideoStreamId(request.VideoStreamId).ConfigureAwait(false);
        if (channelGroup == null)
        {
            return null;
        }
        ChannelGroupDto? dto = Mapper.Map<ChannelGroupDto?>(channelGroup);
        if (dto == null)
        {
            return null;
        }
        MemoryCache.UpdateChannelGroupWithActives(dto);
        return dto;
    }
}