using AutoMapper;

using MediatR;

using Microsoft.Extensions.Caching.Memory;

using StreamMasterDomain.Dto;
using StreamMasterDomain.Pagination;

namespace StreamMasterApplication.Icons.Queries;

public record GetIcons(IconFileParameters iconFileParameters) : IRequest<IPagedList<IconFileDto>>;

internal class GetIconsQueryHandler : IRequestHandler<GetIcons, IPagedList<IconFileDto>>
{
    private readonly IMapper _mapper;
    private readonly IMemoryCache _memoryCache;

    public GetIconsQueryHandler(IMemoryCache memoryCache, IMapper mapper)
    {
        _memoryCache = memoryCache;
        _mapper = mapper;
    }

    public async Task<IPagedList<IconFileDto>> Handle(GetIcons request, CancellationToken cancellationToken)
    {
        var icons = _memoryCache.GetIcons(_mapper);
        var test = await icons.ToPagedListAsync(request.iconFileParameters.PageNumber, request.iconFileParameters.PageSize).ConfigureAwait(false);
        return test;
    }
}