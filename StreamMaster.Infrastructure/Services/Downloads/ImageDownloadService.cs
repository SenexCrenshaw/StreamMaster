using System.Drawing;
using System.Drawing.Imaging;
using System.Net;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using StreamMaster.Domain.Configuration;
using StreamMaster.Domain.Dto;
using StreamMaster.Domain.Enums;
using StreamMaster.Domain.Helpers;
using StreamMaster.SchedulesDirect.Domain.Interfaces;
using StreamMaster.SchedulesDirect.Domain.JsonClasses;

using Svg;

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
                ProcesslogoInfoQueue(cancellationToken)
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

            if (shouldRefresh)
            {
                await dataRefreshService.RefreshDownloadServiceStatus();
                return;
            }

            // Check if both queues are empty
            bool areQueuesEmpty = imageDownloadQueue.IsProgramArtworkQueueEmpty() && imageDownloadQueue.IslogoInfoQueueEmpty();

            // Only refresh if items were processed and the throttle time has passed
            if (areQueuesEmpty || imageDownloadQueue.ProgramArtworkCount <= _settings.CurrentValue.MaxConcurrentDownloads || imageDownloadQueue.logoInfoCount <= _settings.CurrentValue.MaxConcurrentDownloads)
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

            bool success = await DownloadProgramMetadataArtworkAsync(metadataBatch, cancellationToken);

            //await RefreshDownloadServiceAsync();

            ImageDownloadServiceStatus.TotalProgramMetadata = imageDownloadQueue.ProgramArtworkCount;
            return success;
        }

        private async Task<bool> DownloadProgramMetadataArtworkAsync(List<ProgramArtwork> artwork, CancellationToken cancellationToken)
        {
            //List<string> successfullyDownloaded = [];
            bool needsRefresh = false;
            foreach (ProgramArtwork art in artwork)
            {

                if (!CanProceedWithDownload())
                {
                    return false;
                }
                needsRefresh = true;
                using CancellationTokenSource timeoutCts = new(TimeSpan.FromSeconds(5)); // Set your desired timeout duration
                using CancellationTokenSource linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

                try
                {
                    await downloadSemaphore.WaitAsync(linkedCts.Token);
                }
                catch (OperationCanceledException)
                {
                    //if (timeoutCts.IsCancellationRequested)
                    //{
                    //    // Handle timeout specifically
                    //    ImageDownloadServiceStatus.TotalProgramMetadataErrors++;                        
                    //}
                    downloadSemaphore.Release();
                    return false;
                }
                finally
                {
                    //_ = downloadSemaphore.Release();

                }

                try
                {

                    LogoInfo logoInfo = new(art.Uri, SMFileTypes.ProgramLogo, true);

                    if (string.IsNullOrEmpty(logoInfo.FullPath))
                    {
                        imageDownloadQueue.TryDequeueProgramArtwork(art.Uri);
                        ImageDownloadServiceStatus.TotalProgramMetadataNoArt++;
                        await RefreshDownloadServiceAsync();
                        continue;
                    }

                    if (File.Exists(logoInfo.FullPath))
                    {
                        ImageDownloadServiceStatus.TotalProgramMetadataAlreadyExists++;
                        imageDownloadQueue.TryDequeueProgramArtwork(art.Uri);
                        await RefreshDownloadServiceAsync();
                        continue;
                    }

                    if (await DownloadImageAsync(logoInfo, cancellationToken))
                    {
                        ImageDownloadServiceStatus.TotalProgramMetadataDownloaded++;
                        needsRefresh = true;
                    }
                    else
                    {
                        ImageDownloadServiceStatus.TotalProgramMetadataErrors++;
                    }
                    imageDownloadQueue.TryDequeueProgramArtwork(art.Uri);
                    await RefreshDownloadServiceAsync();
                }
                finally
                {
                    _ = downloadSemaphore.Release();
                    await RefreshDownloadServiceAsync();
                }
            }

            return needsRefresh;
        }

        private async Task<bool> ProcesslogoInfoQueue(CancellationToken cancellationToken)
        {
            List<LogoInfo> logoInfoBatch = imageDownloadQueue.GetNextlogoInfoBatch(_settings.CurrentValue.MaxConcurrentDownloads);
            ImageDownloadServiceStatus.TotallogoInfo = imageDownloadQueue.logoInfoCount;

            if (logoInfoBatch.Count == 0)
            {
                return false;
            }

            logger.LogDebug("Processing batch of logoInfos: {Count}", logoInfoBatch.Count);

            foreach (LogoInfo logoInfo in logoInfoBatch)
            {
                ++ImageDownloadServiceStatus.TotallogoInfoDownloadAttempts;

                if (!logoInfo.Url.StartsWith("http") || string.IsNullOrEmpty(logoInfo.FullPath))
                {
                    ImageDownloadServiceStatus.TotallogoInfoAlreadyExists++;
                    imageDownloadQueue.TryDequeuelogoInfo(logoInfo.Name);
                }
                else
                {
                    if (File.Exists(logoInfo.FullPath))
                    {
                        ImageDownloadServiceStatus.TotallogoInfoAlreadyExists++;
                        imageDownloadQueue.TryDequeuelogoInfo(logoInfo.Name);
                    }
                    else
                    {
                        if (await DownloadImageAsync(logoInfo, cancellationToken))
                        {
                            ImageDownloadServiceStatus.TotallogoInfoSuccessful++;
                            imageDownloadQueue.TryDequeuelogoInfo(logoInfo.Name);
                        }
                        else
                        {
                            ImageDownloadServiceStatus.TotallogoInfoErrors++;
                            imageDownloadQueue.TryDequeuelogoInfo(logoInfo.Name);
                        }
                    }
                }
                await RefreshDownloadServiceAsync();
            }

            ImageDownloadServiceStatus.TotallogoInfo = imageDownloadQueue.logoInfoCount;
            return true;
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

        public async Task<bool> DownloadImageAsync(LogoInfo logoInfo, CancellationToken cancellationToken)
        {
            if (logoInfo.IsSchedulesDirect && !CanProceedWithDownload())
            {
                return false;
            }

            try
            {

                HttpResponseMessage? response = logoInfo.IsSchedulesDirect
    ? await GetSdImage(logoInfo.Url)
    : await httpClient.GetAsync(logoInfo.Url, cancellationToken).ConfigureAwait(false);

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

                        if (logoInfo.IsSVG)
                        {
                            // Save original SVG bytes to a .svg file
                            //string svgPath = Path.ChangeExtension(logoInfo.FullPath, ".svg");
                            //await using (FileStream svgFileStream = new(svgPath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, FileOptions.Asynchronous))
                            //{
                            //    await stream.CopyToAsync(svgFileStream, cancellationToken).ConfigureAwait(false);
                            //}
                            //stream.Position = 0;

                            SvgDocument svgDocument = Svg.SvgDocument.Open<Svg.SvgDocument>(stream);
                            int width = (int)svgDocument.Width.Value;
                            int height = (int)svgDocument.Height.Value;

                            if (svgDocument.ViewBox.Width != 0)
                            {
                                width = (int)svgDocument.ViewBox.Width;
                            }

                            if (svgDocument.ViewBox.Height != 0)
                            {
                                height = (int)svgDocument.ViewBox.Height;
                            }

                            using Bitmap bitmap = new(width, height);
                            using (Graphics graphics = Graphics.FromImage(bitmap))
                            {
                                graphics.Clear(Color.Transparent);
                                svgDocument.Draw(graphics);
                            }


                            bitmap.Save(logoInfo.FullPath, ImageFormat.Png);
                            return true;
                        }
                        else
                        {
                            // Save the original response content to file
                            await using FileStream fileStream = new(logoInfo.FullPath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, FileOptions.Asynchronous);
                            await stream.CopyToAsync(fileStream, cancellationToken).ConfigureAwait(false);
                            return true;
                        }
                    }
                    logger.LogDebug("Failed to download image from {Url} with status code {StatusCode}", logoInfo.Url, response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Failed to download image from {Url} {Message}", logoInfo.Url, ex.InnerException?.Message);
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