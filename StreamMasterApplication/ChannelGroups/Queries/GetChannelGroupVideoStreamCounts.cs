using AutoMapper;

using MediatR;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMasterDomain.Cache;
using StreamMasterDomain.Dto;

namespace StreamMasterApplication.ChannelGroups.Queries;

public record GetChannelGroupVideoStreamCounts() : IRequest<List<ChannelGroupStreamCount>>;

internal class GetChannelGroupVideoStreamCountsHandler : BaseMemoryRequestHandler, IRequestHandler<GetChannelGroupVideoStreamCounts, List<ChannelGroupStreamCount>>
{
    public GetChannelGroupVideoStreamCountsHandler(ILogger<GetChannelGroupVideoStreamCountsHandler> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender, IMemoryCache memoryCache)
        : base(logger, repository, mapper, publisher, sender, memoryCache) { }

    public Task<List<ChannelGroupStreamCount>> Handle(GetChannelGroupVideoStreamCounts request, CancellationToken cancellationToken)
    {
        return Task.FromResult(MemoryCache.ChannelGroupStreamCounts());
    }
}