using System.Text;
using System.Web;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Caching.Memory;

using StreamMaster.API.Interfaces;
using StreamMaster.Domain.Enums;

namespace StreamMaster.API.Controllers;

public class FilesController(IMemoryCache memoryCache, IHttpContextAccessor httpContextAccessor, IOptionsMonitor<Setting> settings, ILogoService logoService, IContentTypeProvider mimeTypeProvider) : ApiControllerBase, IFileController
{
    [AllowAnonymous]
    [Route("{filetype}/{source}")]

    public async Task<IActionResult> GetFile(string source, SMFileTypes filetype, CancellationToken cancellationToken)
    {
        string sourceDecoded = string.Empty;

        if (IsBase64String(source))
        {
            try
            {
                sourceDecoded = Encoding.UTF8.GetString(Convert.FromBase64String(source));
            }
            catch (FormatException)
            {
                // Handle cases where the base64 string might be improperly formatted
                //sourceDecoded = HttpUtility.UrlDecode(source);
            }
        }
        else
        {
            string? newSource = Path.GetFileNameWithoutExtension(source);
            if (!string.IsNullOrWhiteSpace(newSource))
            {
                if (IsBase64String(newSource))
                {
                    try
                    {
                        sourceDecoded = Encoding.UTF8.GetString(Convert.FromBase64String(newSource));
                    }
                    catch (FormatException)
                    {
                        // Handle cases where the base64 string might be improperly formatted
                        sourceDecoded = HttpUtility.UrlDecode(newSource);
                    }
                    string ext = Path.GetExtension(source);
                    sourceDecoded += $"{ext}";
                }

            }

            //sourceDecoded = HttpUtility.UrlDecode(source);
        }

        if (sourceDecoded == "" || sourceDecoded == "noimage.png" || sourceDecoded.EndsWith(settings.CurrentValue.DefaultLogo))
        {
            //string baseUrl = httpContextAccessor.GetUrl();

            return Redirect("/" + settings.CurrentValue.DefaultLogo);
        }

        // string sourceDecoded = HttpUtility.UrlDecode(source);
        //string sourceDecoded = Encoding.UTF8.GetString(Convert.FromBase64String(source));
        if (source == "noimage.png")
        {
            return Redirect("/" + settings.CurrentValue.DefaultLogo);
        }

        (byte[]? image, string? fileName) = await GetCacheEntryAsync(sourceDecoded, filetype, cancellationToken).ConfigureAwait(false);
        if (image == null || fileName == null)
        {
            return sourceDecoded.Contains("api/files") ? Redirect("/" + settings.CurrentValue.DefaultLogo) : (IActionResult)Redirect(sourceDecoded);
        }

        string contentType = GetContentType(sourceDecoded);
        return File(image, contentType, fileName);
    }

    private static bool IsBase64String(string base64)
    {
        if (string.IsNullOrEmpty(base64) || base64.Length % 4 != 0)
        {
            return false;
        }

        try
        {
            // Attempt to convert; if it fails, it's not a valid base64 string
            _ = Convert.FromBase64String(base64);
            return true;
        }
        catch (FormatException)
        {
            return false;
        }
    }

    private async Task<(byte[]? image, string? fileName)> GetCacheEntryAsync(string URL, SMFileTypes fileType, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(URL))
        {
            return (null, null);
        }

        //string source = HttpUtility.UrlDecode(URL);
        //string fileName = "";
        //string returnName = "";

        ImagePath? imagePath = logoService.GetValidImagePath(URL, fileType);
        if (imagePath == null)
        {
            return (null, null);
        }

        if (System.IO.File.Exists(imagePath.FullPath))
        {
            try
            {
                byte[] ret = await System.IO.File.ReadAllBytesAsync(imagePath.FullPath, cancellationToken).ConfigureAwait(false);
                return (ret, imagePath.ReturnName);
            }
            catch
            {
                return (null, null);
            }
        }

        return (null, null);
    }

    private string GetContentType(string fileName)
    {
        string cacheKey = $"ContentType-{fileName}";

        if (!memoryCache.TryGetValue(cacheKey, out string? contentType))
        {
            if (!mimeTypeProvider.TryGetContentType(fileName, out contentType))
            {
                contentType = "application/octet-stream";
            }
            contentType ??= "application/octet-stream";

            _ = memoryCache.Set(cacheKey, contentType, CacheManagerExtensions.NeverRemoveCacheEntryOptions);
        }

        return contentType ?? "application/octet-stream";
    }
}