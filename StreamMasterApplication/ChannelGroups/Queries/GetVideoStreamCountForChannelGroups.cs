using AutoMapper;

using MediatR;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMasterDomain.Cache;
using StreamMasterDomain.Dto;

namespace StreamMasterApplication.ChannelGroups.Queries;

public record GetVideoStreamCountForChannelGroups() : IRequest<IEnumerable<GetChannelGroupVideoStreamCountResponse>>;

internal class GetVideoStreamCountForChannelGroupsHandler : BaseMemoryRequestHandler, IRequestHandler<GetVideoStreamCountForChannelGroups, IEnumerable<GetChannelGroupVideoStreamCountResponse>>
{
    public GetVideoStreamCountForChannelGroupsHandler(ILogger<GetVideoStreamCountForChannelGroupsHandler> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender, IMemoryCache memoryCache)
        : base(logger, repository, mapper, publisher, sender, memoryCache) { }

    public async Task<IEnumerable<GetChannelGroupVideoStreamCountResponse>> Handle(GetVideoStreamCountForChannelGroups request, CancellationToken cancellationToken)
    {
        return MemoryCache.GetChannelGroupVideoStreamCounts();
    }
}