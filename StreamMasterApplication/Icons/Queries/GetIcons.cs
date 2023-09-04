using AutoMapper;

using MediatR;

using Microsoft.Extensions.Caching.Memory;

using StreamMasterDomain.Cache;
using StreamMasterDomain.Dto;
using StreamMasterDomain.Filtering;
using StreamMasterDomain.Pagination;

using System.Text.Json;

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
        if (request.iconFileParameters.PageSize == 0)
        {
            PagedResponse<IconFileDto> emptyResponse = new();
            emptyResponse.TotalItemCount = _memoryCache.GetIcons(_mapper).Count;
            return emptyResponse;
        }

        IQueryable<IconFileDto> icons = _memoryCache.GetIcons(_mapper).AsQueryable();

        if (!string.IsNullOrEmpty(request.iconFileParameters.JSONFiltersString) || !string.IsNullOrEmpty(request.iconFileParameters.OrderBy))
        {
            if (!string.IsNullOrEmpty(request.iconFileParameters.JSONFiltersString))
            {
                List<DataTableFilterMetaData>? filters = JsonSerializer.Deserialize<List<DataTableFilterMetaData>>(request.iconFileParameters.JSONFiltersString);
                icons = FilterHelper<IconFileDto>.ApplyFiltersAndSort(icons, filters, "Name asc");
            }
        }


        IPagedList<IconFileDto> test = await icons.ToPagedListAsync(request.iconFileParameters.PageNumber, request.iconFileParameters.PageSize).ConfigureAwait(false);

        PagedResponse<IconFileDto> pagedResponse = test.ToPagedResponse(test.TotalItemCount);
        return pagedResponse;
    }
}