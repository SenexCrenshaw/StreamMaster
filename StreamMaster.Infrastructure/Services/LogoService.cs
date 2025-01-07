using System.Collections.Concurrent;
using System.Diagnostics;
using System.Web;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using StreamMaster.Application.Common;
using StreamMaster.Application.Common.Extensions;
using StreamMaster.Domain.API;
using StreamMaster.Domain.Configuration;
using StreamMaster.Domain.Crypto;
using StreamMaster.Domain.Dto;
using StreamMaster.Domain.Enums;
using StreamMaster.Domain.Extensions;
using StreamMaster.Domain.Helpers;
using StreamMaster.Domain.Repository;
using StreamMaster.Domain.XmltvXml;
using StreamMaster.PlayList;
using StreamMaster.PlayList.Models;
using StreamMaster.SchedulesDirect.Domain.Interfaces;

namespace StreamMaster.Infrastructure.Services;

public class LogoService(ICustomPlayListBuilder customPlayListBuilder, IHttpContextAccessor httpContextAccessor, IOptionsMonitor<Setting> settings, IOptionsMonitor<CustomLogoDict> customLogos, IImageDownloadService imageDownloadService, IContentTypeProvider mimeTypeProvider, IMemoryCache memoryCache, IImageDownloadQueue imageDownloadQueue, IServiceProvider serviceProvider, IDataRefreshService dataRefreshService, ILogger<LogoService> logger)
    : ILogoService
{
    private ConcurrentDictionary<string, CustomLogoDto> Logos { get; } = [];
    private static readonly SemaphoreSlim scantvLogoSemaphore = new(1, 1);

    public List<XmltvProgramme> GetXmltvProgrammeForPeriod(VideoStreamConfig videoStreamConfig, DateTime startDate, int days, string baseUrl)
    {
        List<(Movie Movie, DateTime StartTime, DateTime EndTime)> movies = customPlayListBuilder.GetMoviesForPeriod(videoStreamConfig.Name, startDate, days);
        List<XmltvProgramme> ret = [];
        foreach ((Movie Movie, DateTime StartTime, DateTime EndTime) x in movies)
        {
            XmltvProgramme programme = XmltvProgrammeConverter.ConvertMovieToXmltvProgramme(x.Movie, videoStreamConfig.EPGId, x.StartTime, x.EndTime);
            if (x.Movie.Thumb is not null && !string.IsNullOrEmpty(x.Movie.Thumb.Text))
            {
                string src = $"/api/files/smChannelLogo/{videoStreamConfig.ChannelNumber}";// GetLogoUrl(x.Movie.Thumb.Text, baseUrl, SMStreamTypeEnum.CustomPlayList);
                programme.Icons = [new XmltvIcon { Src = src }];
            }
            ret.Add(programme);
        }
        return ret;
    }

    #region Custom Logo

    public string AddCustomLogo(string Name, string Source)
    {
        Source = ImageConverter.ConvertDataToPNG(Name, Source);

        customLogos.CurrentValue.AddCustomLogo(Source.ToUrlSafeBase64String(), Name);

        AddLogoToCache(Source, Name, smFileType: SMFileTypes.CustomLogo);

        SettingsHelper.UpdateSetting(customLogos.CurrentValue);
        return Source;
    }

    public void RemoveCustomLogo(string Source)
    {
        if (!ImageConverter.IsCustomSource(Source))
        {
            customLogos.CurrentValue.RemoveProfile(Source);
            SettingsHelper.UpdateSetting(customLogos.CurrentValue);
        }

        string toTest = Source.ToUrlSafeBase64String();
        CustomLogo? test = customLogos.CurrentValue.GetCustomLogo(toTest);
        if (test?.IsReadOnly != false)
        {
            return;
        }
        customLogos.CurrentValue.RemoveProfile(toTest);

        SettingsHelper.UpdateSetting(customLogos.CurrentValue);

        Source = Source.Remove(0, 14);

        ImagePath? imagePath = GetValidImagePath(Source, SMFileTypes.CustomLogo);
        if (imagePath is null)
        {
            return;
        }
        if (File.Exists(imagePath.FullPath))
        {
            File.Delete(imagePath.FullPath);
        }
    }

    #endregion Custom Logo

    public string GetLogoUrl(SMChannel smChannel, string baseUrl)
    {
        return settings.CurrentValue.LogoCache || !smChannel.Logo.StartsWithIgnoreCase("http")
            ? $"{baseUrl}{BuildInfo.PATH_BASE}/api/files/sm/{smChannel.Id}"
            : smChannel.Logo;
    }

    public string GetLogoUrl(SMChannel smChannel)
    {
        string baseUrl = httpContextAccessor.GetUrl();

        return GetLogoUrl(smChannel, baseUrl);
    }

    public string GetLogoUrl(XmltvChannel xmltvChannel)
    {
        string baseUrl = httpContextAccessor.GetUrl();
        return xmltvChannel.Icons is null || xmltvChannel.Icons.Count == 0
            ? "/" + settings.CurrentValue.DefaultLogo
            : GetLogoUrl(xmltvChannel.Id, xmltvChannel.Icons[0].Src, baseUrl);
    }

    private string GetLogoUrl(string Id, string Logo, string baseUrl)
    {
        return settings.CurrentValue.LogoCache || !Logo.StartsWithIgnoreCase("http")
            ? $"{baseUrl}/api/files/sm/{Id}"
            : Logo;
    }

    public async Task<DataResponse<bool>> CacheSMChannelLogosAsync(CancellationToken cancellationToken)
    {
        using IServiceScope scope = serviceProvider.CreateScope();
        ISMChannelService channelService = scope.ServiceProvider.GetRequiredService<ISMChannelService>();

        IQueryable<SMChannel> channelsQuery = channelService.GetSMStreamLogos(true);

        if (!await channelsQuery.AnyAsync(cancellationToken).ConfigureAwait(false))
        {
            return DataResponse.False;
        }

        try
        {
            int degreeOfParallelism = Environment.ProcessorCount;

            await channelsQuery.AsAsyncEnumerable()
                .ForEachAsync(degreeOfParallelism, channel =>
                {
                    if (string.IsNullOrEmpty(channel.Logo) || !channel.Logo.IsValidUrl())
                    {
                        return Task.CompletedTask;
                    }
                    AddLogoToCache(channel.Logo, channel.Name, channel.M3UFileId, SMFileTypes.Logo, OG: true);
                    LogoInfo logoInfo = new(channel.Name, channel.Logo);
                    imageDownloadQueue.EnqueueLogo(logoInfo);
                    return Task.CompletedTask;
                }, cancellationToken)
                .ConfigureAwait(false);

            await dataRefreshService.RefreshLogos().ConfigureAwait(false);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "An error occurred while building logos cache from SM streams.");
            return DataResponse.False;
        }

        return DataResponse.True;
    }

    public async Task<DataResponse<bool>> AddSMStreamLogosAsync(CancellationToken cancellationToken)
    {
        using IServiceScope scope = serviceProvider.CreateScope();
        ISMStreamService smStreamService = scope.ServiceProvider.GetRequiredService<ISMStreamService>();

        IQueryable<SMStream> streamsQuery = smStreamService.GetSMStreamLogos(true);

        if (!await streamsQuery.AnyAsync(cancellationToken).ConfigureAwait(false))
        {
            return DataResponse.False;
        }

        try
        {
            int degreeOfParallelism = Environment.ProcessorCount;

            await streamsQuery.AsAsyncEnumerable()
                .ForEachAsync(degreeOfParallelism, stream =>
                {
                    // Ensure thread safety in AddLogoToCache
                    AddLogoToCache(stream.Logo, stream.Name, stream.M3UFileId, SMFileTypes.Logo, OG: true);
                    return Task.CompletedTask;
                }, cancellationToken)
                .ConfigureAwait(false);

            await dataRefreshService.RefreshLogos().ConfigureAwait(false);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "An error occurred while building logos cache from SM streams.");
            return DataResponse.False;
        }

        return DataResponse.True;
    }

    public async Task<(FileStream? fileStream, string? FileName, string? ContentType)> GetLogoAsync(string fileName, CancellationToken cancellationToken)
    {
        if (fileName.IsRedirect())
        {
            return (null, fileName, null);
        }

        string? imagePath = fileName.GetLogoImageFullPath();

        if (imagePath == null || !File.Exists(imagePath))
        {
            return (null, null, null);
        }

        try
        {
            (FileStream? fileStream, string? FileName, string? ContentType) result = await GetLogoStreamAsync(imagePath, fileName, cancellationToken);
            return result;
        }
        catch
        {
            return (null, null, null);
        }
    }

    public async Task<(FileStream? fileStream, string? FileName, string? ContentType)> GetProgramLogoAsync(string fileName, CancellationToken cancellationToken)
    {
        try
        {
            if (fileName.IsRedirect())
            {
                return (null, fileName, null);
            }

            string? imagePath = fileName.GetProgramLogoFullPath();

            return imagePath == null || !File.Exists(imagePath)
                ? ((FileStream? fileStream, string? FileName, string? ContentType))(null, null, null)
                : await ThingAsync(fileName, imagePath, cancellationToken).ConfigureAwait(false);
        }
        catch
        {
            return (null, null, null);
        }
    }

    public async Task<(FileStream? fileStream, string? FileName, string? ContentType)> GetLogoForChannelAsync(int SMChannelId, CancellationToken cancellationToken)
    {
        IServiceScope scope = serviceProvider.CreateScope();
        IRepositoryWrapper repositoryWrapper = scope.ServiceProvider.GetRequiredService<IRepositoryWrapper>();

        SMChannel? channel = repositoryWrapper.SMChannel.GetSMChannel(SMChannelId);
        if (channel == null || string.IsNullOrEmpty(channel.Logo))
        {
            return (null, null, null);
        }

        if (channel.Logo.IsRedirect())
        {
            return (null, channel.Logo, null);
        }

        string fileName;
        if (channel.Logo.StartsWithIgnoreCase("/api/files/cu/"))
        {
            fileName = channel.Logo.Remove(0, 14);
            return await GetCustomLogoAsync(fileName, cancellationToken);
        }
        else if (channel.Logo.StartsWithIgnoreCase("/api/files/tv/"))
        {
            fileName = channel.Logo.Remove(0, 14);
            return await GetTVLogoAsync(fileName, cancellationToken);
        }
        else
        {
            string test = LogoInfo.Cleanup(channel.Logo);
            fileName = test.StartsWithIgnoreCase("http") ? test.GenerateFNV1aHash() : test;
        }

        (FileStream? fileStream, string? FileName, string? ContentType) ret = await GetLogoAsync(fileName, cancellationToken);

        return ret;
    }

    public async Task<(FileStream? fileStream, string? FileName, string? ContentType)> GetCustomLogoAsync(string Source, CancellationToken cancellationToken)
    {
        string toTest = $"/api/files/cu/{Source}".ToUrlSafeBase64String();

        CustomLogo? test = customLogos.CurrentValue.GetCustomLogo(toTest);
        if (test is null)
        {
            return (null, null, null);
        }

        ImagePath? imagePath = GetValidImagePath(Source, SMFileTypes.CustomLogo);

        if (imagePath == null || !File.Exists(imagePath.FullPath))
        {
            return (null, null, null);
        }

        try
        {
            (FileStream? fileStream, string? FileName, string? ContentType) result = await GetLogoStreamAsync(imagePath.FullPath, Source, cancellationToken);
            return result;
        }
        catch
        {
            return (null, null, null);
        }
    }

    private static async Task<FileStream?> GetFileStreamAsync(string imagePath)
    {
        try
        {
            FileStream fileStream = new(imagePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.Asynchronous);
            await Task.CompletedTask.ConfigureAwait(false);

            return fileStream;
        }
        catch
        {
            return null;
        }
    }

    private async Task<(FileStream? fileStream, string? FileName, string? ContentType)> GetLogoStreamAsync(string imagePath, string fileName, CancellationToken cancellationToken)
    {
        try
        {
            return fileName.IsRedirect() ? ((FileStream? fileStream, string? FileName, string? ContentType))(null, null, null) : await ThingAsync(fileName, imagePath, cancellationToken).ConfigureAwait(false);
        }
        catch
        {
            return (null, null, null);
        }
    }

    private async Task<(FileStream? fileStream, string? FileName, string? ContentType)> ThingAsync(string fileName, string imagePath, CancellationToken cancellationToken)
    {
        try
        {
            if (fileName.IsRedirect())
            {
                return (null, null, null);
            }

            imagePath = imagePath.GetPNGPath();
            fileName = fileName.GetPNGPath();

            if (imagePath == null || !File.Exists(imagePath))
            {
                return (null, null, null);
            }

            FileStream? fileStream = null;

            if (File.Exists(imagePath))
            {
                fileStream = await GetFileStreamAsync(imagePath).ConfigureAwait(false);
            }

            if (fileStream == null)
            {
                LogoInfo logoInfo = new(fileName)
                {
                    IsSchedulesDirect = true
                };
                if (!string.IsNullOrEmpty(logoInfo.FullPath))
                {
                    if (await imageDownloadService.DownloadImageAsync(logoInfo, cancellationToken).ConfigureAwait(false))
                    {
                        fileStream = await GetFileStreamAsync(imagePath).ConfigureAwait(false);
                    }
                }
                if (fileStream == null)
                {
                    return (null, null, null);
                }
            }

            string contentType = GetContentType(fileName);

            // Ensure the file is ready to be read asynchronously
            await Task.CompletedTask.ConfigureAwait(false);

            return (fileStream, fileName, contentType);
        }
        catch
        {
            return (null, null, null);
        }
    }

    public static readonly MemoryCacheEntryOptions NeverRemoveCacheEntryOptions = new MemoryCacheEntryOptions().SetPriority(CacheItemPriority.NeverRemove);

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

            _ = memoryCache.Set(cacheKey, contentType, NeverRemoveCacheEntryOptions);
        }

        return contentType ?? "application/octet-stream";
    }

    private static string? GetCachedFile(string source, SMFileTypes smFileType)
    {
        string? fullPath = source.GetImageFullPath(smFileType);

        return string.IsNullOrEmpty(fullPath) ? null : !File.Exists(fullPath) ? null : fullPath;
    }

    public void CacheLogo(CustomLogoDto logoFile, bool OG = false)
    {
        if (!OG)
        {
            string url = $"/api/files/5/{logoFile.Source}";
            logoFile.Source = url;
        }

        Logos.TryAdd(logoFile.Source, logoFile);
    }

    public void AddLogoToCache(string URL, string title, int FileId = -1, SMFileTypes smFileType = SMFileTypes.Logo, bool OG = false)
    {
        if (string.IsNullOrEmpty(URL))
        {
            return;
        }

        string sourceKey = OG ? URL : URL.GenerateFNV1aHash();

        CustomLogoDto logoFile = new()
        {
            Source = sourceKey,
            Value = URL,
            Name = title
        };

        CacheLogo(logoFile, OG);
    }

    public void ClearLogos()
    {
        Logos.Clear();
    }

    public CustomLogoDto? GetLogoBySource(string source)
    {
        return Logos.TryGetValue(source.GenerateFNV1aHash(), out CustomLogoDto? logo) ? logo : null;
    }

    public ImagePath? GetValidImagePath(string URL, SMFileTypes fileType, bool? checkExists = true)
    {
        string url = HttpUtility.UrlDecode(URL);

        if (fileType == SMFileTypes.CustomLogo)
        {
            string LogoReturnName = Path.GetFileName(url);
            string? cachedFile = GetCachedFile(url, fileType);
            return cachedFile != null
                ? new ImagePath
                {
                    ReturnName = LogoReturnName,
                    FullPath = cachedFile,
                    SMFileType = SMFileTypes.CustomLogo
                }
                : null;
        }

        if (fileType == SMFileTypes.ProgramLogo)
        {
            string LogoReturnName = Path.GetFileName(url);
            string? cachedFile = GetCachedFile(url, fileType);
            return cachedFile != null
                ? new ImagePath
                {
                    ReturnName = LogoReturnName,
                    FullPath = cachedFile,
                    SMFileType = SMFileTypes.Logo
                }
                : null;
        }

        if (fileType == SMFileTypes.Logo)
        {
            string LogoReturnName = Path.GetFileName(url);
            string? cachedFile = GetCachedFile(url, fileType);
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

        if (fileType is SMFileTypes.TvLogo)
        {
            string fullPath = BuildInfo.TVLogoFolder + "/" + URL;
            return File.Exists(fullPath)
                ? new ImagePath
                {
                    ReturnName = Path.GetFileName(fullPath),
                    FullPath = fullPath,
                    SMFileType = SMFileTypes.TvLogo
                }
                : null;
        }

        string returnName;
        if (Logos.TryGetValue(url, out CustomLogoDto? cache))
        {
            returnName = cache.Value;
            string tvLogosFileName = Path.Combine(BuildInfo.TVLogoFolder, returnName);
            return new ImagePath
            {
                ReturnName = returnName,
                FullPath = tvLogosFileName,
                SMFileType = SMFileTypes.TvLogo
            };
        }

        Stopwatch sw = Stopwatch.StartNew();
        CustomLogoDto? logo = GetLogoBySource(url);
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

        returnName = $"{logo.Name}";
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

    public List<CustomLogoDto> GetLogos()
    {
        IEnumerable<CustomLogoDto> masterLogos = customLogos.CurrentValue.GetCustomLogosDto().Concat(Logos.Values);
        IOrderedEnumerable<CustomLogoDto> ret = masterLogos.OrderBy(a => a.Name);
        return [.. ret];
    }

    public async Task<bool> ScanForTvLogosAsync(CancellationToken cancellationToken = default)
    {
        await scantvLogoSemaphore.WaitAsync(cancellationToken);
        try
        {
            FileDefinition fd = FileDefinitions.TVLogo;
            if (!Directory.Exists(fd.DirectoryLocation))
            {
                return false;
            }

            DirectoryInfo dirInfo = new(BuildInfo.TVLogoFolder);

            await UpdateTVLogosFromDirectoryAsync(dirInfo, dirInfo.FullName, cancellationToken).ConfigureAwait(false);

            return true;
        }
        finally
        {
            scantvLogoSemaphore.Release();
        }
    }

    public async Task<(FileStream? fileStream, string? FileName, string? ContentType)> GetTVLogoAsync(string Source, CancellationToken cancellationToken)
    {
        string ext = Path.GetExtension(Source);
        string name = Path.GetFileNameWithoutExtension(Source);
        string toTest = $"/api/files/tv/{Source}";
        CustomLogoDto? test = Logos.Values.FirstOrDefault(a => a.Source == toTest);
        if (test is null)
        {
            return (null, null, null);
        }

        string fileName = name.FromUrlSafeBase64String();

        ImagePath? imagePath = GetValidImagePath(fileName, SMFileTypes.TvLogo);

        if (imagePath == null || !File.Exists(imagePath.FullPath))
        {
            return (null, null, null);
        }

        try
        {
            (FileStream? fileStream, string? FileName, string? ContentType) result = await GetLogoStreamAsync(imagePath.FullPath, Source, cancellationToken);
            return result;
        }
        catch
        {
            return (null, null, null);
        }
    }

    public async Task UpdateTVLogosFromDirectoryAsync(DirectoryInfo dirInfo, string tvLogosLocation, CancellationToken cancellationToken = default)
    {
        foreach (FileInfo file in dirInfo.GetFiles("*png"))
        {
            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }

            string basePath = dirInfo.FullName.Replace(tvLogosLocation, "");
            if (basePath.StartsWith(Path.DirectorySeparatorChar))
            {
                basePath = basePath.Remove(0, 1);
            }

            string? name = Path.GetFileNameWithoutExtension(file.Name);
            if (name is null)
            {
                continue;
            }
            string basename = basePath.Replace(Path.DirectorySeparatorChar, ' ');
            string title = $"{basename}-{name}";

            string url = Path.Combine(basePath, file.Name).ToUrlSafeBase64String();
            url += Path.GetExtension(file.Name);

            string realUrl = $"/api/files/tv/{url}";

            CustomLogoDto tvLogo = new()
            {
                Source = realUrl,
                Value = realUrl,
                Name = title
            };

            CacheLogo(tvLogo, true);
        }

        foreach (DirectoryInfo newDir in dirInfo.GetDirectories())
        {
            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }
            await UpdateTVLogosFromDirectoryAsync(newDir, tvLogosLocation, cancellationToken).ConfigureAwait(false);
        }

        return;
    }

    public void RemoveLogosByM3UFileId(int id)
    {
        foreach (KeyValuePair<string, CustomLogoDto> logo in Logos.Where(a => a.Value.FileId == id))
        {
            _ = Logos.TryRemove(logo.Key, out _);
        }
    }
}