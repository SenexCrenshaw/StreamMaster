using StreamMaster.Domain.Dto;
using StreamMaster.Domain.Repository;
using StreamMaster.Domain.Services;

namespace StreamMaster.Application.ChannelGroups.Queries;

public record GetChannelGroupIdNames() : IRequest<List<ChannelGroupIdName>>;

internal class GetChannelGroupIdNamesQueryHandler(ILogger<GetChannelGroupIdNames> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache) : BaseMediatorRequestHandler(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache), IRequestHandler<GetChannelGroupIdNames, List<ChannelGroupIdName>>
{
    public async Task<List<ChannelGroupIdName>> Handle(GetChannelGroupIdNames request, CancellationToken cancellationToken)
    {
        List<ChannelGroupIdName> ret = await Repository.ChannelGroup.GetChannelGroupNames(cancellationToken);

        return ret;
    }
}