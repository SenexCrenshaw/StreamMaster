//using AutoMapper;

//using MediatR;

//using Microsoft.AspNetCore.Http;
//using Microsoft.EntityFrameworkCore;

//using System.Text.Json;
//using System.Text.Json.Serialization;

//namespace StreamMasterDomain.Pagination;

//public class PaginationMetadata
//{
//    public int TotalCount { get; set; }
//    public int PageSize { get; set; }
//    public int CurrentPage { get; set; }
//    public int TotalPages { get; set; }
//    public bool HasNext { get; set; }
//    public bool HasPrevious { get; set; }
//}

//public class PagedList<T> : List<T>
//{
//    [JsonInclude]
//    public int CurrentPage { get; private set; }

//    [JsonInclude]
//    public int TotalPages { get; private set; }

//    public int PageSize { get; private set; }
//    public int TotalCount { get; private set; }
//    public bool HasPrevious => CurrentPage > 1;
//    public bool HasNext => CurrentPage < TotalPages;

//    public PagedList(List<T> items, int count, int pageNumber, int pageSize)
//    {
//        TotalCount = count;
//        PageSize = pageSize;
//        CurrentPage = pageNumber;
//        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
//        AddRange(items);
//    }

//    public string GetMetadataJSON()
//    {
//        return JsonSerializer.Serialize(Metadata);
//    }

//    public PaginationMetadata Metadata
//    {
//        get
//        {
//            return new()
//            {
//                TotalCount = TotalCount,
//                PageSize = PageSize,
//                CurrentPage = CurrentPage,
//                TotalPages = TotalPages,
//                HasNext = HasNext,
//                HasPrevious = HasPrevious
//            };
//        }
//    }

//    public static async Task<PagedList<T>> ToPagedList(IQueryable<T> source, int pageNumber, int pageSize)
//    {
//        int count = source.Count();
//        if (pageSize == 0)
//        {
//            pageSize = count;
//        }
//        if (pageNumber == 0)
//        {
//            pageNumber = -1;
//        }
//        List<T> items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
//        return new PagedList<T>(items, count, pageNumber, pageSize);
//    }

//    public static PagedList<T> ToPagedList(List<T> source, int pageNumber, int pageSize)
//    {
//        int count = source.Count();
//        if (pageSize == 0)
//        {
//            pageSize = count;
//        }
//        if (pageNumber == 0)
//        {
//            pageNumber = -1;
//        }
//        List<T> items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
//        return new PagedList<T>(items, count, pageNumber, pageSize);
//    }
//}