using AutoMapper;

using System.Text.Json;

namespace StreamMaster.API.Extensions;

public static class APIExtensions
{
    public static PagedList<TDestination> GetPagedResult<TSource, TDestination>(PagedList<TSource> source, HttpResponse Response, IMapper mapper)
    {
        string json = JsonSerializer.Serialize(source.GetMetaData());
        Response.Headers.Append("X-Pagination", json);

        List<TDestination> destination = mapper.Map<List<TDestination>>(source);

        PagedList<TDestination> result = new(destination, source.PageNumber, source.PageSize);

        return result;
    }

    public static PagedList<TDestination> GetPagedResult<TSource, TDestination>(IPagedList<TSource> source, HttpResponse Response, IMapper mapper)
    {
        string json = JsonSerializer.Serialize(source.GetMetaData());
        Response.Headers.Append("X-Pagination", json);

        List<TDestination> destination = mapper.Map<List<TDestination>>(source);

        PagedList<TDestination> result = new(destination, source.PageNumber, source.PageSize);

        return result;
    }
}