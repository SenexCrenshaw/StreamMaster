using StreamMaster.Domain.Cache;
using StreamMaster.Domain.Dto;
using StreamMaster.Domain.Models;
using StreamMaster.Domain.Repository;
using StreamMaster.Domain.Services;

namespace StreamMaster.Application.ChannelGroups.Queries;

public record GetChannelGroup(int Id) : IRequest<ChannelGroupDto?>;

internal class GetChannelGroupHandler(ILogger<GetChannelGroup> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache) : BaseMediatorRequestHandler(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache), IRequestHandler<GetChannelGroup, ChannelGroupDto?>
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