using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using StreamMaster.Domain.Common;
using StreamMaster.Domain.Configuration;
using StreamMaster.Domain.Dto;
using StreamMaster.Domain.Enums;
using StreamMaster.Domain.Extensions;
using StreamMaster.SchedulesDirect.Domain.Interfaces;
using StreamMaster.SchedulesDirect.Domain.JsonClasses;
using StreamMaster.SchedulesDirect.Domain.Models;
using StreamMaster.SchedulesDirect.Helpers;

namespace StreamMaster.Infrastructure.Services.Downloads
{
    public class ImageDownloadService : IHostedService, IDisposable, IImageDownloadService
    {
        private readonly ILogger<ImageDownloadService> logger;
        private readonly IDataRefreshService dataRefreshService;
        private readonly IOptionsMonitor<Setting> _settings;
        private readonly IOptionsMonitor<SDSettings> _sdSettings;
        private readonly ISchedulesDirectAPIService schedulesDirectAPI;
        private readonly IImageDownloadQueue imageDownloadQueue;
        private readonly ISchedulesDirectDataService schedulesDirectDataService;
        private readonly SemaphoreSlim downloadSemaphore;
        private readonly HttpClient httpClient; // Reused HttpClient

        private const int BatchSize = 10;
        private bool exitLoop = false;
        private bool IsActive = false;
        private readonly object _lockObject = new();
        private static DateTime _lastRefreshTime = DateTime.MinValue;
        private static readonly object _refreshLock = new();

        public ImageDownloadServiceStatus imageDownloadServiceStatus { get; } = new();

        public ImageDownloadService(
            ILogger<ImageDownloadService> logger,
            IDataRefreshService dataRefreshService,
            IOptionsMonitor<Setting> settings,
            IOptionsMonitor<SDSettings> sdSettings,
            ISchedulesDirectAPIService schedulesDirectAPI,
            IImageDownloadQueue imageDownloadQueue,
            ISchedulesDirectDataService schedulesDirectDataService)
        {
            this.logger = logger;
            this.dataRefreshService = dataRefreshService;
            _settings = settings;
            _sdSettings = sdSettings;
            this.schedulesDirectAPI = schedulesDirectAPI;
            this.imageDownloadQueue = imageDownloadQueue;
            this.schedulesDirectDataService = schedulesDirectDataService;
            downloadSemaphore = new SemaphoreSlim(_settings.CurrentValue.MaxConcurrentDownloads);
            httpClient = new HttpClient(); // Shared HttpClient instance
        }

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

        private async Task<bool> ProcessQueuesAsync(CancellationToken cancellationToken)
        {
            // Run both processing tasks concurrently
            bool[] results = await Task.WhenAll(
                ProcessProgramMetadataQueue(cancellationToken),
                ProcessNameLogoQueue(cancellationToken)
            ).ConfigureAwait(false);

            // If no items were processed, exit early
            bool isProcessed = results.Any(processed => processed);
            if (!isProcessed)
            {
                return false;
            }

            // Check if both queues are empty
            bool areQueuesEmpty = imageDownloadQueue.IsProgramMetadataQueueEmpty() && imageDownloadQueue.IsNameLogoQueueEmpty();

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

            // Only refresh if items were processed and the throttle time has passed
            if (shouldRefresh || areQueuesEmpty || imageDownloadQueue.ProgramMetadataCount <= BatchSize)
            {
                await dataRefreshService.RefreshDownloadServiceStatus();
            }

            return true;
        }

        private async Task<bool> ProcessProgramMetadataQueue(CancellationToken cancellationToken)
        {
            List<ProgramMetadata> metadataBatch = imageDownloadQueue.GetNextProgramMetadataBatch(BatchSize);
            imageDownloadServiceStatus.TotalProgramMetadata = imageDownloadQueue.ProgramMetadataCount;

            if (metadataBatch.Count == 0)
            {
                return false;
            }

            logger.LogDebug("Processing batch of ProgramMetadata: {Count}", metadataBatch.Count);
            imageDownloadServiceStatus.TotalProgramMetadataDownloadAttempts += metadataBatch.Count;

            List<string> successfullyDownloaded = [];

            foreach (ProgramMetadata metadata in metadataBatch)
            {
                List<ProgramArtwork> artwork = GetArtwork(metadata);
                if (artwork.Count == 0)
                {
                    logger.LogDebug("No artwork to download for ProgramId: {ProgramId}", metadata.ProgramId);
                    imageDownloadServiceStatus.TotalProgramMetadataNoArt++;
                    successfullyDownloaded.Add(metadata.ProgramId);
                    continue;
                }

                bool success = await DownloadProgramMetadataArtworkAsync(artwork, metadata.ProgramId, cancellationToken);
                if (success)
                {
                    successfullyDownloaded.Add(metadata.ProgramId);
                }
            }

            imageDownloadQueue.TryDequeueProgramMetadataBatch(successfullyDownloaded);
            return successfullyDownloaded.Count != 0;
        }

        private async Task<bool> ProcessNameLogoQueue(CancellationToken cancellationToken)
        {
            List<NameLogo> nameLogoBatch = imageDownloadQueue.GetNextNameLogoBatch(BatchSize);
            imageDownloadServiceStatus.TotalNameLogo = imageDownloadQueue.NameLogoCount;

            if (nameLogoBatch.Count == 0)
            {
                return false;
            }

            logger.LogDebug("Processing batch of NameLogos: {Count}", nameLogoBatch.Count);
            imageDownloadServiceStatus.TotalNameLogoDownloadAttempts += nameLogoBatch.Count;

            List<string> successfullyDownloaded = [];

            foreach (NameLogo nameLogo in nameLogoBatch)
            {
                if (!nameLogo.Logo.StartsWith("http"))
                {
                    successfullyDownloaded.Add(nameLogo.Name);
                    continue;
                }

                string? filePath = GetFilePath(nameLogo);
                if (filePath == null || File.Exists(filePath))
                {
                    imageDownloadServiceStatus.TotalNameLogoAlreadyExists++;
                    successfullyDownloaded.Add(nameLogo.Name);
                    continue;
                }

                if (await DownloadImageAsync(nameLogo.Logo, filePath, isSchedulesDirect: false, cancellationToken))
                {
                    imageDownloadServiceStatus.TotalNameLogoSuccessful++;
                    successfullyDownloaded.Add(nameLogo.Name);
                }
                else
                {
                    imageDownloadServiceStatus.TotalNameLogoErrors++;
                }
            }

            imageDownloadQueue.TryDequeueNameLogoBatch(successfullyDownloaded);
            return successfullyDownloaded.Count != 0;
        }

        private List<ProgramArtwork> GetArtwork(ProgramMetadata metadata)
        {
            // Determine artwork size from settings, default to "Md"
            string artworkSize = string.IsNullOrEmpty(_sdSettings.CurrentValue.ArtworkSize) ? "Md" : _sdSettings.CurrentValue.ArtworkSize;
            List<ProgramArtwork> artwork = [];

            // Find the corresponding program using the ProgramId from metadata
            MxfProgram? program = schedulesDirectDataService.AllPrograms.Find(p => p.ProgramId == metadata.ProgramId);

            // If extras (artwork) exist in the program, fetch them
            if (program?.extras != null)
            {
                artwork = program.GetArtWork();
            }

            // If no artwork was found in the program, try fetching from metadata.Data
            if (artwork.Count == 0 && metadata.Data?.Count > 0)
            {
                // Use SDHelpers to get tiered images (series, sport, episode) from metadata
                artwork = SDHelpers.GetTieredImages(metadata.Data, ["series", "sport", "episode"], artworkSize);
            }

            return artwork;
        }

        private static string? GetFilePath(NameLogo nameLogo)
        {
            // If the logo URL is empty or null, return null
            if (string.IsNullOrEmpty(nameLogo.Logo))
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
            string ext = Path.GetExtension(nameLogo.Logo);
            if (string.IsNullOrEmpty(ext))
            {
                ext = fd.DefaultExtension;
            }

            // Generate a unique filename using an MD5 hash of the logo URL
            string fileName = FileUtil.EncodeToMD5(nameLogo.Logo) + ext;

            // Determine a subdirectory based on the first character of the filename for better organization
            string subDir = char.ToLowerInvariant(fileName[0]).ToString();

            // Build the full file path by combining the logo folder, directory location, subdirectory, and filename
            return Path.Combine(BuildInfo.LogoFolder, fd.DirectoryLocation, subDir, fileName);
        }

        private async Task<bool> DownloadProgramMetadataArtworkAsync(List<ProgramArtwork> artwork, string programId, CancellationToken cancellationToken)
        {
            List<string> successfullyDownloaded = [];

            foreach (ProgramArtwork art in artwork)
            {
                await downloadSemaphore.WaitAsync(cancellationToken);
                try
                {
                    string? logoPath = art.Uri.GetSDImageFullPath();
                    if (string.IsNullOrEmpty(logoPath))
                    {
                        imageDownloadServiceStatus.TotalProgramMetadataNoArt++;
                        successfullyDownloaded.Add(programId);
                        continue;
                    }

                    if (File.Exists(logoPath))
                    {
                        imageDownloadServiceStatus.TotalProgramMetadataAlreadyExists++;
                        successfullyDownloaded.Add(programId);
                        continue;
                    }

                    string url = art.Uri.StartsWith("http") ? art.Uri : $"image/{art.Uri}";

                    if (await DownloadImageAsync(url, logoPath, isSchedulesDirect: true, cancellationToken))
                    {
                        imageDownloadServiceStatus.TotalProgramMetadataDownloaded++;
                        successfullyDownloaded.Add(programId);
                    }
                    else
                    {
                        imageDownloadServiceStatus.TotalProgramMetadataErrors++;
                    }
                }
                finally
                {
                    downloadSemaphore.Release();
                }
            }

            if (successfullyDownloaded.Count > 0)
            {
                logger.LogDebug("All artwork for ProgramId: {ProgramId} downloaded", programId);
                imageDownloadQueue.TryDequeueProgramMetadataBatch(successfullyDownloaded);
            }

            return successfullyDownloaded.Count > 0;
        }

        private async Task<bool> DownloadImageAsync(string url, string filePath, bool isSchedulesDirect, CancellationToken cancellationToken)
        {
            try
            {
                using HttpResponseMessage? response = isSchedulesDirect
                    ? await GetSdImage(url)
                    : await httpClient.GetAsync(url, cancellationToken);

                if (response?.IsSuccessStatusCode == true)
                {
                    await using Stream stream = await response.Content.ReadAsStreamAsync(cancellationToken);
                    await using FileStream fileStream = new(filePath, FileMode.Create);
                    await stream.CopyToAsync(fileStream, cancellationToken);
                    return true;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to download image from {Url}", url);
            }
            return false;
        }

        private async Task<HttpResponseMessage?> GetSdImage(string uri)
        {
            return await schedulesDirectAPI.GetSdImage(uri);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            exitLoop = true;
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            downloadSemaphore.Dispose();
            httpClient.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}