using AutoMapper.QueryableExtensions;

using Microsoft.EntityFrameworkCore;

namespace StreamMasterApplication.ChannelGroups.Queries;

public record GetAllChannelGroupsRequest() : IRequest<List<ChannelGroupDto>>;

internal class GetAllChannelGroupsHandler(ILogger<GetAllChannelGroupsRequest> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext) : BaseMediatorRequestHandler(logger, repository, mapper, publisher, sender, hubContext), IRequestHandler<GetAllChannelGroupsRequest, List<ChannelGroupDto>>
{
    public async Task<List<ChannelGroupDto>> Handle(GetAllChannelGroupsRequest request, CancellationToken cancellationToken)
    {
        List<ChannelGroupDto> res = await Repository.ChannelGroup.FindAll().ProjectTo<ChannelGroupDto>(Mapper.ConfigurationProvider).ToListAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        return res;
    }

}