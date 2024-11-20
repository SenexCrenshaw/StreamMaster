using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;
using System.Web;

using AutoMapper;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using StreamMaster.Domain.API;
using StreamMaster.Domain.Common;
using StreamMaster.Domain.Configuration;
using StreamMaster.Domain.Dto;
using StreamMaster.Domain.Enums;
using StreamMaster.Domain.Extensions;
using StreamMaster.Domain.XmltvXml;
using StreamMaster.PlayList;
using StreamMaster.PlayList.Models;
using StreamMaster.SchedulesDirect.Domain.Interfaces;

namespace StreamMaster.Infrastructure.Services;
public class LogoService(IMapper mapper, ICustomPlayListBuilder customPlayListBuilder, IImageDownloadQueue imageDownloadQueue, IServiceProvider serviceProvider, IDataRefreshService dataRefreshService, IOptionsMonitor<Setting> _settings, ILogger<LogoService> logger)
    : ILogoService
{
    private ConcurrentDictionary<string, LogoFileDto> Logos { get; } = [];
    private ConcurrentDictionary<string, TvLogoFile> TvLogos { get; set; } = [];

    public List<XmltvProgramme> GetXmltvProgrammeForPeriod(StationChannelName stationChannelName, DateTime startDate, int days, string baseUrl)
    {
        List<(Movie Movie, DateTime StartTime, DateTime EndTime)> movies = customPlayListBuilder.GetMoviesForPeriod(stationChannelName.ChannelName, startDate, days);
        List<XmltvProgramme> ret = [];
        foreach ((Movie Movie, DateTime StartTime, DateTime EndTime) x in movies)
        {
            XmltvProgramme programme = XmltvProgrammeConverter.ConvertMovieToXmltvProgramme(x.Movie, stationChannelName.Id);
            if (x.Movie.Thumb is not null && !string.IsNullOrEmpty(x.Movie.Thumb.Text))
            {
                string src = GetLogoUrl(x.Movie.Thumb.Text, baseUrl);
                programme.Icons = [new XmltvIcon { Src = src }];
            }
            ret.Add(programme);
        }
        return ret;
    }

    public void CacheSMChannelLogos()
    {
        if (!_settings.CurrentValue.LogoCache.EqualsIgnoreCase("Cache"))
        {
            return;
        }

        using IServiceScope scope = serviceProvider.CreateScope();
        ISMChannelService smChannelService = scope.ServiceProvider.GetRequiredService<ISMChannelService>();

        IQueryable<NameLogo> smChannelsLogos = smChannelService.GetNameLogos();

        foreach (NameLogo smChannelsLogo in smChannelsLogos)
        {
            imageDownloadQueue.EnqueueNameLogo(smChannelsLogo);
        }
    }

    private static string? GetCachedFile(string source, SMFileTypes smFileType)
    {
        FileDefinition? fd = FileDefinitions.GetFileDefinition(smFileType);
        if (fd is null)
        {
            return null;
        }

        string ext = Path.GetExtension(source) ?? fd.DefaultExtension;

        string fileName = FileUtil.EncodeToMD5(source) + ext;
        string subDir = char.ToLower(fileName[0]).ToString();

        string fullPath = Path.Combine(fd.DirectoryLocation, subDir, fileName);

        return !File.Exists(fullPath) ? null : fullPath;
    }
    public string GetLogoUrl(string logoSource, string baseUrl)
    {
        if (string.IsNullOrEmpty(logoSource))
        {
            return $"{baseUrl}{_settings.CurrentValue.DefaultLogo}";
        }

        if (logoSource.StartsWith('/'))
        {
            logoSource = logoSource[1..];
        }

        if (logoSource.StartsWith(BuildInfo.CustomPlayListFolder))
        {
            return GetApiUrl(baseUrl, logoSource, SMFileTypes.CustomLogo);
        }

        if (logoSource.StartsWith("images/"))
        {
            return $"{baseUrl}/{logoSource}";
        }
        else if (!logoSource.StartsWith("http"))
        {
            return GetApiUrl(baseUrl, logoSource, SMFileTypes.TvLogo);
        }
        else if (_settings.CurrentValue.LogoCache.EqualsIgnoreCase("cache"))
        {
            return GetApiUrl(baseUrl, logoSource, SMFileTypes.Logo);
        }

        return logoSource;
    }
    private static string GetApiUrl(string url, string source, SMFileTypes path)
    {
        string encodedPath = Convert.ToBase64String(Encoding.UTF8.GetBytes(source));

        return $"{url}/api/files/{(int)path}/{encodedPath}";
    }

    public async Task<DataResponse<bool>> BuildLogosCacheFromSMStreamsAsync(CancellationToken cancellationToken)
    {
        if (_settings.CurrentValue.LogoCache != "Cache")
        {
            return DataResponse.False;
        }

        using IServiceScope scope = serviceProvider.CreateScope();
        ISMStreamService smStreamService = scope.ServiceProvider.GetRequiredService<ISMStreamService>();

        IQueryable<SMStream> streams = smStreamService.GetSMStreamLogos(true);

        if (!streams.Any()) { return DataResponse.False; }

        int totalCount = streams.Count();

        ParallelOptions parallelOptions = new()
        {
            CancellationToken = cancellationToken,
            MaxDegreeOfParallelism = Environment.ProcessorCount
        };

        _ = Parallel.ForEach(streams, parallelOptions, stream =>
        {
            if (cancellationToken.IsCancellationRequested) { return; }

            string source = HttpUtility.UrlDecode(stream.Logo);

            LogoFileDto logo = ToLogoFileDto(source, stream.Name, stream.M3UFileId, FileDefinitions.Logo);
            AddLogo(logo);
        });
        await dataRefreshService.RefreshLogos();
        return DataResponse.True;
    }

    private static LogoFileDto ToLogoFileDto(string sourceUrl, string? recommendedName, int fileId, FileDefinition fileDefinition)
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

        LogoFileDto icon = new()
        {
            Source = source,
            Extension = ext,
            Name = Path.GetFileNameWithoutExtension(name),
            SMFileType = fileDefinition.SMFileType,
            FileId = fileId
        };

        return icon;
    }

    public void AddLogo(LogoFileDto LogoFile)
    {
        _ = Logos.TryAdd(LogoFile.Source, LogoFile);
    }

    public void AddLogo(string artworkUri, string title)
    {
        if (string.IsNullOrEmpty(artworkUri))
        {
            return;
        }

        List<LogoFileDto> logos = GetLogos();

        if (logos.Any(a => a.SMFileType == SMFileTypes.SDImage && a.Source == artworkUri))
        {
            return;
        }

        AddLogo(new LogoFileDto { Source = artworkUri, SMFileType = SMFileTypes.SDImage, Name = title });
    }

    //public void AddIcons(List<LogoFileDto> newIconFiles)
    //{
    //    IEnumerable<LogoFileDto> missingIcons = newIconFiles.Except(Logos.Values, new IconFileDtoComparer());
    //    missingIcons = missingIcons.Distinct(new IconFileDtoComparer());

    //    foreach (LogoFileDto LogoFile in missingIcons)
    //    {
    //        AddLogo(LogoFile);
    //    }
    //}

    public void ClearLogos()
    {
        Logos.Clear();
    }

    public List<TvLogoFile> GetTvLogos()
    {
        return [.. TvLogos.Values];
    }

    public void ClearTvLogos()
    {
        TvLogos.Clear();
    }

    public LogoFileDto? GetLogoBySource(string source)
    {
        return Logos.TryGetValue(source, out LogoFileDto? logo) ? logo : null;
    }

    public ImagePath? GetValidImagePath(string baseURL, SMFileTypes? fileType = null)
    {
        string url = HttpUtility.UrlDecode(baseURL);

        if (fileType == SMFileTypes.Logo)
        {
            string LogoReturnName = Path.GetFileName(url);
            string? cachedFile = GetCachedFile(url, SMFileTypes.Logo);
            if (cachedFile != null)
            {
                return new ImagePath
                {
                    ReturnName = LogoReturnName,
                    FullPath = cachedFile,
                    SMFileType = SMFileTypes.Logo
                };
            }
        }

        if (fileType == SMFileTypes.CustomPlayListLogo)
        {
            string fullPath = BuildInfo.CustomPlayListFolder + baseURL;
            if (File.Exists(fullPath))
            {
                return new ImagePath
                {
                    ReturnName = Path.GetFileName(fullPath),
                    FullPath = fullPath,
                    SMFileType = SMFileTypes.CustomPlayListLogo
                };
            }
        }

        if (fileType == SMFileTypes.CustomPlayList)
        {
            string customPlayListfileName = Path.GetFileNameWithoutExtension(baseURL);
            _ = customPlayListBuilder.GetCustomPlayList(customPlayListfileName);
            string fullPath = Path.Combine(BuildInfo.CustomPlayListFolder, customPlayListfileName, baseURL);
            if (File.Exists(fullPath))
            {
                return new ImagePath
                {
                    ReturnName = Path.GetFileName(fullPath),
                    FullPath = fullPath,
                    SMFileType = SMFileTypes.CustomPlayList
                };
            }
        }

        if (!url.StartsWith("http"))
        {
            string? fullPath = url.GetSDImageFullPath();
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

        string returnName;
        if (TvLogos.TryGetValue(url, out TvLogoFile? cache))
        {
            returnName = cache.Source;
            string tvLogosFileName = Path.Combine(BuildInfo.TVLogoFolder, returnName);
            return new ImagePath
            {
                ReturnName = returnName,
                FullPath = tvLogosFileName,
                SMFileType = SMFileTypes.TvLogo
            };
        }

        Stopwatch sw = Stopwatch.StartNew();
        LogoFileDto? logo = GetLogoBySource(url);

        if (logo is null)
        {
            sw.Stop();
            if (sw.ElapsedMilliseconds > 10)
            {
                logger.LogInformation("GetValidImagePath GetIcBySource took {ElapsedMilliseconds}ms", sw.ElapsedMilliseconds);
            }
            return null;
        }

        FileDefinition fd = FileDefinitions.Logo;

        returnName = $"{logo.Name}.{logo.Extension}";
        string fileName = Path.Combine(fd.DirectoryLocation, returnName);

        return File.Exists(fileName)
            ? new ImagePath
            {
                ReturnName = returnName,
                SMFileType = SMFileTypes.Logo,
                FullPath = fileName,
            }
            : null;
    }

    public List<LogoFileDto> GetLogos(SMFileTypes? SMFileType = null)
    {
        List<LogoFileDto> logos = [];

        if (SMFileType != null)
        {
            logos = Logos.Values.Where(a => a.SMFileType == SMFileType).ToList();
        }
        else
        {
            logos = mapper.Map<List<LogoFileDto>>(TvLogos.Values);
            logos.AddRange(Logos.Values);
        }

        List<LogoFileDto> test = logos.Where(a => a.Name.Contains("Wick") || a.SMFileType == SMFileTypes.CustomPlayList).ToList();
        IOrderedEnumerable<LogoFileDto> ret = logos.OrderBy(a => a.Name);

        return [.. ret];
    }

    public async Task<bool> ReadDirectoryTVLogos(CancellationToken cancellationToken = default)
    {
        TvLogos = new ConcurrentDictionary<string, TvLogoFile>(
    [
        new(
            BuildInfo.LogoDefault,
            new TvLogoFile
            {
                Id = 0,
                Source = BuildInfo.LogoDefault,
                FileExists = true,
                Name = "Default Logo"
            }
        ),
        new (
            "/images/StreamMaster.png",
            new TvLogoFile
            {
                Id = 1,
                Source = "/images/streammaster_logo.png",
                FileExists = true,
                Name = "Stream Master"
            }
        )
    ]);

        FileDefinition fd = FileDefinitions.TVLogo;
        if (!Directory.Exists(fd.DirectoryLocation))
        {
            return false;
        }

        DirectoryInfo dirInfo = new(BuildInfo.TVLogoFolder);

        List<TvLogoFile> tvLogoFiles = await FileUtil.GetTVLogosFromDirectory(dirInfo, dirInfo.FullName, TvLogos.Count, cancellationToken).ConfigureAwait(false);

        foreach (TvLogoFile tvLogoFile in tvLogoFiles)
        {
            _ = TvLogos.TryAdd(tvLogoFile.Source, tvLogoFile);
        }

        return true;
    }

    public void RemoveLogosByM3UFileId(int id)
    {
        foreach (KeyValuePair<string, LogoFileDto> logo in Logos.Where(a => a.Value.FileId == id))
        {
            _ = Logos.TryRemove(logo.Key, out _);
        }
    }
}
