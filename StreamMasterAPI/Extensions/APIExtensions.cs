using AutoMapper;

using StreamMaster.SchedulesDirectAPI;

using StreamMasterDomain.Pagination;

using System.Text.Json;

namespace StreamMasterAPI.Extensions;

public static class APIExtensions
{
    public static PagedList<TDestination> GetPagedResult<TSource, TDestination>(PagedList<TSource> source, HttpResponse Response, IMapper mapper)
    {
        var json = JsonSerializer.Serialize(source.GetMetaData());
        Response.Headers.Add("X-Pagination", json);

        List<TDestination> destination = mapper.Map<List<TDestination>>(source);

        PagedList<TDestination> result = new(destination, source.PageNumber, source.PageSize);

        return result;
    }

    public static PagedList<TDestination> GetPagedResult<TSource, TDestination>(IPagedList<TSource> source, HttpResponse Response, IMapper mapper)
    {
        var json = JsonSerializer.Serialize(source.GetMetaData());
        Response.Headers.Add("X-Pagination", json);

        List<TDestination> destination = mapper.Map<List<TDestination>>(source);

        PagedList<TDestination> result = new(destination, source.PageNumber, source.PageSize);

        return result;
    }
}