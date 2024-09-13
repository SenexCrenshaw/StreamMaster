using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using StreamMaster.Domain.Common;
using StreamMaster.Domain.Configuration;
using StreamMaster.Domain.Dto;
using StreamMaster.Domain.Extensions;
using StreamMaster.SchedulesDirect.Domain.Interfaces;
using StreamMaster.SchedulesDirect.Domain.JsonClasses;
using StreamMaster.SchedulesDirect.Domain.Models;
using StreamMaster.SchedulesDirect.Helpers;

using System.Net;

namespace StreamMaster.Infrastructure.Services.Downloads
{
    public class ImageDownloadService(ILogger<ImageDownloadService> logger, IOptionsMonitor<Setting> _settings, IOptionsMonitor<SDSettings> _sdSettings, ISchedulesDirectAPIService schedulesDirectAPI, IImageDownloadQueue imageDownloadQueue, ISchedulesDirectDataService schedulesDirectDataService)
        : IHostedService, IDisposable, IImageDownloadService
    {
        //private readonly ILogger<ImageDownloadService> logger;
        private readonly SemaphoreSlim downloadSemaphore = new(_settings.CurrentValue.MaxConcurrentDownloads);
        //private readonly Setting settings;
        //private readonly IImageDownloadQueue imageDownloadQueue;
        //private readonly ISchedulesDirectAPIService schedulesDirectAPI;
        //private readonly ISchedulesDirectDataService schedulesDirectDataService;
        private bool exitLoop = false;
        private bool IsActive = false;  // Used to prevent multiple starts
        private DateTime ImageLockOutDate = DateTime.MinValue;
        private readonly object _lockObject = new();

        public ImageDownloadServiceStatus ImageDownloadServiceStatus { get; } = new();

        //public ImageDownloadService(ILogger<ImageDownloadService> logger, IOptionsMonitor<Setting> settingsMonitor, ISchedulesDirectAPIService schedulesDirectAPI, IImageDownloadQueue imageDownloadQueue, ISchedulesDirectDataService schedulesDirectDataService)
        //{
        //    this.logger = logger;
        //    settings = settingsMonitor.CurrentValue;
        //    this.imageDownloadQueue = imageDownloadQueue;
        //    this.schedulesDirectAPI = schedulesDirectAPI;
        //    this.schedulesDirectDataService = schedulesDirectDataService;
        //    downloadSemaphore = new SemaphoreSlim(settings.MaxConcurrentDownloads);
        //}

        // Handle IsActive to prevent multiple starts
        public void Start()
        {
            lock (_lockObject)
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
                await ProcessQueuesAsync(cancellationToken);
                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }

        private async Task ProcessQueuesAsync(CancellationToken cancellationToken)
        {
            await ProcessProgramMetadataQueue(cancellationToken);
            await ProcessNameLogoQueue(cancellationToken);
        }

        private async Task ProcessProgramMetadataQueue(CancellationToken cancellationToken)
        {
            ProgramMetadata? metadata = imageDownloadQueue.GetNextProgramMetadata();
            if (metadata != null)
            {
                logger.LogDebug("Processing ProgramMetadata: {ProgramId}", metadata.ProgramId);
                ImageDownloadServiceStatus.TotalProgramMetadataDownloadAttempts++;

                // Skip if ImageLockOut is active for Schedules Direct downloads
                if (ImageLockOut)
                {
                    logger.LogDebug("Skipping download due to lockout for ProgramId: {ProgramId}", metadata.ProgramId);
                    return;
                }

                // Get and process artwork
                List<ProgramArtwork> artwork = GetArtwork(metadata);
                if (artwork.Count == 0)
                {
                    logger.LogDebug("No artwork to download for ProgramId: {ProgramId}", metadata.ProgramId);
                    ImageDownloadServiceStatus.TotalNoArt++;
                    imageDownloadQueue.TryDequeueProgramMetadata(metadata.ProgramId);
                    return;
                }

                await DownloadArtworkAsync(artwork, metadata.ProgramId, cancellationToken);
            }
        }

        private List<ProgramArtwork> GetArtwork(ProgramMetadata metadata)
        {
            string artworkSize = string.IsNullOrEmpty(_sdSettings.CurrentValue.ArtworkSize) ? "Md" : _sdSettings.CurrentValue.ArtworkSize;
            List<ProgramArtwork> artwork = [];

            MxfProgram? program = schedulesDirectDataService.AllPrograms.Find(p => p.ProgramId == metadata.ProgramId);
            if (program?.extras != null)
            {
                artwork = program.GetArtWork();
            }

            if (artwork.Count == 0 && metadata.Data?.Count > 0)
            {
                artwork = SDHelpers.GetTieredImages(metadata.Data, ["series", "sport", "episode"], artworkSize);
            }

            return artwork;
        }

        private async Task DownloadArtworkAsync(List<ProgramArtwork> artwork, string programId, CancellationToken cancellationToken)
        {
            bool deq = true;
            foreach (ProgramArtwork art in artwork)
            {
                await downloadSemaphore.WaitAsync(cancellationToken);
                try
                {
                    string? logoPath = art.Uri.GetSDImageFullPath();
                    if (string.IsNullOrEmpty(logoPath) || File.Exists(logoPath))
                    {
                        ImageDownloadServiceStatus.TotalAlreadyExists++;
                        continue;
                    }

                    string url = art.Uri.StartsWith("http") ? art.Uri : $"image/{art.Uri}";

                    if (!await DownloadImageAsync(url, logoPath, isSchedulesDirect: true, cancellationToken))
                    {
                        deq = false;
                    }
                }
                finally
                {
                    downloadSemaphore.Release();
                }
            }

            if (deq)
            {
                logger.LogDebug("All artwork for ProgramId: {ProgramId} downloaded", programId);
                imageDownloadQueue.TryDequeueProgramMetadata(programId);
            }
        }

        private async Task ProcessNameLogoQueue(CancellationToken cancellationToken)
        {
            NameLogo? nameLogo = imageDownloadQueue.GetNextNameLogo();
            if (nameLogo?.Logo.StartsWith("http") != true)
            {
                return;
            }

            logger.LogDebug("Processing NameLogo: {Name}", nameLogo.Name);
            ImageDownloadServiceStatus.TotalNameLogoDownloadAttempts++;

            string filePath = GetFilePath(nameLogo.Logo);
            if (File.Exists(filePath))
            {
                ImageDownloadServiceStatus.TotalNameLogoAlreadyExists++;
                imageDownloadQueue.TryDequeueNameLogo(nameLogo.Name);
                return;
            }

            // Do not enforce ImageLockOut for NameLogo
            if (await DownloadImageAsync(nameLogo.Logo, filePath, isSchedulesDirect: false, cancellationToken))
            {
                ImageDownloadServiceStatus.TotalNameLogoSuccessful++;
                imageDownloadQueue.TryDequeueNameLogo(nameLogo.Name);
            }
            else
            {
                ImageDownloadServiceStatus.TotalNameLogoErrors++;
            }
        }

        // Set ImageLockOut when hitting rate limits and downloading images
        private async Task<bool> DownloadImageAsync(string url, string filePath, bool isSchedulesDirect, CancellationToken cancellationToken)
        {
            try
            {
                HttpResponseMessage? response;

                // Use GetSdImage for Schedules Direct URLs, otherwise fall back to HttpClient for other sources
                if (isSchedulesDirect)
                {
                    response = await GetSdImage(url);
                }
                else
                {
                    using HttpClient client = new();
                    response = await client.GetAsync(url, cancellationToken);
                }

                if (response?.IsSuccessStatusCode == true)
                {
                    await using Stream stream = await response.Content.ReadAsStreamAsync(cancellationToken);
                    await using FileStream fileStream = new(filePath, FileMode.Create);
                    await stream.CopyToAsync(fileStream, cancellationToken);
                    return true;
                }

                if (isSchedulesDirect && response?.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    logger.LogDebug("Schedules Direct download limit reached, activating lockout");
                    ImageLockOutDate = DateTime.UtcNow;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to download image from {Url}", url);
            }

            return false;
        }

        private bool ImageLockOut => DateTime.UtcNow < ImageLockOutDate.AddHours(1);

        private async Task<HttpResponseMessage?> GetSdImage(string uri)
        {
            return await schedulesDirectAPI.GetSdImage(uri);
        }

        private static string GetFilePath(string logoUrl)
        {
            string fileName = FileUtil.EncodeToMD5(logoUrl) + Path.GetExtension(logoUrl);
            return Path.Combine(BuildInfo.LogoFolder, fileName);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            exitLoop = true;
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            downloadSemaphore.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
