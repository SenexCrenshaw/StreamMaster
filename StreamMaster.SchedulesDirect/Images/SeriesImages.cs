using System.Collections.Concurrent;
using System.Text.Json;

using StreamMaster.Domain.Cache;

namespace StreamMaster.SchedulesDirect.Images;

public class SeriesImages(
    ILogger<SeriesImages> logger,
    IHybridCache<SeriesImages> hybridCache,
    IImageDownloadQueue imageDownloadQueue,
    IOptionsMonitor<SDSettings> sdSettings,
    ISchedulesDirectAPIService schedulesDirectAPI,
    ISchedulesDirectDataService schedulesDirectDataService) : ISeriesImages, IDisposable
{
    private static readonly SemaphoreSlim classSemaphore = new(1, 1);
    private readonly SemaphoreSlim semaphore = new(SchedulesDirect.MaxParallelDownloads);

    public async Task<bool> ProcessArtAsync()
    {
        await classSemaphore.WaitAsync();
        try
        {
            if (!sdSettings.CurrentValue.SeriesImages)
            {
                return true;
            }

            ISchedulesDirectData schedulesDirectData = schedulesDirectDataService.SchedulesDirectData();
            ICollection<SeriesInfo> toProcess = schedulesDirectData.SeriesInfosToProcess.Values;

            int totalObjects = toProcess.Count;
            logger.LogInformation("Entering GetAllSeriesImages() for {totalObjects} series.", totalObjects);

            List<string> seriesImageQueue = [];
            foreach (SeriesInfo series in toProcess)
            {
                if (string.IsNullOrEmpty(series.ProtoTypicalProgram) ||
                    !schedulesDirectData.Programs.TryGetValue(series.ProtoTypicalProgram, out MxfProgram? _))
                {
                    continue;
                }

                bool refresh = ShouldRefreshSeries(series.SeriesId, out string seriesId);

                if (!refresh && await hybridCache.GetAsync(seriesId) is string cachedJson && !string.IsNullOrEmpty(cachedJson))
                {
                    ProcessCachedImages(series, cachedJson);
                    imageDownloadQueue.EnqueueProgramArtworkCollection(series.ArtWorks);
                }
                else
                {
                    seriesImageQueue.Add($"SH{series.SeriesId}0000");
                }
            }

            logger.LogDebug("Found {seriesImageQueueCount} cached/unavailable series image links.", seriesImageQueue.Count);

            if (seriesImageQueue.Count > 0)
            {
                ConcurrentBag<ProgramMetadata> seriesImageResponses = [];
                await DownloadAndProcessImagesAsync(seriesImageQueue, seriesImageResponses).ConfigureAwait(false);
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

    private static void ProcessCachedImages(SeriesInfo series, string cachedJson)
    {
        series.ArtWorks = string.IsNullOrEmpty(cachedJson)
            ? []
            : JsonSerializer.Deserialize<List<ProgramArtwork>>(cachedJson) ?? [];
    }

    private async Task DownloadAndProcessImagesAsync(List<string> seriesImageQueue, ConcurrentBag<ProgramMetadata> seriesImageResponses)
    {
        List<Task> tasks = [];
        int processedCount = 0;

        for (int i = 0; i <= seriesImageQueue.Count / SchedulesDirect.MaxImgQueries; i++)
        {
            int startIndex = i * SchedulesDirect.MaxImgQueries;
            tasks.Add(Task.Run(async () =>
            {
                await semaphore.WaitAsync();
                try
                {
                    int itemCount = Math.Min(seriesImageQueue.Count - startIndex, SchedulesDirect.MaxImgQueries);
                    await schedulesDirectAPI.DownloadImageResponsesAsync(seriesImageQueue, seriesImageResponses, startIndex).ConfigureAwait(false);
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
        ISchedulesDirectData schedulesDirectData = schedulesDirectDataService.SchedulesDirectData();

        foreach (ProgramMetadata response in seriesImageResponses)
        {
            if (response.Data == null || response.Data.Count == 0)
            {
                logger.LogWarning("No Series Image artwork found for {ProgramId}", response.ProgramId);
                continue;
            }

            SeriesInfo? series = schedulesDirectData.FindOrCreateSeriesInfo(response.ProgramId.Substring(2, 8));
            if (series == null)
            {
                continue;
            }

            List<ProgramArtwork> artworks = SDHelpers.GetTieredImages(response.Data, artworkSize, ["series", "sport", "episode"], sdSettings.CurrentValue.SeriesPosterAspect);
            series.AddArtwork(artworks);

            if (artworks.Count > 0)
            {
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
