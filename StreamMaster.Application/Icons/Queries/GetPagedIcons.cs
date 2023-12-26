using StreamMaster.Domain.Cache;
using StreamMaster.Domain.Common;
using StreamMaster.Domain.Dto;
using StreamMaster.Domain.Filtering;
using StreamMaster.Domain.Pagination;

using System.Text.Json;

namespace StreamMaster.Application.Icons.Queries;

public record GetPagedIcons(IconFileParameters iconFileParameters) : IRequest<PagedResponse<IconFileDto>>;

internal class GetPagedIconsHandler : IRequestHandler<GetPagedIcons, PagedResponse<IconFileDto>>
{
    private readonly IMapper _mapper;
    private readonly IMemoryCache _memoryCache;

    public GetPagedIconsHandler(IMemoryCache memoryCache, IMapper mapper)
    {
        _memoryCache = memoryCache;
        _mapper = mapper;
    }

    public async Task<PagedResponse<IconFileDto>> Handle(GetPagedIcons request, CancellationToken cancellationToken)
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

        PagedResponse<IconFileDto> pagedResponse = test.ToPagedResponse();
        return pagedResponse;
    }
}