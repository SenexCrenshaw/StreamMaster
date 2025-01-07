using System.Runtime.CompilerServices;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using StreamMaster.WebDav.Helpers;

namespace StreamMaster.WebDav.Core;

/// <summary>
/// Middleware for handling WebDAV requests and routing to storage providers.
/// </summary>
public class WebDavMiddleware(RequestDelegate next, IWebDavStorageProvider storageProvider, ILockManager lockManager, ILogger<WebDavMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        string method = context.Request.Method;
        PathString path = context.Request.Path;

        logger.LogDebug("Received {Method} request for {Path}", method, path);

        try
        {
            switch (method)
            {
                case "HEAD":
                    await HandleHeadAsync(context, path);
                    break;

                case "OPTIONS":
                    await HandleOptionsAsync(context);
                    break;

                case "GET":
                    await HandleGetAsync(context, path);
                    break;

                case "PUT":
                    await HandlePutAsync(context, path);
                    break;

                case "PROPFIND":
                    await HandlePropFindAsync(context, path);
                    break;

                case "MKCOL":
                    await HandleMkColAsync(context, path);
                    break;

                case "DELETE":
                    await HandleDeleteAsync(context, path);
                    break;

                case "COPY":
                    await HandleCopyAsync(context, path);
                    break;

                case "MOVE":
                    await HandleMoveAsync(context, path);
                    break;

                case "LOCK":
                    await HandleLockAsync(context, path);
                    break;

                case "UNLOCK":
                    await HandleUnlockAsync(context, path);
                    break;

                default:
                    logger.LogWarning("Unsupported HTTP method: {Method}", method);
                    context.Response.StatusCode = StatusCodes.Status405MethodNotAllowed;
                    break;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing {Method} request for {Path}", method, path);
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        }
    }

    private async Task HandleHeadAsync(HttpContext context, string path)
    {
        logger.LogDebug("Handling HEAD request for {Path}", path);

        if (string.IsNullOrWhiteSpace(path) || path == "/")
        {
            logger.LogTrace("Responding to HEAD request for root directory");
            // Root directory: return headers for a collection
            context.Response.ContentType = "application/xml";
            context.Response.Headers["Content-Length"] = "0"; // No body for HEAD
            context.Response.StatusCode = StatusCodes.Status200OK;
            return;
        }

        // Check if the resource exists
        if (!await storageProvider.ExistsAsync(path, context.RequestAborted))
        {
            logger.LogWarning("Resource not found for HEAD request: {Path}", path);
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            return;
        }

        // Simulate the response headers for a GET request
        Stream? file = await storageProvider.GetFileAsync(path, context.RequestAborted);
        if (file != null)
        {
            context.Response.ContentType = "application/octet-stream";

            // Optionally, add headers for file metadata (e.g., Content-Length)
            if (file is FileStream fileStream)
            {
                context.Response.Headers["Content-Length"] = fileStream.Length.ToString();
            }

            context.Response.StatusCode = StatusCodes.Status200OK;
            logger.LogInformation("HEAD request for {Path} succeeded", path);
        }
        else
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            logger.LogWarning("HEAD request for {Path} returned 404", path);
        }
    }

    private Task HandleOptionsAsync(HttpContext context)
    {
        logger.LogDebug("Handling OPTIONS request");
        context.Response.StatusCode = StatusCodes.Status200OK;

        // Specify allowed methods
        context.Response.Headers["Allow"] = "OPTIONS, GET, PUT, PROPFIND, MKCOL, DELETE, COPY, MOVE, LOCK, UNLOCK";

        // Indicate WebDAV compliance level (RFC 4918)
        context.Response.Headers["DAV"] = "1, 2";

        return Task.CompletedTask;
    }

    private async Task HandleGetAsync(HttpContext context, string path)
    {
        logger.LogDebug("Handling GET request for {Path}", path);

        try
        {
            // Check if the requested path is for a .ts stream
            if (path.EndsWith(".ts", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    string? apiUrl = await storageProvider.GetTSStreamUrlAsync(path, context.RequestAborted);
                    if (string.IsNullOrEmpty(apiUrl))
                    {
                        logger.LogWarning("No API URL found for {Path}", path);
                        context.Response.StatusCode = StatusCodes.Status404NotFound;
                        return;
                    }
                    // Forward the request to the API
                    using HttpClient client = new();
                    HttpRequestMessage requestMessage = new(HttpMethod.Get, apiUrl);

                    // Forward headers from the WebDAV client to the API (if needed)
                    foreach (KeyValuePair<string, Microsoft.Extensions.Primitives.StringValues> header in context.Request.Headers)
                    {
                        if (!requestMessage.Headers.Contains(header.Key) && header.Key != "Host")
                        {
                            requestMessage.Headers.Add(header.Key, header.Value.ToString());
                        }
                    }

                    HttpResponseMessage apiResponse = await client.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, context.RequestAborted);

                    if (apiResponse.IsSuccessStatusCode)
                    {
                        // Copy API response headers to WebDAV client response
                        //context.Response.ContentType = apiResponse.Content.Headers.ContentType?.ToString() ?? "application/octet-stream";
                        //if (apiResponse.Content.Headers.ContentLength.HasValue)
                        //{
                        //    context.Response.ContentLength = apiResponse.Content.Headers.ContentLength.Value;
                        //}

                        // Stream API response body to the WebDAV client
                        await using Stream apiStream = await apiResponse.Content.ReadAsStreamAsync(context.RequestAborted);
                        //await apiStream.CopyToAsync(context.Response.Body, context.RequestAborted);

                        await foreach (byte[] chunk in StreamChunks(apiStream, context.RequestAborted))
                        {
                            await context.Response.Body.WriteAsync(chunk, context.RequestAborted);
                        }

                        logger.LogInformation("GET request for {Path} streamed successfully", path);
                    }
                    else
                    {
                        logger.LogWarning("API returned {StatusCode} for {Path}", apiResponse.StatusCode, path);
                        context.Response.StatusCode = (int)apiResponse.StatusCode;
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error handling GET request for {Path}", path);
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                }
                return;
            }

            // Default file handling for non-.ts requests
            Stream? file = await storageProvider.GetFileAsync(path, context.RequestAborted);
            if (file != null)
            {
                context.Response.ContentType = "application/octet-stream";
                await file.CopyToAsync(context.Response.Body, context.RequestAborted);
                logger.LogInformation("GET request for {Path} succeeded", path);
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                logger.LogWarning("GET request for {Path} returned 404", path);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error handling GET request for {Path}", path);
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        }
    }

    private static async IAsyncEnumerable<byte[]> StreamChunks(Stream source, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        const int bufferSize = 81920; // 80 KB chunks
        byte[] buffer = new byte[bufferSize];
        int bytesRead;
        while ((bytesRead = await source.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0)
        {
            yield return buffer[..bytesRead];
        }
    }


    private string ExtractEncodedIds(string path)
    {
        return Path.GetFileNameWithoutExtension(path);
    }

    private async Task HandlePutAsync(HttpContext context, string path)
    {
        logger.LogDebug("Handling PUT request for {Path}", path);
        await storageProvider.SaveToLocalCacheAsync(path, context.Request.Body, context.RequestAborted);
        context.Response.StatusCode = StatusCodes.Status201Created;
        logger.LogInformation("PUT created file for {Path} succeeded", path);
    }

    private async Task HandlePropFindAsync(HttpContext context, string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            path = "/";
        }
        logger.LogDebug("Handling PROPFIND request for {Path}", path);

        List<Domain.Models.DirectoryEntry> directoryEntries = await storageProvider.ListDirectoryAsync(path, context.RequestAborted).ToListAsync(context.RequestAborted);
        string directoryResponseXml = PropFindHelper.GeneratePropFindResponse(directoryEntries);

        context.Response.StatusCode = StatusCodes.Status207MultiStatus;
        context.Response.ContentType = "application/xml";
        await context.Response.WriteAsync(directoryResponseXml, context.RequestAborted);

        logger.LogInformation("PROPFIND request for {Path} succeeded", path);
    }

    private async Task HandleMkColAsync(HttpContext context, string path)
    {
        logger.LogDebug("Handling MkCol request for {Path}", path);
        if (await storageProvider.ExistsAsync(path, context.RequestAborted))
        {
            context.Response.StatusCode = StatusCodes.Status405MethodNotAllowed;
            logger.LogWarning("MkCol request for {Path} not found 405", path);
            return;
        }

        await storageProvider.CreateDirectoryAsync(path, context.RequestAborted);
        context.Response.StatusCode = StatusCodes.Status201Created;
        logger.LogDebug("MkCol request for {Path} succeeded", path);
    }

    private async Task HandleDeleteAsync(HttpContext context, string path)
    {
        if (!await storageProvider.ExistsAsync(path, context.RequestAborted))
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            logger.LogWarning("Delete request for {Path} not found 404", path);
            return;
        }

        await storageProvider.DeleteAsync(path, context.RequestAborted);
        context.Response.StatusCode = StatusCodes.Status204NoContent;
        logger.LogWarning("Delete request for {Path} succeeded", path);
    }

    private async Task HandleCopyAsync(HttpContext context, string sourcePath)
    {
        if (!context.Request.Headers.TryGetValue("Destination", out Microsoft.Extensions.Primitives.StringValues destinationHeader))
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            logger.LogWarning("Copy request source for {sourcePath} not found 400", sourcePath);
            return;
        }

        string destinationPath = destinationHeader.ToString();
        if (!await storageProvider.ExistsAsync(sourcePath, context.RequestAborted))
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            logger.LogWarning("Copy request for {sourcePath} not found 404", sourcePath);
            return;
        }

        await storageProvider.CopyAsync(sourcePath, destinationPath, context.RequestAborted);
        context.Response.StatusCode = StatusCodes.Status201Created;
        logger.LogWarning("Copy request for {sourcePath} to {destinationPath} succeeded", sourcePath, destinationPath);
    }

    private async Task HandleMoveAsync(HttpContext context, string sourcePath)
    {
        if (!context.Request.Headers.TryGetValue("Destination", out Microsoft.Extensions.Primitives.StringValues destinationHeader))
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            return;
        }

        string destinationPath = destinationHeader.ToString();
        if (!await storageProvider.ExistsAsync(sourcePath, context.RequestAborted))
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            return;
        }

        await storageProvider.MoveAsync(sourcePath, destinationPath, context.RequestAborted);
        context.Response.StatusCode = StatusCodes.Status201Created;
    }

    private async Task HandleLockAsync(HttpContext context, string path)
    {
        if (!context.Request.Headers.TryGetValue("Owner", out Microsoft.Extensions.Primitives.StringValues ownerHeader))
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            return;
        }

        string owner = ownerHeader.ToString();
        bool isExclusive = true; // Assume exclusive for simplicity.
        TimeSpan timeout = TimeSpan.FromMinutes(30);

        string? lockToken = await lockManager.AcquireLockAsync(path, owner, isExclusive, timeout, context.RequestAborted);
        if (lockToken == null)
        {
            context.Response.StatusCode = StatusCodes.Status423Locked;
            return;
        }

        context.Response.StatusCode = StatusCodes.Status200OK;
        context.Response.ContentType = "application/xml";
        await context.Response.WriteAsync($@"<D:prop xmlns:D='DAV:'>
        <D:lockdiscovery>
            <D:activelock>
                <D:locktype><D:write/></D:locktype>
                <D:lockscope><D:exclusive/></D:lockscope>
                <D:depth>infinity</D:depth>
                <D:owner>{owner}</D:owner>
                <D:timeout>Second-{timeout.TotalSeconds}</D:timeout>
                <D:locktoken><D:href>{lockToken}</D:href></D:locktoken>
            </D:activelock>
        </D:lockdiscovery>
    </D:prop>", context.RequestAborted);
    }

    private async Task HandleUnlockAsync(HttpContext context, string path)
    {
        if (!context.Request.Headers.TryGetValue("Lock-Token", out Microsoft.Extensions.Primitives.StringValues lockTokenHeader))
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            return;
        }

        string lockToken = lockTokenHeader.ToString();
        if (!await lockManager.ReleaseLockAsync(lockToken, context.RequestAborted))
        {
            context.Response.StatusCode = StatusCodes.Status409Conflict;
            return;
        }

        context.Response.StatusCode = StatusCodes.Status204NoContent;
    }
}