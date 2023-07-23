using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

using StreamMasterAPI.Interfaces;

using StreamMasterApplication.Common.Interfaces;

using StreamMasterDomain.Entities;
using StreamMasterDomain.Enums;

using System.Web;

namespace StreamMasterAPI.Controllers;

public class FilesController : ApiControllerBase, IFileController
{
    private readonly IAppDbContext _context;
    private readonly IMemoryCache _memoryCache;

    public FilesController(
        IMemoryCache memoryCache,
        IAppDbContext context
    )
    {
        _memoryCache = memoryCache;
        _context = context;
    }

    [AllowAnonymous]
    [Route("{filetype}/{fileName}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetFile(string fileName, SMFileTypes filetype)
    {
        //Console.WriteLine($"{filetype}/{fileName}");
        fileName = HttpUtility.UrlDecode(fileName);

        (CacheEntity? cache, byte[]? image) = await GetCacheEntryAsync(new GetCacheEntryAsyncRequest { URL = fileName, IPTVFileType = filetype }).ConfigureAwait(false);
        if (image == null || cache == null)
        {
            Console.WriteLine($"{fileName} is null");
            return NotFound();
        }
        return File(image, cache.ContentType, cache.Name + "." + cache.FileExtension);
    }

    private async Task<(CacheEntity? cacheEntry, byte[]? data)> GetCacheEntryAsync(GetCacheEntryAsyncRequest request)
    {
        if (string.IsNullOrEmpty(request.URL))
        {
            return (null, null);
        }

        string source = HttpUtility.UrlDecode(request.URL);

        CacheEntity? cache;
        if (request.IPTVFileType == SMFileTypes.TvLogo)
        {
            cache = _memoryCache.TvLogos().FirstOrDefault(a => a.Name + a.FileExtension == source);

            if (cache == null || !cache.FileExists)
            {
                return (cache, null);
            }

            Response.ContentType = cache.ContentType;
            
            var data = await System.IO.File.ReadAllBytesAsync(FileDefinitions.TVLogo.DirectoryLocation+ cache.OriginalSource).ConfigureAwait(false);

            return (cache, data);
        }

        cache = _context.Icons.FirstOrDefault(a => a.Source == source);

        if (cache is null || !cache.FileExists)
        {
            return (cache, null);
        }

        FileDefinition fd = FileDefinitions.Icon;
        switch (request.IPTVFileType)
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
                break;

            default:
                fd = FileDefinitions.Icon;
                break;
        }

        Response.ContentType = cache.ContentType;

        (CacheEntity cache, byte[]) ret = (cache, await System.IO.File.ReadAllBytesAsync($"{fd.DirectoryLocation}{cache.Name}.{cache.FileExtension}").ConfigureAwait(false));

        return ret;
    }
}
