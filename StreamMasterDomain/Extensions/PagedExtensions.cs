using AutoMapper;

using StreamMasterDomain.Filtering;
using StreamMasterDomain.Pagination;

namespace StreamMasterDomain.Extensions;

public static class PagedExtensions
{
    public static async Task<PagedResponse<TDto>> GetPagedResponseAsync<T, TDto>(this IQueryable<T> query, int pageNumber, int pageSize, IMapper mapper)
    where TDto : class
    {
        IPagedList<T> pagedResult = await query.ToPagedListAsync(pageNumber, pageSize).ConfigureAwait(false);
        PagedResponse<TDto> childQDto = pagedResult.ToPagedResponseDto<T, TDto>(mapper);
        return childQDto;
    }

    public static async Task<PagedResponse<T>> GetPagedResponseAsync<T>(this IQueryable<T> query, int pageNumber, int pageSize) where T : class
    {
        IPagedList<T> pagedResult = await query.ToPagedListAsync(pageNumber, pageSize).ConfigureAwait(false);
        PagedResponse<T> childQDto = pagedResult.ToPagedResponse();
        return childQDto;
    }

    public static async Task<PagedResponse<TDto>> GetPagedResponseWithFilterAsync<T, TDto>(this IQueryable<T> query, string? JSONFiltersString, string OrderBy, int pageNumber, int pageSize, IMapper mapper) where T : class
   where TDto : class
    {
        if (!string.IsNullOrEmpty(JSONFiltersString) || !string.IsNullOrEmpty(OrderBy))
        {
            if (!string.IsNullOrEmpty(JSONFiltersString))
            {
                List<DataTableFilterMetaData> filters = Utils.GetFiltersFromJSON(JSONFiltersString);
                query = FilterHelper<T>.ApplyFiltersAndSort(query, filters, OrderBy);
            }
        }

        return await query.GetPagedResponseAsync<T, TDto>(pageNumber, pageSize, mapper);
    }
    public static PagedResponse<PagedT> CreateEmptyPagedResponse<PagedT>(int count = 0) where PagedT : new()
    {
        return new PagedResponse<PagedT>
        {
            PageNumber = 0,
            TotalPageCount = 0,
            PageSize = 0,
            TotalItemCount = count,
            Data = new List<PagedT>()
        };
    }

}
