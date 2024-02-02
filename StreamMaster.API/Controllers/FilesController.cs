using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Caching.Memory;

using StreamMaster.Domain.Cache;
using StreamMaster.Domain.Logging;
using StreamMaster.Domain.Models;
using StreamMaster.Domain.Services;
using StreamMaster.SchedulesDirect.Domain.Enums;

using StreamMasterAPI.Interfaces;

using System.Web;

namespace StreamMaster.API.Controllers;

public class FilesController(IMemoryCache memoryCache, IIconService iconService, IContentTypeProvider mimeTypeProvider) : ApiControllerBase, IFileController
{
    [AllowAnonymous]
    [Route("{filetype}/{source}")]
    [LogExecutionTimeAspect]
    public async Task<IActionResult> GetFile(string source, SMFileTypes filetype, CancellationToken cancellationToken)
    {
        string sourceDecoded = HttpUtility.UrlDecode(source);
        if (source == "noimage.png")
        {
            return Redirect("/images/default.png");
        }

        (byte[]? image, string? fileName) = await GetCacheEntryAsync(sourceDecoded, filetype, cancellationToken).ConfigureAwait(false);
        if (image == null || fileName == null)
        {
            return Redirect(sourceDecoded);
        }

        string contentType = GetContentType(sourceDecoded);
        return File(image, contentType, fileName);
    }

    private async Task<(byte[]? image, string? fileName)> GetCacheEntryAsync(string URL, SMFileTypes IPTVFileType, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(URL))
        {
            return (null, null);
        }

        //string source = HttpUtility.UrlDecode(URL);
        //string fileName = "";
        //string returnName = "";

        ImagePath? imagePath = iconService.GetValidImagePath(URL);
        if (imagePath == null)
        {
            return (null, null);
        }


        if (System.IO.File.Exists(imagePath.FullPath))
        {
            byte[] ret = await System.IO.File.ReadAllBytesAsync(imagePath.FullPath).ConfigureAwait(false);
            return (ret, imagePath.ReturnName);
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

            memoryCache.Set(cacheKey, contentType, CacheManagerExtensions.NeverRemoveCacheEntryOptions);
        }

        return contentType ?? "application/octet-stream";
    }

}