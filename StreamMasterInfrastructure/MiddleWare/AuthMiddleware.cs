using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using StreamMasterDomain.Common;
using StreamMasterDomain.Enums;

namespace StreamMasterInfrastructure.MiddleWare;

public class AuthMiddleware
{
    private readonly ILogger<AuthMiddleware> _logger;
    private readonly RequestDelegate _next;

    public AuthMiddleware(RequestDelegate next, ILogger<AuthMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public string GetQueryParameter(HttpContext httpContext, string key)
    {
        var queryParameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (var queryParameter in httpContext.Request.Query)
        {
            queryParameters[queryParameter.Key] = queryParameter.Value;
        }

        return queryParameters.GetValueOrDefault(key);
    }

    public async Task Invoke(HttpContext context)
    {
        var remoteIpAddress = context.Connection.RemoteIpAddress.ToString();
        var url = $"{context.Request.Path}{context.Request.QueryString}";

        if (
             url.ToLower().EndsWith("/capability") ||
             url.ToLower().EndsWith("/device.xml") ||
             url.ToLower().EndsWith("/discover.json") ||
             url.ToLower().EndsWith("/lineup.json") ||
             url.ToLower().EndsWith("/lineup_status.json") ||
            url.ToLower().StartsWith("/images/") ||
            url.ToLower().StartsWith("/initialize.js") ||
            url.ToLower().StartsWith("/favicon.ico") ||
            url.ToLower().StartsWith($"/api/files/{(int)FileDefinitions.Icon.SMFileType}/") ||
            url.ToLower().StartsWith($"/api/files/{(int)FileDefinitions.TVLogo.SMFileType}/") ||
            url.ToLower().StartsWith($"/api/files/{(int)FileDefinitions.ProgrammeIcon.SMFileType}/")
            )
        {
            await _next(context);
            return;
        }

        //Debug.WriteLine($"remoteIpAddress: {remoteIpAddress}");
        //Debug.WriteLine($"Path: {context.Request.Path}");
        //Debug.WriteLine($"url: {url}");

        if (
            (remoteIpAddress.Equals("::1") || remoteIpAddress.EndsWith("127.0.0.1"))
            && !url.Contains("api"))
        {
            await _next(context);
            return;
        }

        var setting = FileUtil.GetSetting();

        if (string.IsNullOrEmpty(setting.APIUserName) || string.IsNullOrEmpty(setting.APIPassword))
        {
            await _next(context);
            return;
        }

        //Debug.WriteLine($"remoteIpAddress: {remoteIpAddress}");
        //Debug.WriteLine($"Path: {context.Request.Path}");
        //Debug.WriteLine($"url: {url}");

        string apiUserName = GetQueryParameter(context, "APIUserName");
        string apiPassword = GetQueryParameter(context, "APIPassword");

        if (setting.APIPassword != apiPassword || setting.APIUserName != apiUserName)
        {
            context.Response.StatusCode = 401;
            _logger.LogWarning($"Auth Failed: {remoteIpAddress} {url}");
            await context.Response.WriteAsync("Auth Failed");
            return;
        }

        await _next(context);
    }
}
