namespace StreamMasterApplication.VideoStreams.Queries;

public record GetChannelLogoDtos : IRequest<List<ChannelLogoDto>>;

[LogExecutionTimeAspect]
internal class GetChannelLogoDtosHandler(IMemoryCache memoryCache) : IRequestHandler<GetChannelLogoDtos, List<ChannelLogoDto>>
{
    public async Task<List<ChannelLogoDto>> Handle(GetChannelLogoDtos request, CancellationToken cancellationToken)
    {
        return await Task.FromResult(memoryCache.ChannelLogos());
    }
}