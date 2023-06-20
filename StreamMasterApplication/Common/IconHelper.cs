using AutoMapper;

using Microsoft.EntityFrameworkCore;

using StreamMasterDomain.Dto;

using System.Runtime.CompilerServices;
using System.Web;

namespace StreamMasterApplication.Common;

internal static class IconHelper
{
    /// <summary>
    /// AddIcon from URL
    /// </summary>
    /// <param name="sourceUrl"></param>
    /// <param name="recommendedName"></param>
    /// <param name="context"></param>
    /// <param name="_mapper"></param>
    /// <param name="setting"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<(IconFile iconFile, bool isNew)> AddIcon(string sourceUrl, string? additionalUrl, string? recommendedName, IAppDbContext context, IMapper _mapper, SettingDto setting, FileDefinition fileDefinition, CancellationToken cancellationToken = default)
    {
        string source = HttpUtility.UrlDecode(sourceUrl);
        if (sourceUrl.Contains("deba6af644347122056ec73f6b885215ff4534230b214addfc795ae7db60c38f"))
        {
            var aaa = 1;
        }
        IconFile? icon = await context.Icons.AsNoTracking().FirstOrDefaultAsync(a => a.Source == source && a.SMFileType == fileDefinition.SMFileType, cancellationToken: cancellationToken).ConfigureAwait(false);

        if (icon != null)
        {
            //if (icon.FileExists && icon.LastDownloaded.AddDays(-3) > DateTime.Now)// File Exists and last download is less then 3 days ago
            //{
            //    continue;
            //}

            if (icon.DownloadErrors > 3)//Download errors over three and tried last 7 days ago
            {
                if (icon.LastDownloaded.AddDays(-7 * icon.DownloadErrors) < DateTime.Now)
                {
                    return (icon, false);
                }
            }
            else
            {
                return (icon, false);
            }
        }

        bool badDownload = false;

        string contentType = "";
        string ext = "";

        additionalUrl ??= "";

        contentType = await FileUtil.GetContentType(source + additionalUrl).ConfigureAwait(false);

        if (!string.IsNullOrEmpty(contentType))
        {
            ext = contentType == "" ? "" : Path.GetFileName(contentType);
        }
        else
        {
            badDownload = true;
        }

        if (string.IsNullOrEmpty(ext))
        {
            ext = Path.GetExtension(source);

            if (!string.IsNullOrEmpty(ext))
            {
                ext = ext.Remove(0, 1);
            }
        }

        string newUrl = "";
        if (badDownload)
        {
            newUrl = "/"+Constants.IconDefault;
            contentType = "image/png";
            ext = "png";
        }
        else
        {
            newUrl = $"/api/files/{(int)fileDefinition.SMFileType}/{HttpUtility.UrlEncode(source)}";
        }

        string name = "";
        string fullName = "";
        if (ext == "jpeg")
        {
            ext = "jpg";
        }

        if (!string.IsNullOrEmpty(recommendedName))
        {
            name = string.Join("_", recommendedName.Split(Path.GetInvalidFileNameChars())) + $".{ext}";
            fullName = $"{fileDefinition.DirectoryLocation}{name}";
        }
        else
        {
            (fullName, name) = fileDefinition.DirectoryLocation.GetRandomFileName($".{ext}");
        }

        fileDefinition.FileExtension = ext;
        bool isNew = false;
        if (icon == null)
        {
            isNew = true;
            icon = new IconFile
            {
                OriginalSource = source,
                Source = source,
                Url = newUrl,
                Name = Path.GetFileNameWithoutExtension(name),
                ContentType = contentType,
                LastDownloaded = DateTime.Now,
                LastDownloadAttempt = DateTime.Now,
                FileExists = false,
                MetaData = "",
                FileExtension = ext,
                SMFileType = fileDefinition.SMFileType
            };

            if (!badDownload && fileDefinition.SMFileType == SMFileTypes.Icon)
            {
                icon.AddDomainEvent(new IconFileAddedEvent(_mapper.Map<IconFileDto>(icon)));
            }

            _ = context.Icons.Add(icon);
        }

        if (!badDownload)
        {
            (bool success, Exception? ex) = await FileUtil.DownloadUrlAsync(source + additionalUrl, fullName, cancellationToken).ConfigureAwait(false);

            if (success)
            {
                icon.FileExists = true;
                icon.DownloadErrors = 0;
            }
            else
            {
                ++icon.DownloadErrors;
            }
        }
        else
        {
            ++icon.DownloadErrors;
        }
        _ = await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return (icon, isNew);
    }
}
