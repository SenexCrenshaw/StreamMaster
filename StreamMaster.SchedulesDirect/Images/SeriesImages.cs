using System.Collections.Concurrent;
using System.Text.Json;

using StreamMaster.Domain.Cache;
using StreamMaster.SchedulesDirect.Domain;

namespace StreamMaster.SchedulesDirect.Images;

public class SeriesImages(
    ILogger<SeriesImages> logger,
    IImageDownloadQueue imageDownloadQueue,
    IOptionsMonitor<SDSettings> sdSettings,
    ISchedulesDirectAPIService schedulesDirectAPI,
    SMCacheManager<SeriesImages> hybridCache,
    ISchedulesDirectDataService schedulesDirectDataService,
    IProgramRepository programRepository
    ) : ISeriesImages, IDisposable
{
    private static readonly SemaphoreSlim classSemaphore = new(1, 1);
    private readonly SemaphoreSlim semaphore = new(SDAPIConfig.MaxParallelDownloads);

    public async Task<bool> ProcessArtAsync(CancellationToken cancellationToken)
    {
        if (!sdSettings.CurrentValue.SeriesImages)
        {
            return true;
        }

        await classSemaphore.WaitAsync(cancellationToken);
        if (cancellationToken.IsCancellationRequested)
        {
            return false;
        }
        try
        {

            ICollection<SeriesInfo> toProcess = programRepository.SeriesInfos.Values;
            SeriesInfo? test = toProcess.FirstOrDefault(a => a.ProgramId == "EP031891810143");

            if (test is null)
            {
                int aaa = 1;
            }


            int totalObjects = toProcess.Count;
            logger.LogInformation("Entering GetAllSeriesImages() for {totalObjects} series.", totalObjects);

            List<string> seriesImageQueue = [];
            foreach (SeriesInfo seriesInfo in toProcess)
            {
                if (string.IsNullOrEmpty(seriesInfo.ProgramId) ||
                    !programRepository.Programs.TryGetValue(seriesInfo.ProgramId, out MxfProgram? _))
                {
                    continue;
                }

                bool refresh = ShouldRefreshSeries(seriesInfo.SeriesId, out string seriesId);

                if (!refresh)
                {
                    List<ProgramArtwork>? artWorks = await hybridCache.GetAsync<List<ProgramArtwork>>(seriesId);
                    if (artWorks is not null)
                    {
                        seriesInfo.AddArtworks(artWorks);
                        imageDownloadQueue.EnqueueProgramArtworkCollection(artWorks);
                    }
                    else
                    {
                        seriesImageQueue.Add($"SH{seriesInfo.SeriesId}0000");

                    }
                }
                else
                {
                    seriesImageQueue.Add($"SH{seriesInfo.SeriesId}0000");
                }
            }

            logger.LogDebug("Found {seriesImageQueueCount} cached/unavailable series image links.", seriesImageQueue.Count);

            if (seriesImageQueue.Count > 0)
            {
                ConcurrentBag<ProgramMetadata> seriesImageResponses = [];
                await DownloadAndProcessImagesAsync(seriesImageQueue, seriesImageResponses, cancellationToken: cancellationToken).ConfigureAwait(false);
                await ProcessSeriesImageResponsesAsync(seriesImageResponses);
            }

            logger.LogInformation("Exiting Series Images SUCCESS.");

            return true;
        }
        finally
        {
            classSemaphore.Release();
        }
    }

    private bool ShouldRefreshSeries(string seriesId, out string finalSeriesId)
    {
        if (int.TryParse(seriesId, out int digits))
        {
            finalSeriesId = $"SH{seriesId}0000";
            return (digits * sdSettings.CurrentValue.SDStationIds.Count % DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)) + 1 == DateTime.Now.Day;
        }

        finalSeriesId = seriesId;
        return false;
    }
    private async Task DownloadAndProcessImagesAsync(List<string> seriesImageQueue, ConcurrentBag<ProgramMetadata> seriesImageResponses, CancellationToken cancellationToken)
    {
        List<Task> tasks = [];
        int processedCount = 0;

        for (int i = 0; i <= seriesImageQueue.Count / SDAPIConfig.MaxImgQueries; i++)
        {
            int startIndex = i * SDAPIConfig.MaxImgQueries;
            tasks.Add(Task.Run(async () =>
            {
                await semaphore.WaitAsync();
                try
                {
                    int itemCount = Math.Min(seriesImageQueue.Count - startIndex, SDAPIConfig.MaxImgQueries);
                    await schedulesDirectAPI.DownloadImageResponsesAsync(seriesImageQueue, seriesImageResponses, startIndex, cancellationToken).ConfigureAwait(false);
                    Interlocked.Add(ref processedCount, itemCount);
                    logger.LogInformation("Downloaded series images {ProcessedCount} of {TotalCount}", processedCount, seriesImageQueue.Count);
                }
                finally
                {
                    semaphore.Release();
                }
            }));
        }

        await Task.WhenAll(tasks).ConfigureAwait(false);
    }
    private async Task ProcessSeriesImageResponsesAsync(ConcurrentBag<ProgramMetadata> seriesImageResponses)
    {
        string artworkSize = string.IsNullOrEmpty(sdSettings.CurrentValue.ArtworkSize) ? BuildInfo.DefaultSDImageSize : sdSettings.CurrentValue.ArtworkSize;
        ISchedulesDirectData schedulesDirectData = schedulesDirectDataService.SchedulesDirectData;

        foreach (ProgramMetadata response in seriesImageResponses)
        {
            if (response.Data == null || response.Data.Count == 0 || response.Data[0].Code != 0)
            {
                logger.LogWarning("No Series Image artwork found for {ProgramId}", response.ProgramId);
                continue;
            }

            MxfProgram? mfxProgram = programRepository.FindProgram(response.ProgramId);
            if (mfxProgram is null)
            {
                continue;
            }

            SeriesInfo? series = programRepository.FindSeriesInfo(response.ProgramId.Substring(2, 8));
            if (series == null)
            {
                continue;
            }


            List<ProgramArtwork> artworks = SDHelpers.GetTieredImages(response.Data, artworkSize, ["series", "sport", "episode"], sdSettings.CurrentValue.SeriesPosterAspect);
            //series.AddArtworks(artworks);

            if (artworks.Count > 0)
            {
                series.AddArtworks(artworks);
                mfxProgram.AddArtworks(artworks);

                string artworkJson = JsonSerializer.Serialize(artworks);
                await hybridCache.SetAsync(response.ProgramId, artworkJson);
                imageDownloadQueue.EnqueueProgramArtworkCollection(artworks);
            }
            else
            {
                logger.LogWarning("No usable artwork found for {ProgramId}", response.ProgramId);
            }
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            semaphore.Dispose();
        }
    }
    public List<string> GetExpiredKeys()
    {
        return hybridCache.GetExpiredKeysAsync().Result;
    }

    public void RemovedExpiredKeys(List<string>? keysToDelete = null)
    {
    }
}
