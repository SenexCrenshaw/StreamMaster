using System.Net;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using StreamMaster.Domain.Configuration;
using StreamMaster.Domain.Dto;
using StreamMaster.Domain.Enums;
using StreamMaster.Domain.Extensions;
using StreamMaster.Domain.Helpers;
using StreamMaster.SchedulesDirect.Domain.Interfaces;
using StreamMaster.SchedulesDirect.Domain.JsonClasses;

namespace StreamMaster.Infrastructure.Services.Downloads
{
    public class ImageDownloadService : IHostedService, IImageDownloadService, IDisposable
    {
        private readonly ILogger<ImageDownloadService> logger;
        private readonly IDataRefreshService dataRefreshService;
        private readonly IOptionsMonitor<Setting> _settings;
        private readonly IOptionsMonitor<SDSettings> sdSettings;
        private readonly ISchedulesDirectAPIService schedulesDirectAPI;
        private readonly IImageDownloadQueue imageDownloadQueue;
        private readonly SemaphoreSlim downloadSemaphore;
        private readonly HttpClient httpClient; // Reused HttpClient via factory

        //private readonly object _lockObject = new();
        private static DateTime _lastRefreshTime = DateTime.MinValue;
        private static readonly Lock _refreshLock = new();
        private bool logged429 = false;

        public ImageDownloadServiceStatus ImageDownloadServiceStatus { get; } = new();

        public ImageDownloadService(
            ILogger<ImageDownloadService> logger,
            IHttpClientFactory httpClientFactory, // Use factory for HttpClient
            IDataRefreshService dataRefreshService,
            IOptionsMonitor<Setting> settings,
            IOptionsMonitor<SDSettings> sdSettings,
            ISchedulesDirectAPIService schedulesDirectAPI,
            IImageDownloadQueue imageDownloadQueue)
        {
            this.logger = logger;
            this.dataRefreshService = dataRefreshService;
            _settings = settings;
            this.sdSettings = sdSettings;
            this.schedulesDirectAPI = schedulesDirectAPI;
            this.imageDownloadQueue = imageDownloadQueue;
            downloadSemaphore = new SemaphoreSlim(_settings.CurrentValue.MaxConcurrentDownloads);
            httpClient = httpClientFactory.CreateClient(); // Use HttpClientFactory here
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _ = Task.Run(() => ExecuteAsync(cancellationToken), cancellationToken);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Stopping ImageDownloadService.");
            cancellationToken.ThrowIfCancellationRequested();
            return Task.CompletedTask;
        }

        private async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _ = await ProcessQueuesAsync(stoppingToken).ConfigureAwait(false);
                    await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred in ImageDownloadService.");
                }
            }
        }

        private async Task<bool> ProcessQueuesAsync(CancellationToken cancellationToken)
        {
            // Run both processing tasks concurrently
            bool[] results = await Task.WhenAll(
                ProcessProgramMetadataQueue(cancellationToken),
                ProcessNameLogoQueue(cancellationToken)
            ).ConfigureAwait(false);

            // If no items were processed, exit early
            bool isProcessed = results.Any(processed => processed);
            if (isProcessed)
            {
                await RefreshDownloadServiceAsync();
            }
            return isProcessed;
        }

        private async Task RefreshDownloadServiceAsync()
        {
            // Throttle the refresh logic to run at most once per second
            bool shouldRefresh = false;
            lock (_refreshLock)
            {
                if ((DateTime.UtcNow - _lastRefreshTime).TotalSeconds >= 1)
                {
                    _lastRefreshTime = DateTime.UtcNow;
                    shouldRefresh = true;
                }
            }

            // Check if both queues are empty
            bool areQueuesEmpty = imageDownloadQueue.IsProgramArtworkQueueEmpty() && imageDownloadQueue.IsNameLogoQueueEmpty();

            // Only refresh if items were processed and the throttle time has passed
            if (shouldRefresh || areQueuesEmpty || imageDownloadQueue.ProgramArtworkCount <= _settings.CurrentValue.MaxConcurrentDownloads || imageDownloadQueue.NameLogoCount <= _settings.CurrentValue.MaxConcurrentDownloads)
            {
                await dataRefreshService.RefreshDownloadServiceStatus();
            }
        }

        private async Task<bool> ProcessProgramMetadataQueue(CancellationToken cancellationToken)
        {
            if (!CanProceedWithDownload())
            {
                return false;
            }

            List<ProgramArtwork> metadataBatch = imageDownloadQueue.GetNextProgramArtworkBatch(_settings.CurrentValue.MaxConcurrentDownloads);
            ImageDownloadServiceStatus.TotalProgramMetadata = imageDownloadQueue.ProgramArtworkCount;

            if (metadataBatch.Count == 0)
            {
                return false;
            }

            logger.LogDebug("Processing batch of ProgramMetadata: {Count}", metadataBatch.Count);
            ImageDownloadServiceStatus.TotalProgramMetadataDownloadAttempts += metadataBatch.Count;

            //List<string> successfullyDownloaded = [];

            bool success = await DownloadProgramMetadataArtworkAsync(metadataBatch, cancellationToken);
            //if (success)
            //{
            //    successfullyDownloaded.Add(artWork.Uri);
            //}
            await RefreshDownloadServiceAsync();

            //foreach (ProgramArtwork artWork in metadataBatch)
            //{
            //    if (!CanProceedWithDownload())
            //    {
            //        break;
            //    }

            //    //List<ProgramArtwork> artwork = GetArtwork(artWork);
            //    //if (artWork.Count == 0)
            //    //{
            //    //    //logger.LogDebug("No artwork to download for ProgramId: {ProgramId}", artWork.ProgramId);
            //    //    ImageDownloadServiceStatus.TotalProgramMetadataNoArt++;
            //    //    successfullyDownloaded.Add(artWork.Uri);
            //    //    await RefreshDownloadServiceAsync();
            //    //    continue;
            //    }

            //    bool success = await DownloadProgramMetadataArtworkAsync(artWork, artWork.Uri, cancellationToken);
            //    if (success)
            //    {
            //        successfullyDownloaded.Add(artWork.Uri);
            //    }
            //    await RefreshDownloadServiceAsync();
            //}

            //imageDownloadQueue.TryDequeueProgramArtworkBatch(successfullyDownloaded);
            ImageDownloadServiceStatus.TotalProgramMetadata = imageDownloadQueue.ProgramArtworkCount;
            return success;
        }

        //private List<ProgramArtwork> GetArtwork(ProgramMetadata metadata)
        //{
        //    // Determine artwork size from settings, default to "Md"
        //    string artworkSize = string.IsNullOrEmpty(sdSettings.CurrentValue.ArtworkSize) ? "Md" : sdSettings.CurrentValue.ArtworkSize;
        //    List<ProgramArtwork> artwork = [];

        //    // Find the corresponding program using the ProgramId from metadata
        //    MxfProgram? program = schedulesDirectDataService.AllPrograms.Find(p => p.ProgramId == metadata.ProgramId);

        //    // If Extras (artwork) exist in the program, fetch them
        //    if (program?.Extras != null)
        //    {
        //        artwork = program.GetArtWork();
        //    }

        //    // If no artwork was found in the program, try fetching from metadata.Data
        //    if (artwork.Count == 0 && metadata.Data?.Count > 0)
        //    {
        //        // Use SDHelpers to get tiered images (series, sport, episode) from metadata
        //        artwork = SDHelpers.GetTieredImages(metadata.Data, ["series", "sport", "episode"], artworkSize, sdSettings.CurrentValue.SeriesPosterAspect);
        //    }

        //    return artwork;
        //}

        private async Task<bool> DownloadProgramMetadataArtworkAsync(List<ProgramArtwork> artwork, CancellationToken cancellationToken)
        {
            List<string> successfullyDownloaded = [];

            foreach (ProgramArtwork art in artwork)
            {
                if (!CanProceedWithDownload())
                {
                    return true;
                }

                using CancellationTokenSource timeoutCts = new(TimeSpan.FromSeconds(5)); // Set your desired timeout duration
                using CancellationTokenSource linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

                try
                {
                    // Wait with timeout
                    await downloadSemaphore.WaitAsync(linkedCts.Token);
                }
                catch (OperationCanceledException)
                {
                    //if (timeoutCts.IsCancellationRequested)
                    //{
                    //    // Handle timeout specifically
                    //    ImageDownloadServiceStatus.TotalProgramMetadataErrors++;                        
                    //}
                    ImageDownloadServiceStatus.TotalProgramMetadataErrors++;

                    return false;
                }

                try
                {

                    NameLogo nameLogo = new();
                    string? logoPath = art.Uri.GetSDImageFullPath();
                    if (string.IsNullOrEmpty(logoPath))
                    {
                        ImageDownloadServiceStatus.TotalProgramMetadataNoArt++;
                        successfullyDownloaded.Add(art.Uri);
                        continue;
                    }

                    if (File.Exists(logoPath))
                    {
                        ImageDownloadServiceStatus.TotalProgramMetadataAlreadyExists++;
                        successfullyDownloaded.Add(art.Uri);
                        continue;
                    }

                    string url = art.Uri.StartsWith("http") ? art.Uri : $"image/{art.Uri}";

                    nameLogo.IsSchedulesDirect = true;
                    if (await DownloadImageAsync(nameLogo, cancellationToken))
                    {
                        ImageDownloadServiceStatus.TotalProgramMetadataDownloaded++;
                        successfullyDownloaded.Add(art.Uri);
                    }
                    else
                    {
                        ImageDownloadServiceStatus.TotalProgramMetadataErrors++;
                    }
                }
                finally
                {
                    _ = downloadSemaphore.Release();
                }
            }

            if (successfullyDownloaded.Count > 0)
            {
                imageDownloadQueue.TryDequeueProgramArtworkBatch(successfullyDownloaded);
            }

            return successfullyDownloaded.Count > 0;
        }

        private async Task<bool> ProcessNameLogoQueue(CancellationToken cancellationToken)
        {
            List<NameLogo> nameLogoBatch = imageDownloadQueue.GetNextNameLogoBatch(_settings.CurrentValue.MaxConcurrentDownloads);
            ImageDownloadServiceStatus.TotalNameLogo = imageDownloadQueue.NameLogoCount;

            if (nameLogoBatch.Count == 0)
            {
                return false;
            }

            logger.LogDebug("Processing batch of NameLogos: {Count}", nameLogoBatch.Count);
            ImageDownloadServiceStatus.TotalNameLogoDownloadAttempts += nameLogoBatch.Count;

            List<string> successfullyDownloaded = [];

            foreach (NameLogo nameLogo in nameLogoBatch)
            {
                if (!nameLogo.Url.StartsWith("http") || string.IsNullOrEmpty(nameLogo.FullPath))
                {
                    successfullyDownloaded.Add(nameLogo.Name);
                    continue;
                }

                //string? filePath = GetFilePath(nameLogo);
                if (File.Exists(nameLogo.FullPath))
                {
                    ImageDownloadServiceStatus.TotalNameLogoAlreadyExists++;
                    successfullyDownloaded.Add(nameLogo.Name);
                    continue;
                }

                if (await DownloadImageAsync(nameLogo, cancellationToken))
                {
                    ImageDownloadServiceStatus.TotalNameLogoSuccessful++;
                    successfullyDownloaded.Add(nameLogo.Name);
                }
                else
                {
                    successfullyDownloaded.Add(nameLogo.Name);
                    ImageDownloadServiceStatus.TotalNameLogoErrors++;
                }
                await RefreshDownloadServiceAsync();
            }

            imageDownloadQueue.TryDequeueNameLogoBatch(successfullyDownloaded);
            ImageDownloadServiceStatus.TotalNameLogo = imageDownloadQueue.NameLogoCount;
            return successfullyDownloaded.Count != 0;
        }

        private static string? GetFilePath(NameLogo nameLogo)
        {
            // If the logo URL is empty or null, return null
            if (string.IsNullOrEmpty(nameLogo.Url))
            {
                return null;
            }

            // Retrieve the file definition for the logo's file type
            FileDefinition? fd = FileDefinitions.GetFileDefinition(nameLogo.SMFileType);
            if (fd == null)
            {
                return null; // Return null if no file definition was found
            }

            // Get the file extension, default to the definition's extension if none is provided
            //string ext = Path.GetExtension(nameLogo.SMLogoUrl);
            //if (string.IsNullOrEmpty(ext))
            //{
            //    ext = fd.DefaultExtension;
            //    nameLogo.SMLogoUrl += ext;
            //}

            //string fileName = FileUtil.EncodeToMD5(nameLogo;

            // Determine a subdirectory based on the first character of the filename for better organization
            string subDir = char.ToLowerInvariant(nameLogo.Id[0]).ToString();

            // Build the full file path by combining the logo folder, directory location, subdirectory, and filename
            return Path.Combine(fd.DirectoryLocation, subDir, nameLogo.FileName);
        }

        private DateTime Last429Dt = DateTime.MinValue;
        private bool CanProceedWithDownload()
        {
            if (Last429Dt > DateTime.UtcNow)
            {
                if (!logged429)
                {
                    logger.LogWarning("Image downloads are temporarily suspended until {NoDownloadUntil}", sdSettings.CurrentValue.SDTooManyRequestsSuspend);
                    logged429 = true;
                }
                return false;
            }
            Last429Dt = DateTime.MinValue;
            logged429 = false;
            return true;
        }

        public async Task<bool> DownloadImageAsync(NameLogo nameLogo, CancellationToken cancellationToken)
        {
            if (nameLogo.IsSchedulesDirect && !CanProceedWithDownload())
            {
                return false;
            }

            try
            {
                HttpResponseMessage? response = nameLogo.IsSchedulesDirect
                    ? await GetSdImage(nameLogo.Url)
                    : await httpClient.GetAsync(nameLogo.Url, cancellationToken).ConfigureAwait(false);

                if (response != null)
                {
                    if (response.StatusCode == HttpStatusCode.TooManyRequests)
                    {
                        Last429Dt = DateTime.UtcNow
                            .Date.AddDays(1)
                            .AddMinutes(10);
                        sdSettings.CurrentValue.SDTooManyRequestsSuspend = Last429Dt;

                        SettingsHelper.UpdateSetting(sdSettings.CurrentValue);
                        logger.LogWarning("Max image download limit reached. No more downloads allowed until {NoDownloadUntil}", sdSettings.CurrentValue.SDTooManyRequestsSuspend);
                        return false;
                    }

                    if (response.IsSuccessStatusCode)
                    {
                        await using Stream stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
                        await using FileStream fileStream = new(nameLogo.FullPath, FileMode.Create);
                        await stream.CopyToAsync(fileStream, cancellationToken).ConfigureAwait(false);
                        return true;
                    }
                    logger.LogDebug("Failed to download image from {Url} with status code {StatusCode}", nameLogo.Url, response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Failed to download image from {Url} {Message}", nameLogo.Url, ex.InnerException?.Message);
            }
            return false;
        }

        private async Task<HttpResponseMessage?> GetSdImage(string uri)
        {
            return await schedulesDirectAPI.GetSdImage(uri);
        }

        public void Dispose()
        {
            downloadSemaphore.Dispose();
            httpClient.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}