using System.Collections.Concurrent;
using System.Text.Json;

namespace StreamMaster.SchedulesDirect.Images;

public class SeasonImages(
    ILogger<SeasonImages> logger,
    IEPGCache<SeasonImages> epgCache,
    IImageDownloadQueue imageDownloadQueue,
    IOptionsMonitor<SDSettings> intSettings,
    ISchedulesDirectAPIService schedulesDirectAPI,
    ISchedulesDirectDataService schedulesDirectDataService) : ISeasonImages, IDisposable
{
    private readonly SDSettings sdsettings = intSettings.CurrentValue;
    private readonly SemaphoreSlim semaphore = new(SchedulesDirect.MaxParallelDownloads);

    private List<string> seasonImageQueue = [];
    private ConcurrentBag<ProgramMetadata> seasonImageResponses = [];
    private readonly List<Season> seasons = [];
    private int processedObjects;

    public async Task<bool> GetAllSeasonImages()
    {
        // Reset state
        seasonImageQueue = [];
        seasonImageResponses = [];
        seasons.Clear();
        processedObjects = 0;

        if (!sdsettings.SeasonEventImages)
        {
            return true;
        }

        ISchedulesDirectData schedulesDirectData = schedulesDirectDataService.SchedulesDirectData();
        List<Season> toProcess = schedulesDirectData.SeasonsToProcess;

        logger.LogInformation("Entering GetAllSeasonImages() for {totalObjects} seasons.", toProcess.Count);

        foreach (Season season in toProcess)
        {
            string uid = $"{season.SeriesId}_{season.SeasonNumber}";

            if (epgCache.JsonFiles.TryGetValue(uid, out EPGJsonCache? cache) && !string.IsNullOrEmpty(cache.Images))
            {
                cache.Current = true;
                ProcessCachedImages(season, cache);
            }
            else if (!string.IsNullOrEmpty(season.ProtoTypicalProgram) && !seasons.Contains(season))
            {
                seasons.Add(season);
                seasonImageQueue.Add(season.ProtoTypicalProgram);
            }
        }

        logger.LogDebug("Found {processedObjects} cached/unavailable season image links.", processedObjects);

        if (seasonImageQueue.Count > 0)
        {
            await DownloadAndProcessImagesAsync().ConfigureAwait(false);
        }

        logger.LogInformation("Exiting GetAllSeasonImages(). SUCCESS.");
        ResetCache();
        epgCache.SaveCache();
        return true;
    }

    private void ProcessCachedImages(Season season, EPGJsonCache cache)
    {
        if (string.IsNullOrEmpty(cache.Images))
        {
            return;
        }

        List<ProgramArtwork>? artwork = JsonSerializer.Deserialize<List<ProgramArtwork>>(cache.Images);
        if (artwork != null)
        {
            season.extras.AddOrUpdate("artwork", artwork);
            MxfGuideImage? mx = epgCache.GetGuideImageAndUpdateCache(artwork, ImageType.Season);
            if (mx != null)
            {
                season.mxfGuideImage = mx;
            }
        }
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
        ProcessSeasonImageResponses();
        imageDownloadQueue.EnqueueProgramMetadataCollection(seasonImageResponses);
    }

    private void ProcessSeasonImageResponses()
    {
        string artworkSize = string.IsNullOrEmpty(sdsettings.ArtworkSize) ? "Md" : sdsettings.ArtworkSize;

        foreach (ProgramMetadata response in seasonImageResponses)
        {
            if (response.Data == null)
            {
                continue;
            }

            Season? season = seasons.SingleOrDefault(arg => arg.ProtoTypicalProgram == response.ProgramId);
            if (season == null)
            {
                continue;
            }

            season.extras.TryGetValue("artwork", out dynamic? value);

            List<ProgramArtwork> existingArtwork = [];

            if (season.extras.ContainsKey("artwork"))
            {
                if (season.extras["artwork"] is List<ProgramArtwork> artworkList)
                {
                    existingArtwork = artworkList;
                }
            }

            List<ProgramArtwork> tieredImages = SDHelpers.GetTieredImages(response.Data, ["season"], artworkSize);

            List<ProgramArtwork> artwork = value == null ? tieredImages : [.. existingArtwork, .. tieredImages];

            season.extras["artwork"] = artwork;
            string uid = $"{season.SeriesId}_{season.SeasonNumber}";
            if (!epgCache.JsonFiles.ContainsKey(uid))
            {
                epgCache.AddAsset(uid, null);
            }

            MxfGuideImage? mx = epgCache.GetGuideImageAndUpdateCache(artwork, ImageType.Season, uid);
            if (mx != null)
            {
                season.mxfGuideImage = mx;
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
}