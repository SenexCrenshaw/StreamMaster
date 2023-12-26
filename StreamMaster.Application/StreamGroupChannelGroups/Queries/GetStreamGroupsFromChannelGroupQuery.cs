using StreamMaster.Domain.Dto;
using StreamMaster.Domain.Repository;
using StreamMaster.Domain.Services;

namespace StreamMaster.Application.StreamGroupChannelGroups.Queries;

public record GetStreamGroupsFromChannelGroupQuery(int channelGroupId) : IRequest<IEnumerable<StreamGroupDto>>;

[LogExecutionTimeAspect]
internal class GetStreamGroupsFromChannelGroupQueryHandler(ILogger<GetStreamGroupsFromChannelGroupQuery> logger, IRepositoryWrapper repository, IMapper mapper,ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache) : BaseMediatorRequestHandler(logger, repository, mapper,settingsService, publisher, sender, hubContext, memoryCache), IRequestHandler<GetStreamGroupsFromChannelGroupQuery, IEnumerable<StreamGroupDto>>
{
    public async Task<IEnumerable<StreamGroupDto>> Handle(GetStreamGroupsFromChannelGroupQuery request, CancellationToken cancellationToken = default)
    {
        List<StreamGroupDto> ret = await Repository.StreamGroupChannelGroup.GetStreamGroupsFromChannelGroup(request.channelGroupId, cancellationToken).ConfigureAwait(false);

        return ret;

    }
}
