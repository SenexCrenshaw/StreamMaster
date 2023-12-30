using AutoMapper;

using StreamMaster.Domain.Common;
using StreamMaster.Domain.Dto;
using StreamMaster.Domain.Enums;
using StreamMaster.Domain.Extensions;
using StreamMaster.Domain.Services;

using System.Collections.Concurrent;

namespace StreamMaster.Infrastructure.Services;
public class IconService(IMapper mapper) : IIconService
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

    public void ClearTvLogos()
    {
        TvLogos.Clear();
    }

    public IconFileDto? GetIconBySource(string source)
    {
        if (Icons.TryGetValue(source, out IconFileDto? icon))
        {
            return icon;
        }
        return null;

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

        //int index = 0;
        IOrderedEnumerable<IconFileDto> ret = icons.OrderBy(a => a.Name);
        //foreach (IconFileDto? c in ret)
        //{
        //    c.Id = index++;
        //}

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

        var tvLogoFiles = await FileUtil.GetTVLogosFromDirectory(dirInfo, dirInfo.FullName, TvLogos.Count, cancellationToken).ConfigureAwait(false);

        foreach (var tvLogoFile in tvLogoFiles)
        {
            TvLogos.TryAdd(tvLogoFile.Name, tvLogoFile);
        }

        return true;
    }

    public void RemoveIconsByM3UFileId(int id)
    {
        foreach (var icon in Icons.Where(a => a.Value.FileId == id))
        {
            Icons.TryRemove(icon.Key, out _);
        }
    }
}
