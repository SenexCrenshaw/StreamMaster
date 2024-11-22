using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using StreamMaster.API.Interfaces;
using StreamMaster.Domain.Enums;

namespace StreamMaster.API.Controllers;

public class FilesController(IOptionsMonitor<Setting> settings, ILogoService logoService) : ApiControllerBase, IFileController
{
    [AllowAnonymous]
    [Route("{filetype}/{source}")]

    public async Task<IActionResult> GetFile(string source, SMFileTypes filetype, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(source) || source == "noimage.png" || source.EndsWith(settings.CurrentValue.DefaultLogo))
        {
            return Redirect("/" + settings.CurrentValue.DefaultLogo);
        }

        LogoDto? logoDto = await logoService.GetLogoFromCacheAsync(source, filetype, cancellationToken).ConfigureAwait(false);
        return logoDto == null
            ? source.Contains("api/files") ? Redirect("/" + settings.CurrentValue.DefaultLogo) : (IActionResult)Redirect(source)
            : File(logoDto.Image!, logoDto.ContentType ?? "", logoDto.FileName);
    }

    //private static bool IsBase64String(string base64)
    //{
    //    if (string.IsNullOrEmpty(base64) || base64.Length % 4 != 0)
    //    {
    //        return false;
    //    }

    //    try
    //    {
    //        _ = Convert.FromBase64String(base64);
    //        return true;
    //    }
    //    catch (Exception)
    //    {
    //        return false;
    //    }
    //}

    //private async Task<(byte[]? image, string? fileName)> GetLogoFromCacheAsync(string URL, SMFileTypes fileType, CancellationToken cancellationToken)
    //{

    //    if (string.IsNullOrEmpty(URL))
    //    {
    //        return (null, null);
    //    }

    //    //string source = HttpUtility.UrlDecode(URL);
    //    //string fileName = "";
    //    //string returnName = "";

    //    ImagePath? imagePath = logoService.GetValidImagePath(URL, fileType);
    //    if (imagePath == null)
    //    {
    //        return (null, null);
    //    }

    //    if (System.IO.File.Exists(imagePath.FullPath))
    //    {
    //        try
    //        {
    //            byte[] ret = await System.IO.File.ReadAllBytesAsync(imagePath.FullPath, cancellationToken).ConfigureAwait(false);
    //            return (ret, imagePath.ReturnName);
    //        }
    //        catch
    //        {
    //            return (null, null);
    //        }
    //    }

    //    return (null, null);
    //}

    //private string GetContentType(string fileName)
    //{
    //    string cacheKey = $"ContentType-{fileName}";

    //    if (!memoryCache.TryGetValue(cacheKey, out string? contentType))
    //    {
    //        if (!mimeTypeProvider.TryGetContentType(fileName, out contentType))
    //        {
    //            contentType = "application/octet-stream";
    //        }
    //        contentType ??= "application/octet-stream";

    //        _ = memoryCache.Set(cacheKey, contentType, CacheManagerExtensions.NeverRemoveCacheEntryOptions);
    //    }

    //    return contentType ?? "application/octet-stream";
    //}
}