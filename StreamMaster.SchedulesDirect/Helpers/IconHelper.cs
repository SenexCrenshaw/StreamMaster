using StreamMaster.Domain.Dto;
using StreamMaster.Domain.Enums;
using StreamMaster.Domain.Models;

using System.Net;
using System.Web;

namespace StreamMaster.SchedulesDirect.Helpers;

public class IconHelper(IEPGHelper ePGHelper, IIconService iconService, IOptionsMonitor<Setting> intSettings) : IIconHelper
{
    private readonly Setting settings = intSettings.CurrentValue;

    public string GetIconUrl(int EPGNumber, string iconOriginalSource, string _baseUrl, SMFileTypes? sMFileTypes = null)
    {

        if (ePGHelper.IsCustom(EPGNumber))
        {
            return GetApiUrl(SMFileTypes.CustomPlayList, iconOriginalSource, _baseUrl);
        }

        if (ePGHelper.IsDummy(EPGNumber))
        {
            return iconOriginalSource;
        }

        if (ePGHelper.IsSchedulesDirect(EPGNumber))
        {
            return iconOriginalSource.StartsWith("http") ? iconOriginalSource : GetApiUrl(sMFileTypes ?? SMFileTypes.SDImage, iconOriginalSource, _baseUrl);
        }


        if (string.IsNullOrEmpty(iconOriginalSource))
        {
            return $"{_baseUrl}{settings.DefaultIcon}";
        }

        string originalUrl = iconOriginalSource;

        if (iconOriginalSource.StartsWith('/'))
        {
            iconOriginalSource = iconOriginalSource[1..];
        }

        SMFileTypes? smtype = sMFileTypes;
        if (smtype == null)
        {
            ImagePath? imagePath = iconService.GetValidImagePath(iconOriginalSource);

            if (imagePath != null)
            {
                smtype = imagePath.SMFileType;
            }

        }
        smtype ??= SMFileTypes.Icon;

        string icon = settings.CacheIcons ? GetApiUrl((SMFileTypes)smtype, originalUrl, _baseUrl) : iconOriginalSource;

        return icon;
    }

    public static IconFileDto GetIcon(string sourceUrl, string? recommendedName, int fileId, FileDefinition fileDefinition)
    {
        string source = HttpUtility.UrlDecode(sourceUrl);
        string ext = Path.GetExtension(source)?.TrimStart('.') ?? string.Empty;

        string name;
        if (!string.IsNullOrEmpty(recommendedName))
        {
            name = string.Join("_", recommendedName.Split(Path.GetInvalidFileNameChars())) + $".{ext}";
            //fullName = $"{fileDefinition.DirectoryLocation}{name}";
        }
        else
        {
            (_, name) = fileDefinition.DirectoryLocation.GetRandomFileName($".{ext}");
        }

        IconFileDto icon = new()
        {
            Source = source,
            Extension = ext,
            Name = Path.GetFileNameWithoutExtension(name),
            SMFileType = fileDefinition.SMFileType,
            FileId = fileId
        };

        return icon;
    }

    private static string GetApiUrl(SMFileTypes path, string source, string _baseUrl)
    {
        return $"{_baseUrl}/api/files/{(int)path}/{WebUtility.UrlEncode(source)}";
    }


}
