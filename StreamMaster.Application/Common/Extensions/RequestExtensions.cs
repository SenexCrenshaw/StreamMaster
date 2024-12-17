using Microsoft.AspNetCore.Http;
using StreamMaster.Domain.Configuration;

namespace StreamMaster.Application.Common.Extensions;

public static class RequestExtensions
{
    public static void DisableCache(this IHeaderDictionary headers)
    {
        headers.Remove("Last-Modified");
        headers.CacheControl = "no-cache, no-store";
        headers.Expires = "-1";
        headers.Pragma = "no-cache";
    }

    public static void EnableCache(this IHeaderDictionary headers)
    {
        headers.CacheControl = "max-age=31536000, public";
        headers.LastModified = BuildInfo.BuildDateTime.ToString("r");
    }
}
