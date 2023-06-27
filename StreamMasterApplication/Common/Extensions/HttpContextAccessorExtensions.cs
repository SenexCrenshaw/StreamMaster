using Microsoft.AspNetCore.Http;

namespace StreamMasterInfrastructure.Extensions;

public static class HttpContextAccessorExtensions
{
    public static string GetUrl(this IHttpContextAccessor httpContextAccessor)
    {
        var request = httpContextAccessor.HttpContext.Request;
        var scheme = request.Scheme;
        var host = request.Host;

        var url = $"{scheme}://{host}";

        return url;
    }

    public static string GetUrlWithPath(this IHttpContextAccessor httpContextAccessor)
    {
        var request = httpContextAccessor.HttpContext.Request;
        var scheme = request.Scheme;
        var host = request.Host;

        var path = request.Path;

        var url = $"{scheme}://{host}{path}";

        return url;
    }

}
