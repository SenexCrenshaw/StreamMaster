using System.Collections.Concurrent;
using System.Diagnostics;
using System.Web;

using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using StreamMaster.Domain.API;
using StreamMaster.Domain.Cache;
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
public class LogoService(ICustomPlayListBuilder customPlayListBuilder, IImageDownloadService imageDownloadService, IContentTypeProvider mimeTypeProvider, IMemoryCache memoryCache, IImageDownloadQueue imageDownloadQueue, IServiceProvider serviceProvider, IDataRefreshService dataRefreshService, ILogger<LogoService> logger)
    : ILogoService
{
    private ConcurrentDictionary<string, LogoFileDto> Logos { get; } = [];

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

    public async Task<DataResponse<bool>> AddSMChannelLogosAsync(CancellationToken cancellationToken)
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
                    AddLogo(channel.Logo, channel.Name, channel.M3UFileId, SMFileTypes.Logo, OG: true);
                    LogoInfo logoInfo = new(channel.Name, channel.Logo);
                    imageDownloadQueue.EnqueueLogoInfo(logoInfo);
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
                    // Ensure thread safety in AddLogo
                    AddLogo(stream.Logo, stream.Name, stream.M3UFileId, SMFileTypes.Logo, OG: true);
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

    ///// <summary>
    ///// Extracts the ID and filename from a URL with a constant prefix "/api/files/".
    ///// </summary>
    ///// <param name="url">The input URL.</param>
    ///// <param name="id">The extracted ID as an integer.</param>
    ///// <param name="filename">The extracted filename.</param>
    ///// <returns>True if parsing was successful; otherwise, false.</returns>
    //public static bool TryParseUrl(string url, out int id, out string? filename)
    //{
    //    const string prefix = "/api/files/";
    //    id = 0;
    //    filename = null;

    //    if (string.IsNullOrWhiteSpace(url) || !url.StartsWith(prefix))
    //    {
    //        return false;
    //    }

    //    string remaining = url[prefix.Length..];
    //    string[] parts = remaining.Split('/', 2); // Split into at most 2 parts

    //    // Ensure we have both ID and filename
    //    if (parts.Length == 2 && int.TryParse(parts[0], out id))
    //    {
    //        filename = parts[1];
    //        return true;
    //    }

    //    return false;
    //}

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

        string fileName = channel.Logo.StartsWithIgnoreCase("http") ? channel.Logo.GenerateFNV1aHash() : channel.Logo;

        (FileStream? fileStream, string? FileName, string? ContentType) ret = await GetLogoAsync(fileName, cancellationToken);

        return ret;
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
        string? fullPath = source.GetImageFullPath(smFileType);

        return string.IsNullOrEmpty(fullPath) ? null : !File.Exists(fullPath) ? null : fullPath;
    }

    public void AddLogo(LogoFileDto logoFile, bool OG = false)
    {
        if (!OG)
        {
            string url = $"/api/files/{(int)logoFile.SMFileType}/{logoFile.Source}";
            logoFile.Source = url;
        }

        Logos.TryAdd(logoFile.Source, logoFile);
    }

    public void AddLogo(string URL, string title, int FileId = -1, SMFileTypes smFileType = SMFileTypes.Logo, bool OG = false)
    {
        if (string.IsNullOrEmpty(URL))
        {
            return;
        }

        string sourceKey = OG ? URL : URL.GenerateFNV1aHash();

        LogoFileDto logoFile = new()
        {
            FileId = FileId,
            Source = sourceKey,
            Value = URL,
            SMFileType = smFileType,
            Name = title
        };

        AddLogo(logoFile, OG);
    }

    public void ClearLogos()
    {
        Logos.Clear();
    }

    public LogoFileDto? GetLogoBySource(string source)
    {
        return Logos.TryGetValue(source.GenerateFNV1aHash(), out LogoFileDto? logo) ? logo : null;
    }

    public ImagePath? GetValidImagePath(string URL, SMFileTypes fileType, bool? checkExists = true)
    {
        string url = HttpUtility.UrlDecode(URL);

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

        string returnName;
        if (Logos.TryGetValue(url, out LogoFileDto? cache))
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

    public List<LogoFileDto> GetLogos(SMFileTypes? SMFileType = null)
    {
        ICollection<LogoFileDto> logos;

        if (SMFileType != null)
        {
            logos = Logos.Values.Where(a => a.SMFileType == SMFileType).ToList();
        }
        else
        {
            //logos = mapper.Map<List<LogoFileDto>>(TvLogos.Values);
            logos = Logos.Values;
        }

        //List<LogoFileDto> test = logos.Where(a => a.Name.ContainsIgnoreCase("GSN") || a.SMFileType == SMFileTypes.CustomPlayList).ToList();
        IOrderedEnumerable<LogoFileDto> ret = logos.OrderBy(a => a.Name);

        return [.. ret];
    }

    public async Task<bool> ReadDirectoryTVLogos(CancellationToken cancellationToken = default)
    {
        Logos.TryAdd(BuildInfo.LogoDefault, new LogoFileDto
        {
            Source = BuildInfo.LogoDefault,
            Value = BuildInfo.LogoDefault,
            Name = "Default Logo"
        });

        Logos.TryAdd("/images/streammaster_logo.png", new LogoFileDto
        {
            Source = "/images/streammaster_logo.png",
            Value = "/images/streammaster_logo.png",
            Name = "Stream Master"
        });

        FileDefinition fd = FileDefinitions.TVLogo;
        if (!Directory.Exists(fd.DirectoryLocation))
        {
            return false;
        }

        DirectoryInfo dirInfo = new(BuildInfo.TVLogoFolder);

        await UpdateTVLogosFromDirectoryAsync(dirInfo, dirInfo.FullName, cancellationToken).ConfigureAwait(false);

        return true;
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

            string url = Path.Combine(basePath, file.Name);

            LogoFileDto tvLogo = new()
            {
                Source = url,
                Value = url,
                SMFileType = SMFileTypes.TvLogo,
                Name = title
            };

            AddLogo(tvLogo, true);
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
        foreach (KeyValuePair<string, LogoFileDto> logo in Logos.Where(a => a.Value.FileId == id))
        {
            _ = Logos.TryRemove(logo.Key, out _);
        }
    }
}
