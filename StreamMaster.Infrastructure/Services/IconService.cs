using AutoMapper;

using Microsoft.Extensions.Caching.Memory;

using StreamMaster.Domain.Cache;
using StreamMaster.Domain.Common;
using StreamMaster.Domain.Dto;
using StreamMaster.Domain.Enums;
using StreamMaster.Domain.Extensions;
using StreamMaster.Domain.Services;

using System.Collections.Concurrent;
using System.Web;

namespace StreamMaster.Infrastructure.Services;
public class IconService(IMapper mapper, IMemoryCache memoryCache) : IIconService
{
    private ConcurrentDictionary<string, IconFileDto> Icons { get; set; } = [];
    private ConcurrentDictionary<string, TvLogoFile> TvLogos { get; set; } = [];

    public void AddIcon(IconFileDto iconFile)
    {
        Icons.TryAdd(iconFile.Source, iconFile);
    }

    //public void SetIndexes()
    //{
    //    //var index = 0;
    //    //foreach (var icon in Icons.OrderBy(a => a.Value.Name))
    //    //{
    //    //    icon.Value.Id = index++;
    //    //}
    //}

    public void AddIcons(List<IconFileDto> newIconFiles)
    {
        IEnumerable<IconFileDto> missingIcons = newIconFiles.Except(Icons.Values, new IconFileDtoComparer());
        missingIcons = missingIcons.Distinct(new IconFileDtoComparer());

        foreach (IconFileDto iconFile in missingIcons)
        {
            AddIcon(iconFile);
        }
        //SetIndexes();
    }

    public void ClearIcons()
    {
        Icons.Clear();
    }

    public List<TvLogoFile> GetTvLogos()
    {
        return [.. TvLogos.Values];
    }

    public void ClearTvLogos()
    {
        TvLogos.Clear();
    }

    public IconFileDto? GetIconBySource(string source)
    {
        return Icons.TryGetValue(source, out IconFileDto? icon) ? icon : null;
    }

    public ImagePath? GetValidImagePath(string URL)
    {
        string source = HttpUtility.UrlDecode(URL);
        string fileName = "";
        string returnName = "";

        string fullPath = source.GetSDImageFullPath();
        if (File.Exists(fullPath))
        {
            return new ImagePath
            {
                ReturnName = Path.GetFileName(fullPath),
                FullPath = fullPath,
                SMFileType = SMFileTypes.SDImage
            };
        }

        List<TvLogoFile> a = GetTvLogos();
        TvLogoFile? cache = GetTvLogos().FirstOrDefault(a => a.Source == source);
        if (cache != null)
        {
            returnName = cache.Source;
            fileName = FileDefinitions.TVLogo.DirectoryLocation + returnName;
            return new ImagePath
            {
                ReturnName = returnName,
                FullPath = fileName,
                SMFileType = SMFileTypes.TvLogo
            };

        }

        Setting setting = memoryCache.GetSetting();

        IconFileDto? icon = GetIconBySource(source);

        if (icon is null)
        {
            return null;
        }
        FileDefinition fd = FileDefinitions.Icon;

        //switch (IPTVFileType)
        //{
        //    case SMFileTypes.Icon:
        //        fd = FileDefinitions.Icon;
        //        break;

        //    case SMFileTypes.ProgrammeIcon:
        //        fd = FileDefinitions.ProgrammeIcon;
        //        break;

        //    case SMFileTypes.M3U:
        //        break;

        //    case SMFileTypes.EPG:
        //        break;

        //    case SMFileTypes.HDHR:
        //        break;

        //    case SMFileTypes.Channel:
        //        break;

        //    case SMFileTypes.M3UStream:
        //        break;

        //    case SMFileTypes.Image:
        //        break;

        //    case SMFileTypes.TvLogo:
        //        fd = FileDefinitions.TVLogo;
        //        break;

        //    default:
        //        fd = FileDefinitions.Icon;
        //        break;
        //}
        returnName = $"{icon.Name}.{icon.Extension}";
        fileName = $"{fd.DirectoryLocation}{returnName}";

        return File.Exists(fileName)
            ? new ImagePath
            {
                ReturnName = returnName,
                SMFileType = SMFileTypes.Icon,
                FullPath = fileName,
            }
            : null;
    }

    public List<IconFileDto> GetIcons(SMFileTypes? SMFileType = null)
    {
        List<IconFileDto> icons = [];

        if (SMFileType != null)
        {
            icons = Icons.Values.Where(a => a.SMFileType == SMFileType).ToList();
        }
        else
        {
            icons = mapper.Map<List<IconFileDto>>(TvLogos.Values);
            icons.AddRange(Icons.Values);
        }

        IOrderedEnumerable<IconFileDto> ret = icons.OrderBy(a => a.Name);

        return [.. ret];
    }

    public async Task<bool> ReadDirectoryTVLogos(CancellationToken cancellationToken = default)
    {
        FileDefinition fd = FileDefinitions.TVLogo;
        if (!Directory.Exists(fd.DirectoryLocation))
        {
            return false;
        }

        DirectoryInfo dirInfo = new(BuildInfo.TVLogoDataFolder);

        TvLogos = new ConcurrentDictionary<string, TvLogoFile>(new List<KeyValuePair<string, TvLogoFile>>
    {
        new(
            "Default Icon",
            new TvLogoFile
            {
                Id = 0,
                Source = BuildInfo.IconDefault,
                FileExists = true,
                Name = "Default Icon"
            }
        ),
        new (
            "Stream Master",
            new TvLogoFile
            {
                Id = 1,
                Source = "images/StreamMaster.png",
                FileExists = true,
                Name = "Stream Master"
            }
        )
    });

        List<TvLogoFile> tvLogoFiles = await FileUtil.GetTVLogosFromDirectory(dirInfo, dirInfo.FullName, TvLogos.Count, cancellationToken).ConfigureAwait(false);

        foreach (TvLogoFile tvLogoFile in tvLogoFiles)
        {
            TvLogos.TryAdd(tvLogoFile.Name, tvLogoFile);
        }

        return true;
    }

    public void RemoveIconsByM3UFileId(int id)
    {
        foreach (KeyValuePair<string, IconFileDto> icon in Icons.Where(a => a.Value.FileId == id))
        {
            Icons.TryRemove(icon.Key, out _);
        }
    }
}
