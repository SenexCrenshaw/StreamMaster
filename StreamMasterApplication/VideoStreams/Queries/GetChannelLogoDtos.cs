using MediatR;

using Microsoft.Extensions.Caching.Memory;

using StreamMasterDomain.Dto;

namespace StreamMasterApplication.VideoStreams.Queries;

public record GetChannelLogoDtos : IRequest<List<ChannelLogoDto>>;

internal class GetChannelLogoDtosHandler : IRequestHandler<GetChannelLogoDtos, List<ChannelLogoDto>>
{
    private readonly IMemoryCache _memoryCache;

    public GetChannelLogoDtosHandler(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public async Task<List<ChannelLogoDto>> Handle(GetChannelLogoDtos request, CancellationToken cancellationToken)
    {
        return await Task.FromResult(_memoryCache.ChannelLogos());
    }
}