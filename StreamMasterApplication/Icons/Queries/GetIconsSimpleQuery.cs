using AutoMapper;

using MediatR;

using Microsoft.Extensions.Caching.Memory;

using StreamMasterDomain.Cache;
using StreamMasterDomain.Dto;
using StreamMasterDomain.Pagination;

namespace StreamMasterApplication.Icons.Queries;
public record GetIconsSimpleQuery(IconFileParameters iconFileParameters) : IRequest<IEnumerable<IconSimpleDto>>;

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
        List<IconFileDto> icons = _memoryCache.GetIcons(_mapper).Skip(request.iconFileParameters.First).ToList();
        List<IconFileDto> ficons = icons.Take(request.iconFileParameters.Count).ToList();
        IEnumerable<IconSimpleDto> ret = _mapper.Map<IEnumerable<IconSimpleDto>>(ficons);

        //IPagedList<IconSimpleDto> pagedResult = await ret.ToPagedListAsync(request.iconFileParameters.PageNumber, request.iconFileParameters.PageSize).ConfigureAwait(false);


        return Task.FromResult(ret);
    }
}