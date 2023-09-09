using AutoMapper.QueryableExtensions;

using Microsoft.EntityFrameworkCore;

namespace StreamMasterApplication.StreamGroupChannelGroups.Queries;

public record GetAllChannelGroupsRequest(int StreamGroupId) : IRequest<List<ChannelGroupDto>>;

internal class GetAllChannelGroupsHandler(ILogger<GetAllChannelGroupsRequest> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext) : BaseMediatorRequestHandler(logger, repository, mapper, publisher, sender, hubContext), IRequestHandler<GetAllChannelGroupsRequest, List<ChannelGroupDto>>
{
    public async Task<List<ChannelGroupDto>> Handle(GetAllChannelGroupsRequest request, CancellationToken cancellationToken)
    {
        List<ChannelGroupDto> channelGroups = await Repository.ChannelGroup.FindAll().ProjectTo<ChannelGroupDto>(Mapper.ConfigurationProvider).ToListAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

        List<int> selectedIds = await Repository.StreamGroupChannelGroup.FindAll().Where(a => a.StreamGroupId == request.StreamGroupId).Select(a => a.ChannelGroupId).ToListAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

        channelGroups = channelGroups
        .OrderBy(a => selectedIds.Contains(a.Id) ? 0 : 1)
        .ThenBy(a => a.Name)
        .ToList();

        return channelGroups;
    }

}