using AutoMapper;

using Reinforced.Typings.Attributes;

using StreamMaster.Domain.Attributes;

using System.Xml.Serialization;

namespace StreamMaster.Domain.API;

[RequireAll]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public class PagedResponse<T> : APIResponse<List<T>>
{
    [XmlIgnore]
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPageCount { get; set; }
    public int First { get; set; }
}

public static class PagedListExtensions
{
    public static PagedResponse<TDto> ToPagedResponseDto<T, TDto>(this PagedResponse<T> paged, IMapper mapper)
    {
        int first = (paged.PageNumber - 1) * paged.PageSize;
        return new PagedResponse<TDto>
        {
            Data = mapper.Map<List<TDto>>(paged.Data),
            First = first,
            PageNumber = paged.PageNumber,
            PageSize = paged.PageSize,
            TotalPageCount = paged.TotalPageCount,
            TotalItemCount = paged.TotalItemCount
        };
    }
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
    public static PagedResponse<T> ToPagedResponse<T>(this IPagedList<T> pagedList)
    {
        int first = (pagedList.PageNumber - 1) * pagedList.PageSize;
        return new PagedResponse<T>
        {
            Data = [.. pagedList],
            First = first,
            PageNumber = pagedList.PageNumber,
            PageSize = pagedList.PageSize,
            TotalPageCount = pagedList.PageCount,
            TotalItemCount = pagedList.TotalItemCount
        };
    }
}