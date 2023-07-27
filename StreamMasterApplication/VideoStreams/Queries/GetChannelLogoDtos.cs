using MediatR;

using Microsoft.Extensions.Caching.Memory;

using StreamMasterDomain.Dto;

namespace StreamMasterApplication.VideoStreams.Queries;

public record GetChannelLogoDtos : IRequest<IEnumerable<ChannelLogoDto>>;

internal class GetChannelLogoDtosHandler : IRequestHandler<GetChannelLogoDtos, IEnumerable<ChannelLogoDto>>
{
    private readonly IMemoryCache _memoryCache;

    public GetChannelLogoDtosHandler(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public async Task<IEnumerable<ChannelLogoDto>> Handle(GetChannelLogoDtos request, CancellationToken cancellationToken)
    {
        return await Task.FromResult(_memoryCache.ChannelLogos());
    }
}
