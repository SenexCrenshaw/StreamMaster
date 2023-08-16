using AutoMapper;

using MediatR;

using Microsoft.Extensions.Caching.Memory;

using StreamMasterDomain.Cache;
using StreamMasterDomain.Dto;

namespace StreamMasterApplication.Icons.Queries;
public record GetIconsSimpleQuery() : IRequest<IEnumerable<IconSimpleDto>>;

internal class GetIconsSimpleQueryHandler : IRequestHandler<GetIconsSimpleQuery, IEnumerable<IconSimpleDto>>
{
    private readonly IMapper _mapper;
    private readonly IMemoryCache _memoryCache;

    public GetIconsSimpleQueryHandler(IMemoryCache memoryCache, IMapper mapper)
    {
        _memoryCache = memoryCache;
        _mapper = mapper;
    }

    public Task<IEnumerable<IconSimpleDto>> Handle(GetIconsSimpleQuery request, CancellationToken cancellationToken)
    {
        List<IconFileDto> icons = _memoryCache.GetIcons(_mapper);
        IEnumerable<IconSimpleDto> ret = _mapper.Map<IEnumerable<IconSimpleDto>>(icons);

        return Task.FromResult(ret);
    }
}