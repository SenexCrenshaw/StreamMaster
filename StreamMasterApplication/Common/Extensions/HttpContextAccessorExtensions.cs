using Microsoft.AspNetCore.Http;

namespace StreamMasterApplication.Common.Extensions;

public static class HttpContextAccessorExtensions
{
    public static string GetUrl(this IHttpContextAccessor httpContextAccessor)
    {
        return httpContextAccessor.GetUrlWithPath();
        //HttpRequest request = httpContextAccessor.HttpContext.Request;
        //string scheme = request.Scheme;
        //HostString host = request.Host;

        //string url = $"{scheme}://{host}";

        //return url;
    }

    public static string GetUrlWithPath(this IHttpContextAccessor httpContextAccessor)
    {
        HttpRequest? request = httpContextAccessor.HttpContext?.Request;
        if (request == null)
        {
            return string.Empty;
        }

        string path = request.Path.ToString()
            .Replace("/capability", "")
            .Replace("/device.xml", "")
            .Replace("/discover.json", "");

        return $"{request.Scheme}://{request.Host}{path}";
    }

    public static string GetUrlWithPathValue(this IHttpContextAccessor httpContextAccessor)
    {
        string? value = httpContextAccessor.HttpContext?.Request.Path.Value;
        if (value == null)
        {
            return string.Empty;
        }

        return value;
    }

}
