using Microsoft.AspNetCore.Http;

namespace StreamMasterInfrastructure.Extensions;

public static class RequestExtensions
{
    public static void DisableCache(this IHeaderDictionary headers)
    {
        headers.Remove("Last-Modified");
        headers["Cache-Control"] = "no-cache, no-store";
        headers["Expires"] = "-1";
        headers["Pragma"] = "no-cache";
    }

    public static void EnableCache(this IHeaderDictionary headers)
    {
        headers["Cache-Control"] = "max-age=31536000, public";
        headers["Last-Modified"] = DateTime.Now.ToString("r");
    }

    public static string GetRemoteIP(this HttpContext context)
    {
        return context?.Request?.GetRemoteIP() ?? "Unknown";
    }

    public static string GetRemoteIP(this HttpRequest request)
    {
        if (request == null)
        {
            return "Unknown";
        }

        var remoteIP = request.HttpContext.Connection.RemoteIpAddress;

        if (remoteIP.IsIPv4MappedToIPv6)
        {
            remoteIP = remoteIP.MapToIPv4();
        }

        return remoteIP.ToString();
    }
}
