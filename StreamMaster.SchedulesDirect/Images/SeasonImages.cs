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
    private readonly List<Season> seasons = [];
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
        //ResetCache();
        epgCache.SaveCache();
        ClearCache();
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
            season.Extras.AddOrUpdate("artwork", artwork);
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
        string artworkSize = string.IsNullOrEmpty(sdSettings.CurrentValue.ArtworkSize) ? "Md" : sdSettings.CurrentValue.ArtworkSize;

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

            //_ = season.Extras.TryGetValue("artwork", out dynamic? value);

            List<ProgramArtwork> existingArtwork = [];

            //if (season.Extras.ContainsKey("artwork"))
            //{
            //    if (season.Extras["artwork"] is List<ProgramArtwork> artworkList)
            //    {
            //        existingArtwork = artworkList;
            //    }
            //}


            //List<string> test = response.Data.Select(a => a.Tier).Distinct().ToList();
            //List<ProgramArtwork> staples = response.Data.Where(a => a.Tier.ToLower() == "season").ToList();
            //List<ProgramArtwork> staplesMd = staples.Where(a => a.Size == "Md").ToList();
            //List<ProgramArtwork> staplesaspect = staples.Where(a => a.Aspect == sdSettings.CurrentValue.SeriesPosterAspect).ToList();
            //List<ProgramArtwork> staplesMd23 = response.Data.Where(a => a.Category == "Staple" && a.Size == "Md" && a.Aspect == "2x3").ToList();

            //List<ProgramArtwork> testartwork = SDHelpers.GetTieredImages(response.Data, ["episode", "series"], artworkSize);
            //List<ProgramArtwork> artworks = SDHelpers.GetTieredImages(response.Data, ["season"], artworkSize, sdSettings.CurrentValue.SeriesPosterAspect);


            if (!season.Extras.TryGetValue("artwork", out dynamic? value))
            {
                List<ProgramArtwork> artworks = SDHelpers.GetTieredImages(response.Data, ["season"], artworkSize, sdSettings.CurrentValue.SeriesPosterAspect);

                season.Extras["artwork"] = artworks;
            }

            if (season.Extras["artwork"].Count > 0)
            {

                string uid = $"{season.SeriesId}_{season.SeasonNumber}";

                MxfGuideImage? mx = epgCache.GetGuideImageAndUpdateCache(season.Extras["artwork"], ImageType.Season, uid);
                if (mx != null)
                {
                    season.mxfGuideImage = mx;
                }
                if (!epgCache.JsonFiles.ContainsKey(uid))
                {
                    epgCache.AddAsset(uid, null);
                }
                imageDownloadQueue.EnqueueProgramArtworkCollection(season.Extras["artwork"]);
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