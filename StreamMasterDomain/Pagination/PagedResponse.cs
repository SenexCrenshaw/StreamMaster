using AutoMapper;

using StreamMasterDomain.Attributes;

using System.Xml.Serialization;

namespace StreamMasterDomain.Pagination;

[RequireAll]
public class PagedResponse<T>
{
    [XmlIgnore]
    public List<T> Data { get; set; } = new List<T>();
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    //public int TotalItemCount { get; set; }
    public int TotalPageCount { get; set; }
    public int TotalItemCount { get; set; }
    public int First { get; set; }
}

public static class PagedListExtensions
{
    public static PagedResponse<TDto> ToPagedResponseDto<T, TDto>(this IPagedList<T> pagedList, IMapper mapper)
    {
        int first = (pagedList.PageNumber - 1) * pagedList.PageSize;
        return new PagedResponse<TDto>
        {
            Data = mapper.Map<List<TDto>>(pagedList),
            First = first,
            PageNumber = pagedList.PageNumber,
            PageSize = pagedList.PageSize,
            TotalPageCount = pagedList.PageCount,
            TotalItemCount = pagedList.TotalItemCount
        };
    }
    public static PagedResponse<T> ToPagedResponse<T>(this IPagedList<T> pagedList, int totalRecords)
    {
        int first = (pagedList.PageNumber - 1) * pagedList.PageSize;
        return new PagedResponse<T>
        {
            Data = pagedList.ToList(),
            First = first,
            PageNumber = pagedList.PageNumber,
            PageSize = pagedList.PageSize,
            TotalPageCount = pagedList.PageCount,
            TotalItemCount = pagedList.TotalItemCount
        };
    }
}