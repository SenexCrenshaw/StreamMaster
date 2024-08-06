using Microsoft.AspNetCore.Http;

namespace StreamMaster.Application.Common.Extensions;

public static class HttpContextAccessorExtensions
{
    public static string GetUrl(this IHttpContextAccessor httpContextAccessor)
    {
        return httpContextAccessor.GetUrlWithPath(false);

    }

    public static string GetUrl(this HttpRequest httpRequest)
    {
        return httpRequest.GetUrlWithPath(false);

    }

    public static string GetUrlWithPath(this IHttpContextAccessor httpContextAccessor, bool includePath = true)
    {
        HttpRequest? request = httpContextAccessor.HttpContext?.Request;
        if (request == null)
        {
            return string.Empty;
        }

        string path = request.Path.ToString()
            .Replace("/capability", "")
            .Replace("/device.xml", "")
              .Replace("/lineup.json", "")
            .Replace("/discover.json", "");

        string url = includePath ? $"{request.Scheme}://{request.Host}{path}" : $"{request.Scheme}://{request.Host}";
        if (url.StartsWith("wss"))
        {
            url = "https" + url[3..];
        }
        return url;
    }

    public static string GetUrlWithPath(this HttpRequest request, bool includePath = true)
    {

        string path = request.Path.ToString()
            .Replace("/capability", "")
            .Replace("/device.xml", "")
              .Replace("/lineup.json", "")
            .Replace("/discover.json", "");

        string url = includePath ? $"{request.Scheme}://{request.Host}{path}" : $"{request.Scheme}://{request.Host}";
        if (url.StartsWith("wss"))
        {
            url = "https" + url[3..];
        }
        return url;
    }

    public static string GetUrlWithPathValue(this HttpRequest httpRequest)
    {
        string? value = httpRequest.Path.Value;
        return value ?? string.Empty;
    }

    public static string GetUrlWithPathValue(this IHttpContextAccessor httpContextAccessor)
    {
        string? value = httpContextAccessor.HttpContext?.Request.Path.Value;
        return value ?? string.Empty;
    }

}
