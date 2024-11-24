using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;
using System.Web;

using AutoMapper;

using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using StreamMaster.Domain.API;
using StreamMaster.Domain.Cache;
using StreamMaster.Domain.Common;
using StreamMaster.Domain.Configuration;
using StreamMaster.Domain.Crypto;
using StreamMaster.Domain.Dto;
using StreamMaster.Domain.Enums;
using StreamMaster.Domain.Extensions;
using StreamMaster.Domain.Repository;
using StreamMaster.Domain.XmltvXml;
using StreamMaster.PlayList;
using StreamMaster.PlayList.Models;
using StreamMaster.SchedulesDirect.Domain.Interfaces;

namespace StreamMaster.Infrastructure.Services;
public class LogoService(IMapper mapper, ICustomPlayListBuilder customPlayListBuilder, IImageDownloadService imageDownloadService, IContentTypeProvider mimeTypeProvider, IMemoryCache memoryCache, IImageDownloadQueue imageDownloadQueue, IServiceProvider serviceProvider, IDataRefreshService dataRefreshService, IOptionsMonitor<Setting> _settings, ILogger<LogoService> logger)
    : ILogoService
{
    private ConcurrentDictionary<string, LogoFileDto> Logos { get; } = [];
    private ConcurrentDictionary<string, TvLogoFile> TvLogos { get; set; } = [];

    public List<XmltvProgramme> GetXmltvProgrammeForPeriod(VideoStreamConfig videoStreamConfig, DateTime startDate, int days, string baseUrl)
    {
        List<(Movie Movie, DateTime StartTime, DateTime EndTime)> movies = customPlayListBuilder.GetMoviesForPeriod(videoStreamConfig.Name, startDate, days);
        List<XmltvProgramme> ret = [];
        foreach ((Movie Movie, DateTime StartTime, DateTime EndTime) x in movies)
        {
            XmltvProgramme programme = XmltvProgrammeConverter.ConvertMovieToXmltvProgramme(x.Movie, videoStreamConfig.EPGId, x.StartTime, x.EndTime);
            if (x.Movie.Thumb is not null && !string.IsNullOrEmpty(x.Movie.Thumb.Text))
            {
                string src = GetLogoUrl(x.Movie.Thumb.Text, baseUrl, SMStreamTypeEnum.CustomPlayList);
                programme.Icons = [new XmltvIcon { Src = src }];
            }
            ret.Add(programme);
        }
        return ret;
    }

    public async Task CacheSMChannelLogos()
    {
        using IServiceScope scope = serviceProvider.CreateScope();

        //if (!_settings.CurrentValue.LogoCache.EqualsIgnoreCase("Cache"))
        //{
        //    return;
        //}


        ISMChannelService smChannelService = scope.ServiceProvider.GetRequiredService<ISMChannelService>();

        IQueryable<NameLogo> smChannelsLogos = smChannelService.GetNameLogos();

        foreach (NameLogo smChannelsLogo in smChannelsLogos)
        {
            imageDownloadQueue.EnqueueNameLogo(smChannelsLogo);
        }

    }
    public async Task<LogoDto?> GetLogoForChannelAsync(int SMChannelId, CancellationToken cancellationToken)
    {
        using IServiceScope scope = serviceProvider.CreateScope();
        IRepositoryWrapper repositoryWrapper = scope.ServiceProvider.GetRequiredService<IRepositoryWrapper>();

        SMChannel? channel = repositoryWrapper.SMChannel.GetSMChannel(SMChannelId);
        if (channel == null || string.IsNullOrEmpty(channel.Logo))
        {
            return null;
        }

        if (IsLocalLogo(channel.Logo))
        {
            LogoDto l = new()
            {
                Url = channel.Logo
            };
            return l;
        }

        string fileName = channel.Logo.StartsWithIgnoreCase("http") ? channel.Logo.GenerateFNV1aHash() : channel.Logo;

        LogoDto? ret = await GetLogoFromCacheAsync(fileName, cancellationToken).ConfigureAwait(false);
        if (ret is null)
        {
            NameLogo nameLogo = new(channel.Name, channel.Logo);
            //string? fullPath = GetFullPath(nameLogo.FileName, SMFileTypes.Logo);
            if (!string.IsNullOrEmpty(nameLogo.FullPath))
            {
                //FIX dont know the type
                if (await imageDownloadService.DownloadImageAsync(nameLogo, cancellationToken).ConfigureAwait(false))
                {
                    return await GetLogoFromCacheAsync(fileName, cancellationToken).ConfigureAwait(false);
                }
            }
        }
        return ret;
    }

    public bool IsLocalLogo(string Logo)
    {
        return string.IsNullOrEmpty(Logo)
          //|| !Logo.StartsWith("http")
          || Logo.EqualsIgnoreCase("noimage.png")
          || Logo.EqualsIgnoreCase(_settings.CurrentValue.DefaultLogo)
          || Logo.EqualsIgnoreCase("/images/streammaster_logo.png");
    }

    /// <summary>
    /// Extracts the ID and filename from a URL with a constant prefix "/api/files/".
    /// </summary>
    /// <param name="url">The input URL.</param>
    /// <param name="id">The extracted ID as an integer.</param>
    /// <param name="filename">The extracted filename.</param>
    /// <returns>True if parsing was successful; otherwise, false.</returns>
    public static bool TryParseUrl(string url, out int id, out string? filename)
    {
        const string prefix = "/api/files/";
        id = 0;
        filename = null;

        if (string.IsNullOrWhiteSpace(url) || !url.StartsWith(prefix))
        {
            return false;
        }

        string remaining = url[prefix.Length..];
        string[] parts = remaining.Split('/', 2); // Split into at most 2 parts

        // Ensure we have both ID and filename
        if (parts.Length == 2 && int.TryParse(parts[0], out id))
        {
            filename = parts[1];
            return true;
        }

        return false;
    }

    public async Task<LogoDto?> GetLogoFromCacheAsync(string fileName, CancellationToken cancellationToken)
    {
        if (IsLocalLogo(fileName))
        {
            return null;
        }

        ImagePath? imagePath = GetValidImagePath(fileName);
        if (imagePath == null)
        {
            return null;
        }

        if (File.Exists(imagePath.FullPath))
        {
            try
            {
                byte[] ret = await File.ReadAllBytesAsync(imagePath.FullPath, cancellationToken).ConfigureAwait(false);
                string contentType = GetContentType(imagePath.FullPath);
                return new LogoDto(fileName, contentType, imagePath.ReturnName, ret);
            }
            catch
            {
                return null;
            }
        }

        return null;
    }

    public async Task<LogoDto?> GetLogoFromCacheAsync(string source, SMFileTypes fileType, CancellationToken cancellationToken)
    {
        //string sourceDecoded = string.Empty;

        //if (source.IsBase64String())
        //{
        //    try
        //    {
        //        sourceDecoded = Encoding.UTF8.GetString(Convert.FromBase64String(source));
        //    }
        //    catch (FormatException)
        //    {
        //        // Handle cases where the base64 string might be improperly formatted
        //        //sourceDecoded = HttpUtility.UrlDecode(source);
        //    }
        //}
        //else
        //{
        //    string? newSource = Path.GetFileNameWithoutExtension(source);
        //    if (!string.IsNullOrWhiteSpace(newSource))
        //    {
        //        if (newSource.IsBase64String())
        //        {
        //            try
        //            {
        //                sourceDecoded = Encoding.UTF8.GetString(Convert.FromBase64String(newSource));
        //            }
        //            catch (FormatException)
        //            {
        //                // Handle cases where the base64 string might be improperly formatted
        //                sourceDecoded = HttpUtility.UrlDecode(newSource);
        //            }
        //            string ext = Path.GetExtension(source);
        //            sourceDecoded += $"{ext}";
        //        }
        //    }

        //    //sourceDecoded = HttpUtility.UrlDecode(source);
        //}
        if (source is null)
        {
            return null;
        }

        if (source.Length == 0 || source.HashEquals("noimage.png") || source.HashEquals(_settings.CurrentValue.DefaultLogo))
        {
            return null;
        }

        // string sourceDecoded = HttpUtility.UrlDecode(source);
        //string sourceDecoded = Encoding.UTF8.GetString(Convert.FromBase64String(source));
        //if (source == "noimage.png")
        //{
        //    return null;
        //}

        //if (string.IsNullOrEmpty(sourceDecoded))
        //{
        //    return null;
        //}

        string sourceDecoded = source;//.GenerateFNV1a64Hash();
        ImagePath? imagePath = GetValidImagePath(sourceDecoded, fileType);
        if (imagePath == null)
        {
            return null;
        }

        if (File.Exists(imagePath.FullPath))
        {
            try
            {
                byte[] ret = await File.ReadAllBytesAsync(imagePath.FullPath, cancellationToken).ConfigureAwait(false);
                string contentType = GetContentType(imagePath.FullPath);
                return new LogoDto(sourceDecoded, contentType, imagePath.ReturnName, ret);
            }
            catch
            {
                return null;
            }
        }

        return null;
    }

    private string GetContentType(string fileName)
    {
        string cacheKey = $"ContentType-{fileName}";

        if (!memoryCache.TryGetValue(cacheKey, out string? contentType))
        {
            if (!mimeTypeProvider.TryGetContentType(fileName, out contentType))
            {
                contentType = "application/octet-stream";
            }
            contentType ??= "application/octet-stream";

            _ = memoryCache.Set(cacheKey, contentType, CacheManagerExtensions.NeverRemoveCacheEntryOptions);
        }

        return contentType ?? "application/octet-stream";
    }

    private static string? GetCachedFile(string source, SMFileTypes smFileType)
    {
        string fullPath = NameLogo.GetFullPath(source, smFileType);

        return string.IsNullOrEmpty(fullPath) ? null : !File.Exists(fullPath) ? null : fullPath;
    }

    public string GetLogoUrl2(string logoSource, SMFileTypes logoType)
    {
        string source = logoSource;
        switch (logoType)
        {
            case SMFileTypes.Logo:
                string newlogoSource = GetApiUrl(logoSource, logoType);
                return newlogoSource;
            case SMFileTypes.CustomPlayListLogo:
                if (logoSource.StartsWith(BuildInfo.CustomPlayListFolder))
                {
                    source = logoSource.Remove(0, BuildInfo.CustomPlayListFolder.Length);
                }
                break;

        }

        return GetApiUrl(source, logoType);
    }
    private static string GetApiUrl(string source, SMFileTypes path)
    {
        string encodedPath = Convert.ToBase64String(Encoding.UTF8.GetBytes(source));

        return $"/api/files/{(int)path}/{encodedPath}";
    }

    public string GetLogoUrl(string logoSource, string baseUrl, SMStreamTypeEnum smStream)
    {
        baseUrl ??= string.Empty;

        if (string.IsNullOrEmpty(logoSource))
        {
            return $"{baseUrl}/{_settings.CurrentValue.DefaultLogo}";
        }

        if (logoSource.StartsWith('/'))
        {
            logoSource = logoSource[1..];
        }

        if (smStream == SMStreamTypeEnum.Custom || smStream == SMStreamTypeEnum.CustomPlayList || logoSource.StartsWith(BuildInfo.CustomPlayListFolder))
        {
            string a = logoSource.Remove(0, BuildInfo.CustomPlayListFolder.Length);
            return GetApiUrl(baseUrl, a, SMFileTypes.CustomPlayListLogo);
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

    private static string GetApiUrl(string baseUrl, string source, SMFileTypes path)
    {
        string encodedPath = Convert.ToBase64String(Encoding.UTF8.GetBytes(source));

        return $"{baseUrl}/api/files/{(int)path}/{encodedPath}";
    }

    public async Task<DataResponse<bool>> BuildLogosCacheFromSMStreamsAsync(CancellationToken cancellationToken)
    {
        //if (_settings.CurrentValue.LogoCache != "Cache")
        //{
        //    return DataResponse.False;
        //}

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

            //NameLogo nl = new(stream.Name, stream.Logo, SMFileTypes.SDStationLogo, false);
            //LogoFileDto logo = new() { Source = nl.FileName, Value = stream.Logo, SMFileType = SMFileTypes.SDStationLogo, Name = stream.Name };

            LogoFileDto logo = ToLogoFileDto(source, stream.Name, stream.M3UFileId, FileDefinitions.Logo);
            logo.Value = logo.Source;
            AddLogo(logo.Source, stream.Name, noHash: true, SMFileTypes.Logo, noApi: true);
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

    public void AddLogo(LogoFileDto LogoFile, bool noApi = false)
    {
        if (!noApi)
        {
            string url = $"/api/files/{(int)LogoFile.SMFileType}/{LogoFile.Source}";// GetApiUrl(LogoFile.Source, LogoFile.SMFileType);
            LogoFile.Source = url;
        }
        //_ = Logos.TryAdd(LogoFile.Source, LogoFile);
    }

    public void AddLogo(string URL, string title, bool noHash = false, SMFileTypes smFileType = SMFileTypes.Logo, bool noApi = false)
    {
        if (string.IsNullOrEmpty(URL))
        {
            return;
        }

        List<LogoFileDto> logos = GetLogos();

        if (logos.Any(a => a.SMFileType == SMFileTypes.SDStationLogo && a.Source == URL))
        {
            return;
        }

        AddLogo(new LogoFileDto { Source = noHash ? URL : URL.GenerateFNV1aHash(), Value = URL, SMFileType = smFileType, Name = title }, noApi);
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
        return Logos.TryGetValue(source.GenerateFNV1aHash(), out LogoFileDto? logo) ? logo : null;
    }

    public ImagePath? GetValidImagePath(string URL)
    {
        ImagePath? test = GetValidImagePath(URL, SMFileTypes.SDStationLogo);
        if (test is not null)
        {
            return test;
        }

        test = GetValidImagePath(URL, SMFileTypes.Logo);
        if (test is not null)
        {
            return test;
        }

        test = GetValidImagePath(URL, SMFileTypes.TvLogo);
        if (test is not null)
        {
            // A
            return test;
        }

        return test;
    }

    public ImagePath? GetValidImagePath(string URL, SMFileTypes fileType, bool? checkExists = true)
    {
        string url = HttpUtility.UrlDecode(URL);

        if (fileType == SMFileTypes.Logo)
        {
            string LogoReturnName = Path.GetFileName(url);
            string? cachedFile = GetCachedFile(url, SMFileTypes.Logo);
            return cachedFile != null
                ? new ImagePath
                {
                    ReturnName = LogoReturnName,
                    FullPath = cachedFile,
                    SMFileType = SMFileTypes.Logo
                }
                : null;
        }

        if (fileType is SMFileTypes.CustomPlayListLogo or SMFileTypes.CustomPlayList)
        {
            string fullPath = BuildInfo.CustomPlayListFolder + URL;
            return File.Exists(fullPath)
                ? new ImagePath
                {
                    ReturnName = Path.GetFileName(fullPath),
                    FullPath = fullPath,
                    SMFileType = SMFileTypes.CustomPlayListLogo
                }
                : null;
        }

        //if (fileType == SMFileTypes.CustomPlayList)
        //{
        //    string customPlayListfileName = Path.GetFileNameWithoutExtension(baseURL);
        //    _ = customPlayListBuilder.GetCustomPlayList(customPlayListfileName);
        //    string fullPath = Path.Combine(BuildInfo.CustomPlayListFolder, customPlayListfileName, baseURL);
        //    return File.Exists(fullPath)
        //        ? new ImagePath
        //        {
        //            ReturnName = Path.GetFileName(fullPath),
        //            FullPath = fullPath,
        //            SMFileType = SMFileTypes.CustomPlayList
        //        }
        //        : null;
        //}

        if (fileType == SMFileTypes.SDStationLogo)
        {
            if (!url.StartsWith("http"))
            {
                try
                {
                    string? fullPath = url.GetSDStaionImageFullPath();
                    if (fullPath is null || !FileUtil.IsValidFilePath(fullPath))
                    {
                        // test
                        return null;
                    }

                    return checkExists == true && !File.Exists(fullPath)
                        ? null
                        : new ImagePath
                        {
                            ReturnName = Path.GetFileName(fullPath),
                            FullPath = fullPath,
                            SMFileType = SMFileTypes.SDStationLogo
                        };
                }
                catch
                {
                }
            }
            return null;
        }

        if (fileType == SMFileTypes.SDImage)
        {
            if (!url.StartsWith("http"))
            {
                try
                {
                    string? fullPath = url.GetSDImageFullPath();
                    if (fullPath is null || !FileUtil.IsValidFilePath(fullPath))
                    {
                        // test
                        return null;
                    }

                    return checkExists == true && !File.Exists(fullPath)
                        ? null
                        : new ImagePath
                        {
                            ReturnName = Path.GetFileName(fullPath),
                            FullPath = fullPath,
                            SMFileType = SMFileTypes.SDImage
                        };
                }
                catch
                {
                }
            }
            return null;
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
        //_ = GetValidImagePath(baseUrl, fileType);
        //_ = GetValidImagePath(baseUrl, SMFileTypes.SDImage);
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

        List<LogoFileDto> test = logos.Where(a => a.Name.ContainsIgnoreCase("GSN") || a.SMFileType == SMFileTypes.CustomPlayList).ToList();
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
                Value = BuildInfo.LogoDefault,
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
                Value = "/images/streammaster_logo.png",
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
