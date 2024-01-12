namespace StreamMaster.Application.ChannelGroups.Queries;

public record GetChannelGroupVideoStreamCounts() : IRequest<List<ChannelGroupStreamCount>>;

internal class GetChannelGroupVideoStreamCountsHandler(ILogger<GetChannelGroupVideoStreamCounts> logger, IMemoryCache MemoryCache)
    : IRequestHandler<GetChannelGroupVideoStreamCounts, List<ChannelGroupStreamCount>>
{
    public Task<List<ChannelGroupStreamCount>> Handle(GetChannelGroupVideoStreamCounts request, CancellationToken cancellationToken)
    {
        return Task.FromResult(MemoryCache.ChannelGroupStreamCounts());
    }
}