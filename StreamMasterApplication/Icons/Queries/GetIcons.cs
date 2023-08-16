using AutoMapper;

using MediatR;

using Microsoft.Extensions.Caching.Memory;

using StreamMasterDomain.Cache;
using StreamMasterDomain.Dto;
using StreamMasterDomain.Pagination;

namespace StreamMasterApplication.Icons.Queries;

public record GetIcons(IconFileParameters iconFileParameters) : IRequest<PagedResponse<IconFileDto>>;

internal class GetIconsHandler : IRequestHandler<GetIcons, PagedResponse<IconFileDto>>
{
    private readonly IMapper _mapper;
    private readonly IMemoryCache _memoryCache;

    public GetIconsHandler(IMemoryCache memoryCache, IMapper mapper)
    {
        _memoryCache = memoryCache;
        _mapper = mapper;
    }

    public async Task<PagedResponse<IconFileDto>> Handle(GetIcons request, CancellationToken cancellationToken)
    {
        List<IconFileDto> icons = _memoryCache.GetIcons(_mapper);
        IPagedList<IconFileDto> test = await icons.ToPagedListAsync(request.iconFileParameters.PageNumber, request.iconFileParameters.PageSize).ConfigureAwait(false);

        PagedResponse<IconFileDto> pagedResponse = test.ToPagedResponse(icons.Count);
        return pagedResponse;
    }
}