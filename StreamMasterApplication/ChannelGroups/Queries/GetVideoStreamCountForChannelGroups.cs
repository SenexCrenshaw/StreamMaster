using AutoMapper;

using MediatR;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMasterDomain.Dto;

namespace StreamMasterApplication.ChannelGroups.Queries;

public record GetChannelGroupsVideoStreamCount() : IRequest<IEnumerable<ChannelGroupStreamCount>>;

internal class GetChannelGroupsVideoStreamCountHandler : BaseMemoryRequestHandler, IRequestHandler<GetChannelGroupsVideoStreamCount, IEnumerable<ChannelGroupStreamCount>>
{
    public GetChannelGroupsVideoStreamCountHandler(ILogger<GetChannelGroupsVideoStreamCountHandler> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender, IMemoryCache memoryCache)
        : base(logger, repository, mapper, publisher, sender, memoryCache) { }

    public async Task<IEnumerable<ChannelGroupStreamCount>> Handle(GetChannelGroupsVideoStreamCount request, CancellationToken cancellationToken)
    {
        return Repository.ChannelGroup.GetChannelGroupVideoStreamCounts();
    }
}