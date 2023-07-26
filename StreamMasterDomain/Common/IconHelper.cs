using AutoMapper;

using Microsoft.Extensions.Caching.Memory;

using StreamMasterDomain.Dto;

using System.Web;

namespace StreamMasterDomain.Common;

public static class IconHelper
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
    public static async Task<IconFileDto> AddIcon(string sourceUrl, string? recommendedName, IMapper _mapper, IMemoryCache memoryCache, FileDefinition fileDefinition, CancellationToken cancellationToken)
    {
        string source = HttpUtility.UrlDecode(sourceUrl);
        var icons = memoryCache.Icons();
        if (!icons.Any())
        {
            if (await ReadDirectoryLogos(memoryCache, cancellationToken).ConfigureAwait(false))
            {
                var cacheValue = _mapper.Map<List<IconFileDto>>(memoryCache.TvLogos());
                icons = cacheValue;
                memoryCache.Set(icons);
            }
        }

        var icon = icons.FirstOrDefault(a => a.Source == source && a.SMFileType == fileDefinition.SMFileType);

        if (icon != null)
        {
            return icon;
        }

        string ext = Path.GetExtension(source)?.TrimStart('.') ?? string.Empty;

        string name;
        string fullName;
        if (!string.IsNullOrEmpty(recommendedName))
        {
            name = string.Join("_", recommendedName.Split(Path.GetInvalidFileNameChars())) + $".{ext}";
            fullName = $"{fileDefinition.DirectoryLocation}{name}";
        }
        else
        {
            (fullName, name) = fileDefinition.DirectoryLocation.GetRandomFileName($".{ext}");
        }

        icon = new IconFileDto
        {
            Source = source,
            Extension = ext,
            Name = Path.GetFileNameWithoutExtension(name),
            SMFileType = fileDefinition.SMFileType
        };

        memoryCache.Add(icon);

        return icon;
    }

    private static async Task<bool> ReadDirectoryLogos(IMemoryCache memoryCache, CancellationToken cancellationToken)
    {
        if (!Directory.Exists(Constants.TVLogoDirectory))
        {
            return false;
        }

        Setting setting = FileUtil.GetSetting();
        DirectoryInfo dirInfo = new(Constants.TVLogoDirectory);

        List<TvLogoFile> tvLogos = new()
        {
            new TvLogoFile
            {
                Id=0,
                Source = Constants.IconDefault,
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

        tvLogos.AddRange(await FileUtil.GetIconFilesFromDirectory(dirInfo, Constants.TVLogoDirectory, tvLogos.Count, cancellationToken).ConfigureAwait(false));

        memoryCache.ClearIcons();
        memoryCache.Set(tvLogos);
        return true;
    }
}
