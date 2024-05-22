using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using StreamMaster.Application.Common.Interfaces;
using StreamMaster.Application.Hubs;
using StreamMaster.Domain.Common;
using StreamMaster.Domain.Configuration;
using StreamMaster.Domain.Extensions;
using StreamMaster.SchedulesDirect.Domain.Interfaces;
using StreamMaster.SchedulesDirect.Domain.JsonClasses;
using StreamMaster.SchedulesDirect.Domain.Models;
using StreamMaster.SchedulesDirect.Helpers;

using System.Net;

namespace StreamMaster.Infrastructure.Services.Downloads;

public class ImageDownloadService : IHostedService, IDisposable, IImageDownloadService
{
    private readonly ILogger<ImageDownloadService> logger;
    private readonly ISchedulesDirectDataService schedulesDirectDataService;
    private readonly SemaphoreSlim downloadSemaphore;
    private readonly IOptionsMonitor<Setting> intsettings;
    private readonly IHubContext<StreamMasterHub, IStreamMasterHub> hubContext;
    private readonly Setting settings;
    private readonly IImageDownloadQueue imageDownloadQueue;
    private readonly ISchedulesDirectAPIService schedulesDirectAPI;

    private bool IsActive = false;
    private bool ImageLockOut => ImageLockOutDate.AddHours(1) >= SMDT.UtcNow;
    private DateTime ImageLockOutDate = DateTime.MinValue;
    private readonly object Lock = new();

    public int TotalDownloadAttempts { get; private set; }
    public int TotalInQueue => imageDownloadQueue.Count();
    public int TotalSuccessful { get; private set; }
    public int TotalAlreadyExists { get; private set; }
    public int TotalNoArt { get; private set; }
    public int TotalErrors { get; private set; }

    private bool exitLoop = false;
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

    public ImageDownloadService(ILogger<ImageDownloadService> logger, IOptionsMonitor<SDSettings> intsdsettings, ISchedulesDirectAPIService schedulesDirectAPI, IImageDownloadQueue imageDownloadQueue, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IOptionsMonitor<Setting> intsettings, ISchedulesDirectDataService schedulesDirectDataService)
    {
        this.logger = logger;
        this.hubContext = hubContext;
        this.schedulesDirectDataService = schedulesDirectDataService;
        this.schedulesDirectAPI = schedulesDirectAPI;
        sdsettings = intsdsettings.CurrentValue;
        this.imageDownloadQueue = imageDownloadQueue;
        settings = intsettings.CurrentValue;

        downloadSemaphore = new(settings.MaxConcurrentDownloads);

    }
    private readonly SDSettings sdsettings;

    public void Start()
    {
        lock (Lock)
        {
            if (IsActive)
            {
                return;
            }
            _ = StartAsync(CancellationToken.None).ConfigureAwait(false);
            IsActive = true;

        }
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested && !exitLoop)
        {

            if (sdsettings.SDEnabled && !imageDownloadQueue.IsEmpty() && BuildInfo.SetIsSystemReady)
            {
                await DownloadImagesAsync(cancellationToken);
            }

            await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        // Implement stopping logic if needed        
        exitLoop = true;
        return Task.CompletedTask;
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

            if (ImageLockOut)
            {
                return false;
            }


            int maxRetryCount = 1; // Set the maximum number of retries

            for (int retryCount = 0; retryCount <= maxRetryCount; retryCount++)
            {
                HttpResponseMessage response = await GetSdImage(uri).ConfigureAwait(false);

                if (response == null)
                {
                    logger.LogDebug("Art download error, response was null");
                    ++TotalErrors;
                    return false;
                }

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    logger.LogDebug("Art download error, could not be found");
                    ++TotalErrors;
                    return false;
                }

                if (response.StatusCode == HttpStatusCode.Forbidden)
                {
                    logger.LogDebug("Art download error, forbidden");
                    ++TotalErrors;
                    return false;
                }


                if (response.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    logger.LogDebug("Art download limit reached, locking out for 24 hours");
                    ImageLockOutDate = SMDT.UtcNow;
                    return false;
                }

                if (response.IsSuccessStatusCode)
                {
                    logger.LogDebug("Art downloaded successfully");
                    Stream stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
                    if (stream != null)
                    {
                        using FileStream outputStream = File.Create(logoPath);
                        await stream.CopyToAsync(outputStream, cancellationToken);
                        stream.Close();
                        stream.Dispose();
                        FileInfo fileInfo = new(logoPath);
                        if (fileInfo.Length < 2000)
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

    private async Task<HttpResponseMessage?> GetSdImage(string uri)
    {
        return await schedulesDirectAPI.GetSdImage(uri);
    }

    private async Task DownloadImagesAsync(CancellationToken cancellationToken)
    {
        string artworkSize = string.IsNullOrEmpty(sdsettings.ArtworkSize) ? "Md" : sdsettings.ArtworkSize;

        while (!cancellationToken.IsCancellationRequested)
        {

            ProgramMetadata? response = imageDownloadQueue.GetNext();

            if (response == null)
            {
                logger.LogDebug("No jobs");
                // If the queue is empty, break out of the loop and wait for more items
                break;
            }

            try
            {
                logger.LogDebug("Got next job from queue");
                string programId = response.ProgramId;
                List<ProgramArtwork> artwork = [];

                MxfProgram? program = schedulesDirectDataService.AllPrograms.Find(a => a.ProgramId == programId);
                if (program != null && program.extras != null)
                {
                    artwork = program.GetArtWork();
                }

                if (artwork.Count == 0)
                {
                    if (response.Data != null && response.Data.Count != 0)
                    {
                        artwork = SDHelpers.GetTieredImages(response.Data, ["series", "sport", "episode"], artworkSize);
                    }
                    else
                    {
                        logger.LogDebug("No artwork to download, removing job");
                        ++TotalNoArt;
                        await hubContext.Clients.All.MiscRefresh();

                        imageDownloadQueue.TryDequeue(response.ProgramId);

                        break;
                    }
                }

                bool deq = true;
                foreach (ProgramArtwork art in artwork)
                {
                    await downloadSemaphore.WaitAsync(cancellationToken);
                    try
                    {
                        string? logoPath = art.Uri.GetSDImageFullPath();
                        if (logoPath == null)
                        {
                            continue;
                        }

                        if (File.Exists(logoPath))
                        {
                            ++TotalAlreadyExists;
                            continue;
                        }

                        string url = art.Uri.StartsWith("http") ? art.Uri : $"image/{art.Uri}";

                        if (!await DownloadLogo(url, logoPath, cancellationToken) && !ImageLockOut)
                        {
                            deq = false;
                        }

                    }
                    catch (Exception ex)
                    {

                    }
                    finally
                    {
                        _ = downloadSemaphore.Release();
                        await hubContext.Clients.All.MiscRefresh();
                    }
                }
                if (deq)
                {
                    logger.LogDebug("All art for job downloaded, removing job");
                    imageDownloadQueue.TryDequeue(response.ProgramId);
                }
                else
                {
                    logger.LogDebug("Some or all art for job has had issues, NOT removing job");
                }
            }
            catch (Exception ex)
            {

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
