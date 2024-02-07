using AutoMapper;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMaster.Domain.Common;
using StreamMaster.Domain.Dto;
using StreamMaster.Domain.Enums;
using StreamMaster.Domain.Extensions;
using StreamMaster.Domain.Services;
using StreamMaster.SchedulesDirect.Domain.Enums;

using System.Collections.Concurrent;
using System.Diagnostics;
using System.Web;

namespace StreamMaster.Infrastructure.Services;
public class IconService(IMapper mapper, IMemoryCache memoryCache, ILogger<IconService> logger) : IIconService
{
    private ConcurrentDictionary<string, IconFileDto> Icons { get; set; } = [];
    private ConcurrentDictionary<string, TvLogoFile> TvLogos { get; set; } = [];

    public void AddIcon(IconFileDto iconFile)
    {
        Icons.TryAdd(iconFile.Source, iconFile);
    }

    public void AddIcon(string artworkUri, string title)
    {
        if (string.IsNullOrEmpty(artworkUri))
        {
            return;
        }

        List<IconFileDto> icons = GetIcons();

        if (icons.Any(a => a.SMFileType == SMFileTypes.SDImage && a.Source == artworkUri))
        {
            return;
        }

        AddIcon(new IconFileDto { Source = artworkUri, SMFileType = SMFileTypes.SDImage, Name = title });

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

        if (!source.StartsWith("http"))
        {
            string? fullPath = source.GetSDImageFullPath();
            if (fullPath != null && File.Exists(fullPath))
            {
                return new ImagePath
                {
                    ReturnName = Path.GetFileName(fullPath),
                    FullPath = fullPath,
                    SMFileType = SMFileTypes.SDImage
                };
            }
        }


        if (TvLogos.TryGetValue(source, out TvLogoFile? cache))
        {
            returnName = cache.Source;
            fileName = Path.Combine(BuildInfo.TVLogoDataFolder, returnName);
            return new ImagePath
            {
                ReturnName = returnName,
                FullPath = fileName,
                SMFileType = SMFileTypes.TvLogo
            };
        }

        Stopwatch sw = Stopwatch.StartNew();
        IconFileDto? icon = GetIconBySource(source);

        if (icon is null)
        {
            sw.Stop();
            if (sw.ElapsedMilliseconds > 10)
            {
                logger.LogInformation($"GetValidImagePath GetIconBySource took {sw.ElapsedMilliseconds}ms");
            }
            return null;
        }

        FileDefinition fd = FileDefinitions.Icon;

        returnName = $"{icon.Name}.{icon.Extension}";
        fileName = Path.Combine(fd.DirectoryLocation, returnName);

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

        TvLogos = new ConcurrentDictionary<string, TvLogoFile>(
    [
        new(
            BuildInfo.IconDefault,
            new TvLogoFile
            {
                Id = 0,
                Source = BuildInfo.IconDefault,
                FileExists = true,
                Name = "Default Icon"
            }
        ),
        new (
            "images/StreamMaster.png",
            new TvLogoFile
            {
                Id = 1,
                Source = "images/StreamMaster.png",
                FileExists = true,
                Name = "Stream Master"
            }
        )
    ]);

        List<TvLogoFile> tvLogoFiles = await FileUtil.GetTVLogosFromDirectory(dirInfo, dirInfo.FullName, TvLogos.Count, cancellationToken).ConfigureAwait(false);

        foreach (TvLogoFile tvLogoFile in tvLogoFiles)
        {
            TvLogos.TryAdd(tvLogoFile.Source, tvLogoFile);
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
