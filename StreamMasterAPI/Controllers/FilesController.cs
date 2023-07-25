using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Caching.Memory;

using StreamMasterAPI.Interfaces;

using StreamMasterApplication.Common.Interfaces;

using StreamMasterDomain.Common;
using StreamMasterDomain.Entities;
using StreamMasterDomain.Enums;

using System.Web;

namespace StreamMasterAPI.Controllers;

public class FilesController : ApiControllerBase, IFileController
{
    private static readonly IDictionary<string, string> _contentTypesCache = new Dictionary<string, string>();
    
    private readonly IMemoryCache _memoryCache;
    private readonly IContentTypeProvider _mimeTypeProvider;
    private readonly Setting setting;

    public FilesController(
        IMemoryCache memoryCache,
        IContentTypeProvider mimeTypeProvider
    )
    {
        _mimeTypeProvider = mimeTypeProvider;
        _memoryCache = memoryCache;    
        setting = FileUtil.GetSetting();
    }

    [AllowAnonymous]
    [Route("{filetype}/{source}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetFile(string source, SMFileTypes filetype, CancellationToken cancellationToken)
    {
        var sourceDecoded = HttpUtility.UrlDecode(source);

        var (image, fileName) = await GetCacheEntryAsync(sourceDecoded, filetype, cancellationToken).ConfigureAwait(false);
        if (image == null || fileName == null)
        {
            return Redirect(sourceDecoded);
        }

        var contentType = GetContentType(sourceDecoded);
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
        FileDefinition fd = FileDefinitions.Icon;

        if (IPTVFileType == SMFileTypes.TvLogo)
        {            
            var cache = _memoryCache.TvLogos().FirstOrDefault(a => a.Source == source);
            if (cache == null || !cache.FileExists) { return (null, null); }
            fileName = FileDefinitions.TVLogo.DirectoryLocation + cache.Source;
        }
        else
        {
            var icons = _memoryCache.Icons();
            var icon = icons.FirstOrDefault(a => a.Source == source);

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
            fileName = $"{fd.DirectoryLocation}{icon.Name}.{icon.Extension}";
        }

        if (!System.IO.File.Exists(fileName) && IPTVFileType != SMFileTypes.TvLogo  && setting.CacheIcons)
        {
            (bool success, Exception? ex) = await FileUtil.DownloadUrlAsync(source, fileName, cancellationToken).ConfigureAwait(false);
            if (!success)
            {
                return (null, null);
            }
        }

        if (System.IO.File.Exists(fileName))
        {
            byte[] ret = await System.IO.File.ReadAllBytesAsync(fileName).ConfigureAwait(false);
            return (ret, fileName);
        }

        return (null, null);
    }

    private string GetContentType(string fileName)
    {
        if (_contentTypesCache.TryGetValue(fileName, out var cachedContentType))
        {
            return cachedContentType;
        }

        if (!_mimeTypeProvider.TryGetContentType(fileName, out var contentType))
        {
            contentType = "application/octet-stream";
        }

        _contentTypesCache[fileName] = contentType;
        return contentType;
    }
}
