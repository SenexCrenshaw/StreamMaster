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

using System.Net;

namespace StreamMaster.Infrastructure.Services.Downloads
{
    public class ImageDownloadService(ILogger<ImageDownloadService> logger, IDataRefreshService dataRefreshService, IOptionsMonitor<Setting> _settings, IOptionsMonitor<SDSettings> _sdSettings, ISchedulesDirectAPIService schedulesDirectAPI, IImageDownloadQueue imageDownloadQueue, ISchedulesDirectDataService schedulesDirectDataService)
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
        private static DateTime _lastRefreshTime = DateTime.MinValue;
        private static readonly object _refreshLock = new();

        public ImageDownloadServiceStatus imageDownloadServiceStatus { get; } = new();

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
            //bool proccessed = await ProcessProgramMetadataQueue(cancellationToken);
            //bool proccessed2 = await ProcessNameLogoQueue(cancellationToken);

            //if (!proccessed && !proccessed2)
            //{
            //    return;
            //}

            bool[] results = await Task.WhenAll(
    ProcessProgramMetadataQueue(cancellationToken),
    ProcessNameLogoQueue(cancellationToken)
).ConfigureAwait(false);

            if (!results[0] && !results[1])
            {
                return;
            }

            // Throttle the call to RefreshDownloadServiceStatus to once per second
            bool shouldRefresh = false;
            lock (_refreshLock)
            {
                if ((DateTime.UtcNow - _lastRefreshTime).TotalSeconds >= 1)
                {
                    _lastRefreshTime = DateTime.UtcNow;
                    shouldRefresh = true;
                }
            }

            if (shouldRefresh)
            {
                await dataRefreshService.RefreshDownloadServiceStatus();
            }
        }

        private async Task<bool> ProcessProgramMetadataQueue(CancellationToken cancellationToken)
        {
            ProgramMetadata? metadata = imageDownloadQueue.GetNextProgramMetadata();
            imageDownloadServiceStatus.TotalProgramMetadata = imageDownloadQueue.ProgramMetadataCount();
            if (metadata == null)
            {
                return false;
            }

            logger.LogDebug("Processing ProgramMetadata: {ProgramId}", metadata.ProgramId);
            imageDownloadServiceStatus.TotalProgramMetadataDownloadAttempts++;

            // Skip if ImageLockOut is active for Schedules Direct downloads
            if (ImageLockOut)
            {
                logger.LogDebug("Skipping download due to lockout for ProgramId: {ProgramId}", metadata.ProgramId);
                return false;
            }

            // Get and process artwork
            List<ProgramArtwork> artwork = GetArtwork(metadata);
            if (artwork.Count == 0)
            {
                logger.LogDebug("No artwork to download for ProgramId: {ProgramId}", metadata.ProgramId);
                imageDownloadServiceStatus.TotalNoArt++;
                imageDownloadQueue.TryDequeueProgramMetadata(metadata.ProgramId);
                return false;
            }

            return await DownloadArtworkAsync(artwork, metadata.ProgramId, cancellationToken);
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

        private async Task<bool> DownloadArtworkAsync(List<ProgramArtwork> artwork, string programId, CancellationToken cancellationToken)
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
                        imageDownloadServiceStatus.TotalAlreadyExists++;
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

            return deq;
        }

        private async Task<bool> ProcessNameLogoQueue(CancellationToken cancellationToken)
        {
            NameLogo? nameLogo = imageDownloadQueue.GetNextNameLogo();
            imageDownloadServiceStatus.TotalNameLogo = imageDownloadQueue.NameLogoCount();
            if (nameLogo == null)
            {
                return false;
            }

            if (nameLogo.Logo.StartsWith("http") != true)
            {
                imageDownloadQueue.TryDequeueNameLogo(nameLogo.Name);
                return false;
            }


            logger.LogDebug("Processing NameLogo: {Name}", nameLogo.Name);
            imageDownloadServiceStatus.TotalNameLogoDownloadAttempts++;

            string filePath = GetFilePath(nameLogo);
            if (File.Exists(filePath))
            {
                imageDownloadServiceStatus.TotalNameLogoAlreadyExists++;
                imageDownloadQueue.TryDequeueNameLogo(nameLogo.Name);
                return false;
            }

            // Do not enforce ImageLockOut for NameLogo
            if (await DownloadImageAsync(nameLogo.Logo, filePath, isSchedulesDirect: false, cancellationToken))
            {
                imageDownloadServiceStatus.TotalNameLogoSuccessful++;
                imageDownloadQueue.TryDequeueNameLogo(nameLogo.Name);
                return true;
            }

            imageDownloadServiceStatus.TotalNameLogoErrors++;
            return false;

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

        private static string? GetFilePath(NameLogo nameLogo)
        {
            // Ensure the 'Logo' property is not null or empty.
            if (string.IsNullOrEmpty(nameLogo.Logo))
            {
                return null;
            }

            // Retrieve the file definition; return null if not found.
            FileDefinition? fd = FileDefinitions.GetFileDefinition(nameLogo.SMFileType);
            if (fd == null)
            {
                return null;
            }

            // Get the file extension, defaulting if necessary.
            string ext = Path.GetExtension(nameLogo.Logo);
            if (string.IsNullOrEmpty(ext))
            {
                ext = fd.DefaultExtension;
            }

            // Generate the filename using MD5 hash.
            string fileName = FileUtil.EncodeToMD5(nameLogo.Logo) + ext;

            // Determine the subdirectory based on the first character.
            string subDir = char.ToLowerInvariant(fileName[0]).ToString();

            // Build the full path using Path.Combine with multiple arguments.
            return Path.Combine(BuildInfo.LogoFolder, fd.DirectoryLocation, subDir, fileName);
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
