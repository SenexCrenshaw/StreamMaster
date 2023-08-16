using AutoMapper;

using MediatR;

using Microsoft.Extensions.Caching.Memory;

using StreamMasterDomain.Cache;
using StreamMasterDomain.Dto;
using StreamMasterDomain.Pagination;

namespace StreamMasterApplication.Icons.Queries;
public record GetIconsSimpleQuery(IconFileParameters iconFileParameters) : IRequest<IEnumerable<IconFileDto>>;

internal class GetIconsSimpleQueryHandler : IRequestHandler<GetIconsSimpleQuery, IEnumerable<IconFileDto>>
{
    private readonly IMapper _mapper;
    private readonly IMemoryCache _memoryCache;

    public GetIconsSimpleQueryHandler(IMemoryCache memoryCache, IMapper mapper)
    {
        _memoryCache = memoryCache;
        _mapper = mapper;
    }

    public Task<IEnumerable<IconFileDto>> Handle(GetIconsSimpleQuery request, CancellationToken cancellationToken)
    {
        List<IconFileDto> icons = _memoryCache.GetIcons(_mapper).Skip(request.iconFileParameters.First).ToList();
        List<IconFileDto> ficons = icons.Take(request.iconFileParameters.Count).ToList();
        IEnumerable<IconFileDto> ret = _mapper.Map<IEnumerable<IconFileDto>>(ficons);

        return Task.FromResult(ret);
    }
}