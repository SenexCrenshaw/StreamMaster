using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using StreamMaster.SchedulesDirectAPI.Domain.Interfaces;
using StreamMaster.SchedulesDirectAPI.Domain.JsonClasses;
using StreamMaster.SchedulesDirectAPI.Domain.Models;
using StreamMaster.SchedulesDirectAPI.Helpers;

using StreamMasterApplication.Common.Interfaces;
using StreamMasterApplication.Hubs;

using StreamMasterDomain.Cache;
using StreamMasterDomain.Common;
using StreamMasterDomain.Services;

using System.Collections.Concurrent;
using System.Net;

namespace StreamMasterInfrastructure.Services.Downloads;

public class ImageDownloadService : IHostedService, IDisposable, IImageDownloadService
{
    private readonly ILogger<ImageDownloadService> logger;
    private readonly ISchedulesDirectData schedulesDirectData;
    private readonly ISDToken sDToken;
    private readonly SemaphoreSlim downloadSemaphore;
    private readonly IMemoryCache memoryCache;
    private readonly IHubContext<StreamMasterHub, IStreamMasterHub> hubContext;
    private readonly ConcurrentQueue<ProgramMetadata> downloadQueue = new();
    private bool IsActive = false;
    private readonly object Lock = new();

    public int TotalDownloadAttempts { get; private set; }
    public int TotalInQueue {get => downloadQueue.Count; }
    public int TotalSuccessful { get; private set; }
    public int TotalAlreadyExists { get; private set; }
    public int TotalNoArt { get; private set; }
    public int TotalErrors { get; private set; }

    public ImageDownloadServiceStatus GetStatus()
    {
        return new ImageDownloadServiceStatus
        {
            TotalDownloadAttempts = TotalDownloadAttempts,
            TotalInQueue = TotalInQueue,
            TotalSuccessful = TotalSuccessful,
            TotalAlreadyExists = TotalAlreadyExists,
            TotalNoArt = TotalNoArt,
            TotalErrors = TotalErrors
        };
    } 

    public ImageDownloadService(ILogger<ImageDownloadService> logger, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache, ISchedulesDirectData schedulesDirectData, ISDToken sDToken)
    {
        this.logger = logger;
        this.hubContext = hubContext;
        this.schedulesDirectData = schedulesDirectData;
        this.sDToken = sDToken;
        this.memoryCache = memoryCache;
        var settings = FileUtil.GetSetting();
        downloadSemaphore = new(settings.MaxConcurrentDownloads);        
    }

    private void CheckStart()
    {
        lock (Lock)
        {
            if (IsActive)
            {
                return;
            }

         
            var test = memoryCache.GetSetting();
          
                StartAsync(CancellationToken.None).ConfigureAwait(false);
                IsActive = true;
            
        }
    }
    public void EnqueueProgramMetadataCollection(IEnumerable<ProgramMetadata> metadataCollection)
    {      
        foreach (var metadata in metadataCollection)
        {
            logger.LogDebug("Enqueue Program Metadata for program id {programId}", metadata.ProgramId);
            downloadQueue.Enqueue(metadata);           
        }
        CheckStart();
    }

    // Method to add ProgramMetadata to the download queue
    public void EnqueueProgramMetadata(ProgramMetadata metadata)
    {
        logger.LogDebug("Enqueue Program Metadata for program id {programId}", metadata.ProgramId);
        downloadQueue.Enqueue(metadata);

        CheckStart();       
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            Setting setting = FileUtil.GetSetting();

            if (setting.SDSettings.SDEnabled && !downloadQueue.IsEmpty && memoryCache.IsSystemReady())
            {
                await DownloadImagesAsync(cancellationToken);
            }
                
            // Optionally, introduce a delay before checking the queue again
            await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        // Implement stopping logic if needed
        return Task.CompletedTask;
    }

    private async Task<bool> EnsureToken(CancellationToken cancellationToken)
    {
        return await sDToken.GetTokenAsync(cancellationToken) != null;
    }

    private async Task<bool> DownloadLogo(string uri, string logoPath, CancellationToken cancellationToken)
    {
        logoPath = FileUtil.CleanUpFileName(logoPath);
        ++TotalDownloadAttempts;

        try
        {
            if (File.Exists(logoPath))
            {
                ++TotalAlreadyExists;
                return false;
            }


            if (!await EnsureToken(cancellationToken).ConfigureAwait(false))
            {
                ++TotalErrors;
                return false;
            }

            Setting setting = FileUtil.GetSetting();

            HttpClient httpClient = SDHelpers.CreateHttpClient(setting.ClientUserAgent);

            int maxRetryCount = 1; // Set the maximum number of retries

            for (int retryCount = 0; retryCount <= maxRetryCount; retryCount++)
            {
                HttpResponseMessage response = await httpClient.GetAsync(uri, cancellationToken).ConfigureAwait(false);

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    ++TotalErrors;
                    return false;
                }

                if (response.StatusCode == HttpStatusCode.Forbidden)
                {
                    await sDToken.ResetTokenAsync(cancellationToken);
                  
                    if (retryCount < maxRetryCount)
                    {
                        // Retry the request after resetting the token
                        continue;
                    }
                    else
                    {
                        ++TotalErrors;
                        return false; // Maximum retry attempts reached
                    }
                }

                if (response.IsSuccessStatusCode)
                {
                    Stream stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
                    if (stream != null)
                    {
                        using var outputStream = File.Create(logoPath);
                        await stream.CopyToAsync(outputStream, cancellationToken);
                        stream.Close();
                        stream.Dispose();
                        var fileInfo = new FileInfo(logoPath);
                        if ( fileInfo.Length < 2000)
                        {
                            ++TotalErrors;
                            logger.LogError($"Failed to download image from {uri} to {logoPath}: {response.StatusCode}");
                            return false;
                        }
                        logger.LogDebug($"Downloaded image from {uri} to {logoPath}: {response.StatusCode}");

                        ++TotalSuccessful;
                        return true;
                    }
                }
                else
                {
                    ++TotalErrors;
                    logger.LogError($"Failed to download image from {uri} to {logoPath}: {response.StatusCode}");
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to download image from {Url} to {FileName}.", uri, logoPath);
        }
        ++TotalErrors;
        return false;
    }

    private async Task DownloadImagesAsync(CancellationToken cancellationToken)
    {
                

        while (!cancellationToken.IsCancellationRequested)
        {
            if (!downloadQueue.TryDequeue(out var response))
            {
                // If the queue is empty, break out of the loop and wait for more items
                break;
            }
            try
            {
                string programId = response.ProgramId;
                var artwork = new List<ProgramArtwork>();
                var program = schedulesDirectData.Programs.Find(a => a.ProgramId == programId);
                if (program != null && program.extras != null)
                {
                    artwork = program.GetArtWork();
                }

                if (!artwork.Any())
                {
                    if (response.Data != null && response.Data.Any())
                    {
                        artwork = SDHelpers.GetTieredImages(response.Data, ["series", "sport", "episode"]);
                    }
                    else
                    {
                        ++TotalNoArt;
                        await hubContext.Clients.All.MiscRefresh();
                        return;
                    }
                }

                foreach (ProgramArtwork art in artwork)
                {
                    await downloadSemaphore.WaitAsync(cancellationToken);
                    try
                    {
                        var logoPath = art.Uri.GetSDImageFullPath();
                       
                        string url = "";
                        if (art.Uri.StartsWith("http"))
                        {
                            url = art.Uri;
                        }
                        else
                        {
                            url = await sDToken.GetAPIUrl($"image/{art.Uri}", cancellationToken);
                        }
                        await DownloadLogo(url, logoPath, cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        var a = 1;
                    }
                    finally
                    {
                        await hubContext.Clients.All.MiscRefresh();
                        downloadSemaphore.Release();
                    }
                }
            }
            catch (Exception ex)
            {
                var a = 1;
            }
            finally
            {

            }

        }
    }

   
    public void Dispose()
    {
        downloadSemaphore.Dispose();
        GC.SuppressFinalize(this);
    }
}
