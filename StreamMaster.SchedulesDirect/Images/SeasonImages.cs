using System.Collections.Concurrent;
using System.Text.Json;
namespace StreamMaster.SchedulesDirect.Images;

public class SeasonImages(
    ILogger<SeasonImages> logger,
    IEPGCache<SeasonImages> epgCache,
    IImageDownloadQueue imageDownloadQueue,
    IOptionsMonitor<SDSettings> sdSettings,
    ISchedulesDirectAPIService schedulesDirectAPI,
    ISchedulesDirectDataService schedulesDirectDataService) : ISeasonImages, IDisposable
{
    private readonly SemaphoreSlim semaphore = new(SchedulesDirect.MaxParallelDownloads);

    private List<string> seasonImageQueue = [];
    private ConcurrentBag<ProgramMetadata> seasonImageResponses = [];
    private readonly ConcurrentDictionary<string, Season> seasons = new();
    private int processedObjects;

    public async Task<bool> GetAllSeasonImages()
    {
        // Reset state
        seasonImageQueue = [];
        seasonImageResponses = [];
        seasons.Clear();
        processedObjects = 0;

        if (!sdSettings.CurrentValue.SeasonImages)
        {
            return true;
        }

        ISchedulesDirectData schedulesDirectData = schedulesDirectDataService.SchedulesDirectData();
        ICollection<Season> toProcess = schedulesDirectData.SeasonsToProcess.Values;

        logger.LogInformation("Entering GetAllSeasonImages() for {totalObjects} seasons.", toProcess.Count);

        foreach (Season season in toProcess)
        {
            string uid = $"{season.SeriesId}_{season.SeasonNumber}";

            if (epgCache.JsonFiles.TryGetValue(uid, out EPGJsonCache? cache) && !string.IsNullOrEmpty(cache.JsonEntry))
            {


                cache.SetCurrent();

                ProcessCachedImages(season, cache);
                imageDownloadQueue.EnqueueProgramArtworkCollection(season.ArtWorks);
                if (!string.IsNullOrEmpty(season.ProtoTypicalProgram))
                {
                    MxfProgram mxfProgram = schedulesDirectData.FindOrCreateProgram(season.ProtoTypicalProgram);
                    mxfProgram.AddArtwork(season.ArtWorks);
                }


                continue;
            }

            if (string.IsNullOrEmpty(season.ProtoTypicalProgram))
            {
                continue;
            }

            seasons.TryAdd(season.Id, season);
            seasonImageQueue.Add(season.ProtoTypicalProgram);
        }

        logger.LogDebug("Found {processedObjects} cached/unavailable season image links.", processedObjects);

        if (seasonImageQueue.Count > 0)
        {
            await DownloadAndProcessImagesAsync().ConfigureAwait(false);
        }

        logger.LogInformation("Exiting Season Images SUCCESS.");

        epgCache.SaveCache();
        ClearCache();
        return true;
    }

    private static void ProcessCachedImages(Season season, EPGJsonCache cachedFile)
    {
        season.ArtWorks = string.IsNullOrEmpty(cachedFile.JsonEntry)
                ? []
                : JsonSerializer.Deserialize<List<ProgramArtwork>>(cachedFile.JsonEntry) ?? [];
    }

    private async Task DownloadAndProcessImagesAsync()
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
                    _ = Interlocked.Add(ref processedCount, itemCount);
                    logger.LogInformation("Downloaded season images {ProcessedCount} of {TotalCount}", processedCount, seasonImageQueue.Count);
                }
                finally
                {
                    _ = semaphore.Release();
                }
            }));
        }

        await Task.WhenAll(tasks).ConfigureAwait(false);
        ProcessSeasonImageResponses();
    }

    private void ProcessSeasonImageResponses()
    {
        string artworkSize = string.IsNullOrEmpty(sdSettings.CurrentValue.ArtworkSize) ? BuildInfo.DefaultSDImageSize : sdSettings.CurrentValue.ArtworkSize;

        foreach (ProgramMetadata response in seasonImageResponses)
        {
            ++processedObjects;

            if (response.Data == null || response.Data.Count == 0)
            {
                logger.LogWarning("No Season Image artwork found for {ProgramId}", response.ProgramId);
                continue;
            }

            List<Season> test = seasons.Values.Where(arg => arg.ProtoTypicalProgram == response.ProgramId).ToList();
            if (test.Count > 1)
            {
                int aaa = 1;
            }
            Season? season = seasons.Values.FirstOrDefault(arg => arg.ProtoTypicalProgram == response.ProgramId);
            if (season == null)
            {
                continue;
            }

            List<ProgramArtwork> artworks = SDHelpers.GetTieredImages(response.Data, artworkSize, ["season"], sdSettings.CurrentValue.SeriesPosterAspect);
            season.AddArtwork(artworks);

            if (artworks.Count > 0)
            {
                string uid = $"{season.SeriesId}_{season.SeasonNumber}";

                if (!epgCache.JsonFiles.ContainsKey(uid))
                {
                    string artworkJson = JsonSerializer.Serialize(artworks);
                    epgCache.AddAsset(uid, artworkJson);
                }
                imageDownloadQueue.EnqueueProgramArtworkCollection(artworks);
            }
        }
    }

    public void ClearCache()
    {
        seasons.Clear();
        seasonImageQueue.Clear();
        seasonImageResponses.Clear();
        processedObjects = 0;
        //totalObjects = 0;
    }

    public void ResetCache()
    {
        epgCache.ResetCache();
    }

    public void Dispose()
    {
        semaphore.Dispose();
        GC.SuppressFinalize(this);
    }

    public List<string> GetExpiredKeys()
    {
        return epgCache.GetExpiredKeys();
    }

    public void RemovedExpiredKeys(List<string>? keysToDelete = null)
    {
        epgCache.RemovedExpiredKeys(keysToDelete);
    }
}