﻿using StreamMasterDomain.Dto;

namespace StreamMasterApplication.ChannelGroups.Queries;

public record GetChannelGroupVideoStreamCounts() : IRequest<List<ChannelGroupStreamCount>>;

internal class GetChannelGroupVideoStreamCountsHandler : BaseMediatorRequestHandler, IRequestHandler<GetChannelGroupVideoStreamCounts, List<ChannelGroupStreamCount>>
{

    public GetChannelGroupVideoStreamCountsHandler(ILogger<GetChannelGroupVideoStreamCounts> logger, IRepositoryWrapper repository, IMapper mapper,ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache)
 : base(logger, repository, mapper,settingsService, publisher, sender, hubContext, memoryCache) { }
    public Task<List<ChannelGroupStreamCount>> Handle(GetChannelGroupVideoStreamCounts request, CancellationToken cancellationToken)
    {
        return Task.FromResult(MemoryCache.ChannelGroupStreamCounts());
    }
}