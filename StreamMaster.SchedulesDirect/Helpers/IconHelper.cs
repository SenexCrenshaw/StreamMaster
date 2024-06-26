using StreamMaster.Domain.Configuration;
using StreamMaster.Domain.Dto;
using StreamMaster.Domain.Enums;
using StreamMaster.Domain.Models;

using System.Net;
using System.Web;

namespace StreamMaster.SchedulesDirect.Helpers;

public class IconHelper(IEPGHelper ePGHelper, IIconService iconService, IOptionsMonitor<Setting> intsettings) : IIconHelper
{
    private readonly Setting settings = intsettings.CurrentValue;

    public string GetIconUrl(int EPGNumber, string iconOriginalSource, string _baseUrl, SMFileTypes? sMFileTypes = null)
    {

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

    ///// <summary>
    ///// AddIcon from URL
    ///// </summary>
    ///// <param name="sourceUrl"></param>
    ///// <param name="recommendedName"></param>
    ///// <param name="context"></param>
    ///// <param name="_mapper"></param>
    ///// <param name="setting"></param>
    ///// <param name="cancellationToken"></param>
    ///// <returns></returns>
    //public static IconFileDto AddIcon(string sourceUrl, string? recommendedName, int fileId, int Id, IOptionsMonitor<Setting> intsettings, FileDefinition fileDefinition, CancellationToken cancellationToken, bool ignoreAdd = false)
    //{
    //    string source = HttpUtility.UrlDecode(sourceUrl);

    //    IconFileDto? testIcon = memoryCache.GetIcon(source, fileDefinition.SMFileType);

    //    //var testIcon = icons.FirstOrDefault(a => a.Source == source && a.SMFileType == fileDefinition.SMFileType);

    //    if (testIcon != null)
    //    {
    //        return testIcon;
    //    }

    //    IconFileDto icon = GetIcon(sourceUrl, recommendedName, fileId, fileDefinition);
    //    icon.Id = Id;


    //    if (ignoreAdd)
    //    {
    //        return icon;
    //    }

    //    if (fileDefinition.SMFileType != SMFileTypes.ProgrammeIcon)
    //    {
    //        memoryCache.AddIcon(icon);
    //    }
    //    else
    //    {
    //        memoryCache.AddProgrammeLogo(icon);
    //    }

    //    return icon;
    //}

    //public static async Task<bool> ReadDirectoryLogos(IOptionsMonitor<Setting> intsettings, CancellationToken cancellationToken = default)
    //{
    //    FileDefinition fd = FileDefinitions.TVLogo;
    //    if (!Directory.Exists(fd.DirectoryLocation))
    //    {
    //        return false;
    //    }

    //    DirectoryInfo dirInfo = new(BuildInfo.TVLogoDataFolder);

    //    List<TvLogoFile> tvLogos =
    //    [
    //        new TvLogoFile
    //        {
    //            Id = 0,
    //            Source = BuildInfo.IconDefault,
    //            FileExists = true,
    //            Name = "Default Icon"
    //        },

    //        new TvLogoFile
    //        {
    //            Id = 1,
    //            Source = "images/StreamMaster.png",
    //            FileExists = true,
    //            Name = "Stream Master"
    //        }
    //    ];

    //    tvLogos.AddRange(await FileUtil.GetIconFilesFromDirectory(dirInfo, dirInfo.FullName, tvLogos.Count, cancellationToken).ConfigureAwait(false));

    //    //memoryCache.ClearTvLogos();
    //    memoryCache.SetTvLogos(tvLogos);
    //    return true;
    //}

    private static string GetApiUrl(SMFileTypes path, string source, string _baseUrl)
    {
        return $"{_baseUrl}/api/files/{(int)path}/{WebUtility.UrlEncode(source)}";
    }


}
