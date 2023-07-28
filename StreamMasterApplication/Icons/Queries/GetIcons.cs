using AutoMapper;

using MediatR;

using Microsoft.Extensions.Caching.Memory;

using StreamMasterDomain.Dto;

namespace StreamMasterApplication.Icons.Queries;

public record GetIcons : IRequest<List<IconFileDto>>;

internal class GetIconsQueryHandler : IRequestHandler<GetIcons, List<IconFileDto>>
{
    private readonly IMapper _mapper;
    private readonly IMemoryCache _memoryCache;

    public GetIconsQueryHandler(IMemoryCache memoryCache, IMapper mapper)
    {
        _memoryCache = memoryCache;
        _mapper = mapper;
    }

    public async Task<List<IconFileDto>> Handle(GetIcons request, CancellationToken cancellationToken)
    {
        return _memoryCache.GetIcons(_mapper);
    }
}
