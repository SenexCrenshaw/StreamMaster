using AutoMapper;

using Microsoft.Extensions.Caching.Memory;
using StreamMasterDomain.Cache;
using StreamMasterDomain.Dto;
using StreamMasterDomain.Repository;

using System.Web;

namespace StreamMasterDomain.Common;

public static class IconHelper
{
   
    private static readonly object _lock = new object();
    public static IconFileDto GetIcon(string sourceUrl, string? recommendedName, int fileId, FileDefinition fileDefinition)
    {
        string source = HttpUtility.UrlDecode(sourceUrl);
        string ext = Path.GetExtension(source)?.TrimStart('.') ?? string.Empty;

        string name;
        string fullName;
        if (!string.IsNullOrEmpty(recommendedName))
        {
            name = string.Join("_", recommendedName.Split(Path.GetInvalidFileNameChars())) + $".{ext}";
            //fullName = $"{fileDefinition.DirectoryLocation}{name}";
        }
        else
        {
            (_, name) = fileDefinition.DirectoryLocation.GetRandomFileName($".{ext}");
        }

        var icon = new IconFileDto
        {
            Source = source,
            Extension = ext,
            Name = Path.GetFileNameWithoutExtension(name),
            SMFileType = fileDefinition.SMFileType,
            FileId = fileId
        };

        return icon;
    }

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
        public static IconFileDto AddIcon(string sourceUrl, string? recommendedName, int fileId, IMapper _mapper, IMemoryCache memoryCache, FileDefinition fileDefinition, CancellationToken cancellationToken, bool ignoreAdd=false)
    {
        string source = HttpUtility.UrlDecode(sourceUrl);
       
            var testIcon = memoryCache.GetIcon(source, fileDefinition.SMFileType);

            //var testIcon = icons.FirstOrDefault(a => a.Source == source && a.SMFileType == fileDefinition.SMFileType);

            if (testIcon != null)
            {
                return testIcon;
            }

        var icon = GetIcon(sourceUrl, recommendedName, fileId, fileDefinition);



        if ( ignoreAdd)
        {
            return icon;
        }

        if (fileDefinition.SMFileType != SMFileTypes.ProgrammeIcon)
        {
            memoryCache.Add(icon);
        }
        else
        {
            memoryCache.AddProgrammeLogo(icon);
        }

        return icon;
    }

    public static async Task<bool> ReadDirectoryLogos(IMemoryCache memoryCache, CancellationToken cancellationToken)
    {
        var fd = FileDefinitions.TVLogo;
        if (!Directory.Exists(fd.DirectoryLocation))
        {
            return false;
        }

        Setting setting = FileUtil.GetSetting();
        DirectoryInfo dirInfo = new(BuildInfo.TVLogoDataFolder);

        List<TvLogoFile> tvLogos = new()
        {
            new TvLogoFile
            {
                Id=0,
                Source = BuildInfo.IconDefault,
                FileExists = true,
                Name = "Default Icon"
            },

            new TvLogoFile
            {
                Id=1,
                Source = setting.StreamMasterIcon,
                FileExists = true,
                Name = "Stream Master"
            }
        };

        tvLogos.AddRange(await FileUtil.GetIconFilesFromDirectory(dirInfo, fd.DirectoryLocation, tvLogos.Count, cancellationToken).ConfigureAwait(false));

        memoryCache.ClearIcons();
        memoryCache.Set(tvLogos);
        return true;
    }
}
