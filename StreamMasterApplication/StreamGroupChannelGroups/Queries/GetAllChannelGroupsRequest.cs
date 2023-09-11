using AutoMapper.QueryableExtensions;

using Microsoft.EntityFrameworkCore;

namespace StreamMasterApplication.StreamGroupChannelGroups.Queries;

public record GetAllChannelGroupsRequest(int StreamGroupId) : IRequest<List<ChannelGroupDto>>;



[LogExecutionTimeAspect]
public class GetAllChannelGroupsRequestHandler(ILogger<GetAllChannelGroupsRequest> logger, IRepositoryWrapper repository, IMapper mapper,ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache) : BaseMemoryRequestHandler(logger, repository, mapper,settingsService, publisher, sender, hubContext, memoryCache), IRequestHandler<GetAllChannelGroupsRequest, List<ChannelGroupDto>>
{
    public async Task<List<ChannelGroupDto>> Handle(GetAllChannelGroupsRequest request, CancellationToken cancellationToken)
    {
        List<ChannelGroupDto> channelGroups = await Repository.ChannelGroup.FindAll().ProjectTo<ChannelGroupDto>(Mapper.ConfigurationProvider).ToListAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

        List<int> selectedIds = await Repository.StreamGroupChannelGroup.FindAll().Where(a => a.StreamGroupId == request.StreamGroupId).Select(a => a.ChannelGroupId).ToListAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

        IEnumerable<ChannelGroupStreamCount> actives = MemoryCache.ChannelGroupStreamCounts();

        foreach (ChannelGroupStreamCount? active in actives)
        {
            ChannelGroupDto? dto = channelGroups.FirstOrDefault(a => a.Id == active.Id);
            if (dto == null)
            {
                continue;
            }
            dto.ActiveCount = active.ActiveCount;
            dto.HiddenCount = active.HiddenCount;
            dto.TotalCount = active.TotalCount;

        }

        channelGroups = channelGroups
        .OrderBy(a => selectedIds.Contains(a.Id) ? 0 : 1)
        .ThenBy(a => a.Name)
        .ToList();

        return channelGroups;
    }

}