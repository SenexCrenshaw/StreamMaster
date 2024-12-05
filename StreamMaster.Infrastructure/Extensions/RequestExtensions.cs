using Microsoft.AspNetCore.Http;

using StreamMaster.Domain.Extensions;

namespace StreamMaster.Infrastructure.Extensions;

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
        headers.LastModified = SMDT.UtcNow.ToString("r");
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

        System.Net.IPAddress? remoteIP = request.HttpContext.Connection.RemoteIpAddress;

        if (remoteIP == null)
        {
            return string.Empty;
        }

        if (remoteIP.IsIPv4MappedToIPv6)
        {
            remoteIP = remoteIP.MapToIPv4();
        }

        return remoteIP.ToString();
    }
}
