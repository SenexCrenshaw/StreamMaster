using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Caching.Memory;

using StreamMaster.SchedulesDirectAPI.Helpers;

using StreamMasterAPI.Interfaces;

using StreamMasterApplication.Common.Logging;

using StreamMasterDomain.Cache;
using StreamMasterDomain.Common;
using StreamMasterDomain.Enums;
using StreamMasterDomain.Models;

using System.Web;

namespace StreamMasterAPI.Controllers;

public class FilesController(IMemoryCache memoryCache, IContentTypeProvider mimeTypeProvider) : ApiControllerBase, IFileController
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


    private async Task<(string? returnName, string? fullPath)> GetValidImagePath(string URL)
    {
        string source = HttpUtility.UrlDecode(URL);
        string fileName = "";
        string returnName = "";


        //StreamMaster.SchedulesDirectAPIService.Domain.Models.ImageInfo? cache = memoryCache.ImageInfos().FirstOrDefault(a => a.IconUri == source);
        //if (cache == null) { return (null, null); }
        string fullPath = source.GetSDImageFullPath();// Path.Combine(FileDefinitions.SDImage.DirectoryLocation, source);
        if (System.IO.File.Exists(fullPath))
        {
            return (Path.GetFileName(fullPath), fullPath);
        }

        TvLogoFile? cache = memoryCache.GetTvLogos().FirstOrDefault(a => a.Source == source);
        if (cache != null)
        {
            returnName = cache.Source;
            fileName = FileDefinitions.TVLogo.DirectoryLocation + returnName;
            return (returnName, fileName);
        }

        Setting setting = await SettingsService.GetSettingsAsync();


        List<StreamMasterDomain.Dto.IconFileDto> icons = memoryCache.Icons();
        StreamMasterDomain.Dto.IconFileDto? icon = icons.FirstOrDefault(a => a.Source == source);

        if (icon is null)
        {
            return (null, null);
        }
        FileDefinition fd = FileDefinitions.Icon;

        //switch (IPTVFileType)
        //{
        //    case SMFileTypes.Icon:
        //        fd = FileDefinitions.Icon;
        //        break;

        //    case SMFileTypes.ProgrammeIcon:
        //        fd = FileDefinitions.ProgrammeIcon;
        //        break;

        //    case SMFileTypes.M3U:
        //        break;

        //    case SMFileTypes.EPG:
        //        break;

        //    case SMFileTypes.HDHR:
        //        break;

        //    case SMFileTypes.Channel:
        //        break;

        //    case SMFileTypes.M3UStream:
        //        break;

        //    case SMFileTypes.Image:
        //        break;

        //    case SMFileTypes.TvLogo:
        //        fd = FileDefinitions.TVLogo;
        //        break;

        //    default:
        //        fd = FileDefinitions.Icon;
        //        break;
        //}
        returnName = $"{icon.Name}.{icon.Extension}";
        fileName = $"{fd.DirectoryLocation}{returnName}";

        if (System.IO.File.Exists(fileName))
        {
            return (returnName, fileName);
        }


        return (null, null);
    }


    [LogExecutionTimeAspect]
    private async Task<(byte[]? image, string? fileName)> GetCacheEntryAsync(string URL, SMFileTypes IPTVFileType, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(URL))
        {
            return (null, null);
        }

        //string source = HttpUtility.UrlDecode(URL);
        //string fileName = "";
        //string returnName = "";

        (string? returnName, string? fileName) = await GetValidImagePath(URL);
        if (string.IsNullOrEmpty(returnName) || string.IsNullOrEmpty(fileName))
        {
            return (null, null);
        }

        if (System.IO.File.Exists(fileName))
        {
            byte[] ret = await System.IO.File.ReadAllBytesAsync(fileName).ConfigureAwait(false);
            return (ret, returnName);
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