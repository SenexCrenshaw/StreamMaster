using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
namespace StreamMaster.Infrastructure.Extensions;
public static class HttpContextExtensions
{
    public static async Task LogRequestDetailsAsync(this HttpContext context, ILogger logger)
    {
        HttpRequest request = context.Request;

        // Log the request method and Url
        logger.LogInformation("Request Method: {Method}, Request URL: {Url}", request.Method, request.Path);

        // Log headers
        foreach (KeyValuePair<string, Microsoft.Extensions.Primitives.StringValues> header in request.Headers)
        {
            logger.LogInformation("Header: {Key}: {Value}", header.Key, header.Value.ToString());
        }

        // Log query parameters
        foreach (KeyValuePair<string, Microsoft.Extensions.Primitives.StringValues> queryParam in request.Query)
        {
            logger.LogInformation("Query Parameter: {Key}: {Value}", queryParam.Key, queryParam.Value.ToString());
        }

        // Log cookies
        foreach (KeyValuePair<string, string> cookie in request.Cookies)
        {
            logger.LogInformation("Cookie: {Key}: {Value}", cookie.Key, cookie.Value);
        }

        // Log request body if necessary
        context.Request.EnableBuffering();
        using StreamReader reader = new(context.Request.Body, leaveOpen: true);
        string body = await reader.ReadToEndAsync();
        context.Request.Body.Position = 0;

        logger.LogInformation("Request Body: {Body}", body);
    }
}
