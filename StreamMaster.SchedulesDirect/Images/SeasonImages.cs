using System.Collections.Concurrent;
using System.Text.Json;

using StreamMaster.Domain.Cache;

namespace StreamMaster.SchedulesDirect.Images;

public class SeasonImages(
    ILogger<SeasonImages> logger,
    IHybridCache<SeasonImages> hybridCache,
    IImageDownloadQueue imageDownloadQueue,
    IOptionsMonitor<SDSettings> sdSettings,
    ISchedulesDirectAPIService schedulesDirectAPI,
    ISchedulesDirectDataService schedulesDirectDataService) : ISeasonImages, IDisposable
{
    private static readonly SemaphoreSlim classSemaphore = new(1, 1);
    private readonly SemaphoreSlim semaphore = new(SchedulesDirect.MaxParallelDownloads);

    public async Task<bool> ProcessArtAsync()
    {
        await classSemaphore.WaitAsync();
        try
        {
            if (!sdSettings.CurrentValue.SeasonImages)
            {
                return true;
            }

            ISchedulesDirectData schedulesDirectData = schedulesDirectDataService.SchedulesDirectData();
            ICollection<Season> toProcess = schedulesDirectData.SeasonsToProcess.Values;

            int totalObjects = toProcess.Count;
            logger.LogInformation("Entering GetAllSeasonImages() for {totalObjects} seasons.", totalObjects);

            List<string> seasonImageQueue = [];
            foreach (Season season in toProcess)
            {
                string uid = $"{season.SeriesId}_{season.SeasonNumber}";

                string? cachedJson = await hybridCache.GetAsync(uid);
                if (!string.IsNullOrEmpty(cachedJson))
                {
                    ProcessCachedImages(season, cachedJson);
                    imageDownloadQueue.EnqueueProgramArtworkCollection(season.ArtWorks);
                    if (!string.IsNullOrEmpty(season.ProtoTypicalProgram))
                    {
                        MxfProgram mxfProgram = schedulesDirectData.FindOrCreateProgram(season.ProtoTypicalProgram);
                        mxfProgram.AddArtwork(season.ArtWorks);
                    }
                }
                else if (!string.IsNullOrEmpty(season.ProtoTypicalProgram))
                {
                    seasonImageQueue.Add(season.ProtoTypicalProgram);
                }
            }

            logger.LogDebug("Found {processedObjects} cached/unavailable season image links.", seasonImageQueue.Count);

            if (seasonImageQueue.Count > 0)
            {
                ConcurrentBag<ProgramMetadata> seasonImageResponses = [];
                await DownloadAndProcessImagesAsync(seasonImageQueue, seasonImageResponses).ConfigureAwait(false);
                await ProcessSeasonImageResponsesAsync(seasonImageResponses);
            }

            logger.LogInformation("Exiting Season Images SUCCESS.");

            return true;
        }
        finally
        {
            classSemaphore.Release();
        }
    }

    private static void ProcessCachedImages(Season season, string cachedJson)
    {
        season.ArtWorks = string.IsNullOrEmpty(cachedJson)
            ? []
            : JsonSerializer.Deserialize<List<ProgramArtwork>>(cachedJson) ?? [];
    }

    private async Task DownloadAndProcessImagesAsync(List<string> seasonImageQueue, ConcurrentBag<ProgramMetadata> seasonImageResponses)
    {
        List<Task> tasks = [];
        int processedCount = 0;

        for (int i = 0; i <= seasonImageQueue.Count / SchedulesDirect.MaxImgQueries; i++)
        {
            int startIndex = i * SchedulesDirect.MaxImgQueries;
            tasks.Add(Task.Run(async () =>
            {
                await semaphore.WaitAsync();
                try
                {
                    int itemCount = Math.Min(seasonImageQueue.Count - startIndex, SchedulesDirect.MaxImgQueries);
                    await schedulesDirectAPI.DownloadImageResponsesAsync(seasonImageQueue, seasonImageResponses, startIndex).ConfigureAwait(false);
                    Interlocked.Add(ref processedCount, itemCount);
                    logger.LogInformation("Downloaded season images {ProcessedCount} of {TotalCount}", processedCount, seasonImageQueue.Count);
                }
                finally
                {
                    semaphore.Release();
                }
            }));
        }

        await Task.WhenAll(tasks).ConfigureAwait(false);
    }

    private async Task ProcessSeasonImageResponsesAsync(ConcurrentBag<ProgramMetadata> seasonImageResponses)
    {
        string artworkSize = string.IsNullOrEmpty(sdSettings.CurrentValue.ArtworkSize) ? BuildInfo.DefaultSDImageSize : sdSettings.CurrentValue.ArtworkSize;
        ISchedulesDirectData schedulesDirectData = schedulesDirectDataService.SchedulesDirectData();

        foreach (ProgramMetadata response in seasonImageResponses)
        {
            if (response.Data == null || response.Data.Count == 0)
            {
                logger.LogWarning("No Season Image artwork found for {ProgramId}", response.ProgramId);
                continue;
            }

            Season? season = schedulesDirectData.Seasons.Values.FirstOrDefault(arg => arg.ProtoTypicalProgram == response.ProgramId);
            if (season == null)
            {
                continue;
            }

            List<ProgramArtwork> artworks = SDHelpers.GetTieredImages(response.Data, artworkSize, ["season"], sdSettings.CurrentValue.SeriesPosterAspect);
            season.AddArtwork(artworks);

            if (artworks.Count > 0)
            {
                string uid = $"{season.SeriesId}_{season.SeasonNumber}";

                string artworkJson = JsonSerializer.Serialize(artworks);
                await hybridCache.SetAsync(uid, artworkJson);

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
