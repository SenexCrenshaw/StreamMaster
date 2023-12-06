using Microsoft.Extensions.Caching.Memory;

using StreamMasterDomain.Cache;
using StreamMasterDomain.Dto;
using StreamMasterDomain.Models;

using System.Web;

namespace StreamMasterDomain.Common;

public static class IconHelper
{

    private static readonly object _lock = new();
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
    //public static IconFileDto AddIcon(string sourceUrl, string? recommendedName, int fileId, int Id, IMemoryCache memoryCache, FileDefinition fileDefinition, CancellationToken cancellationToken, bool ignoreAdd = false)
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

    public static async Task<bool> ReadDirectoryLogos(IMemoryCache memoryCache, CancellationToken cancellationToken = default)
    {
        FileDefinition fd = FileDefinitions.TVLogo;
        if (!Directory.Exists(fd.DirectoryLocation))
        {
            return false;
        }

        DirectoryInfo dirInfo = new(BuildInfo.TVLogoDataFolder);

        List<TvLogoFile> tvLogos =
        [
            new TvLogoFile
            {
                Id = 0,
                Source = BuildInfo.IconDefault,
                FileExists = true,
                Name = "Default Icon"
            },

            new TvLogoFile
            {
                Id = 1,
                Source = "images/StreamMaster.png",
                FileExists = true,
                Name = "Stream Master"
            }
        ];

        tvLogos.AddRange(await FileUtil.GetIconFilesFromDirectory(dirInfo, dirInfo.FullName, tvLogos.Count, cancellationToken).ConfigureAwait(false));

        //memoryCache.ClearTvLogos();
        memoryCache.SetTvLogos(tvLogos);
        return true;
    }
}
