using AutoMapper;

using MediatR;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMasterDomain.Cache;
using StreamMasterDomain.Dto;

namespace StreamMasterApplication.ChannelGroups.Queries;

public record GetChannelGroupsVideoStreamCount() : IRequest<IEnumerable<GetChannelGroupVideoStreamCountResponse>>;

internal class GetChannelGroupsVideoStreamCountHandler : BaseMemoryRequestHandler, IRequestHandler<GetChannelGroupsVideoStreamCount, IEnumerable<GetChannelGroupVideoStreamCountResponse>>
{
    public GetChannelGroupsVideoStreamCountHandler(ILogger<GetChannelGroupsVideoStreamCountHandler> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender, IMemoryCache memoryCache)
        : base(logger, repository, mapper, publisher, sender, memoryCache) { }

    public async Task<IEnumerable<GetChannelGroupVideoStreamCountResponse>> Handle(GetChannelGroupsVideoStreamCount request, CancellationToken cancellationToken)
    {
        return MemoryCache.GetChannelGroupVideoStreamCounts();
    }
}