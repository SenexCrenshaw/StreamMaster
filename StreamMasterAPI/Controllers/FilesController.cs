using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Caching.Memory;

using StreamMasterAPI.Interfaces;

using StreamMasterDomain.Cache;
using StreamMasterDomain.Common;
using StreamMasterDomain.Enums;
using StreamMasterDomain.Models;

using System.Web;

namespace StreamMasterAPI.Controllers;

public class FilesController(IMemoryCache memoryCache, IContentTypeProvider mimeTypeProvider) : ApiControllerBase, IFileController
{
    private static readonly IDictionary<string, string> _contentTypesCache = new Dictionary<string, string>();

    [AllowAnonymous]
    [Route("{filetype}/{source}")]
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

        string source = HttpUtility.UrlDecode(URL);
        string fileName = "";
        string returnName = "";
        FileDefinition fd = FileDefinitions.Icon;

        if (IPTVFileType == SMFileTypes.TvLogo)
        {
            TvLogoFile? cache = memoryCache.TvLogos().FirstOrDefault(a => a.Source == source);
            if (cache == null || !cache.FileExists) { return (null, null); }
            returnName = cache.Source;
            fileName = FileDefinitions.TVLogo.DirectoryLocation + returnName;
        }
        else
        {
            Setting setting = await SettingsService.GetSettingsAsync();
            if (!setting.CacheIcons)
            {
                return (null, null);
            }
            List<StreamMasterDomain.Dto.IconFileDto> icons = memoryCache.Icons();
            StreamMasterDomain.Dto.IconFileDto? icon = icons.FirstOrDefault(a => a.Source == source);

            if (icon is null)
            {
                return (null, null);
            }

            switch (IPTVFileType)
            {
                case SMFileTypes.Icon:
                    fd = FileDefinitions.Icon;
                    break;

                case SMFileTypes.ProgrammeIcon:
                    fd = FileDefinitions.ProgrammeIcon;
                    break;

                case SMFileTypes.M3U:
                    break;

                case SMFileTypes.EPG:
                    break;

                case SMFileTypes.HDHR:
                    break;

                case SMFileTypes.Channel:
                    break;

                case SMFileTypes.M3UStream:
                    break;

                case SMFileTypes.Image:
                    break;

                case SMFileTypes.TvLogo:
                    fd = FileDefinitions.TVLogo;
                    break;

                default:
                    fd = FileDefinitions.Icon;
                    break;
            }
            returnName = $"{icon.Name}.{icon.Extension}";
            fileName = $"{fd.DirectoryLocation}{returnName}";

            if (!System.IO.File.Exists(fileName))
            {
                (bool success, Exception? ex) = await FileUtil.DownloadUrlAsync(source, fileName, cancellationToken).ConfigureAwait(false);
                if (!success)
                {
                    return (null, null);
                }
            }
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
        if (_contentTypesCache.TryGetValue(fileName, out string? cachedContentType))
        {
            return cachedContentType;
        }

        if (!mimeTypeProvider.TryGetContentType(fileName, out string? contentType))
        {
            contentType = "application/octet-stream";
        }

        _contentTypesCache[fileName] = contentType;
        return contentType;
    }
}