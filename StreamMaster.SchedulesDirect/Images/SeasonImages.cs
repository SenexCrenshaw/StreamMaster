using System.Collections.Concurrent;
using System.Text.Json;

namespace StreamMaster.SchedulesDirect.Images;
public class SeasonImages(ILogger<SeasonImages> logger, IEPGCache<SeasonImages> epgCache, IImageDownloadQueue imageDownloadQueue, IOptionsMonitor<SDSettings> intSettings, ISchedulesDirectAPIService schedulesDirectAPI, ISchedulesDirectDataService schedulesDirectDataService) : ISeasonImages
{
    private readonly List<MxfSeason> seasons = [];
    private List<string> seasonImageQueue = [];
    private ConcurrentBag<ProgramMetadata> seasonImageResponses = [];
    private int processedObjects;
    private int totalObjects;

    private readonly SDSettings sdsettings = intSettings.CurrentValue;
    public async Task<bool> GetAllSeasonImages()
    {

        // reset counters
        seasonImageQueue = [];
        seasonImageResponses = [];
        //IncrementNextStage(mxf.SeasonsToProcess.Count);
        if (!sdsettings.SeasonEventImages)
        {
            return true;
        }

        ISchedulesDirectData schedulesDirectData = schedulesDirectDataService.SchedulesDirectData();
        List<MxfSeason> toProcess = schedulesDirectData.SeasonsToProcess;
        // scan through each series in the mxf
        logger.LogInformation("Entering GetAllSeasonImages() for {totalObjects} seasons.", toProcess.Count);
        foreach (MxfSeason season in toProcess)
        {
            string uid = $"{season.SeriesId}_{season.SeasonNumber}";
            if (epgCache.JsonFiles.ContainsKey(uid) && !string.IsNullOrEmpty(epgCache.JsonFiles[uid].Images))
            {
                epgCache.JsonFiles[uid].Current = true;
                //IncrementProgress();
                if (string.IsNullOrEmpty(epgCache.JsonFiles[uid].Images))
                {
                    continue;
                }

                List<ProgramArtwork> artwork;
                using (StringReader reader = new(epgCache.JsonFiles[uid].Images))
                {
                    season.extras.AddOrUpdate("artwork", artwork = JsonSerializer.Deserialize<List<ProgramArtwork>>(reader.ReadToEnd()));
                }

                season.mxfGuideImage = epgCache.GetGuideImageAndUpdateCache(artwork, ImageType.Season);
            }
            else if (!string.IsNullOrEmpty(season.ProtoTypicalProgram))
            {
                if (seasons.Contains(season))
                {
                    continue;
                }
                seasons.Add(season);
                seasonImageQueue.Add(season.ProtoTypicalProgram);
            }
            else
            {
                //IncrementProgress();
            }
        }
        logger.LogDebug($"Found {processedObjects} cached/unavailable season image links.");

        // maximum 500 queries at a time
        if (seasonImageQueue.Count > 0)
        {
            SemaphoreSlim semaphore = new(SchedulesDirect.MaxParallelDownloads, SchedulesDirect.MaxParallelDownloads);
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
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Error downloading season images at {StartIndex}", startIndex);
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                }));
            }

            await Task.WhenAll(tasks).ConfigureAwait(false);

            // Continue with the rest of your processing
            ProcessSeasonImageResponses();
            imageDownloadQueue.EnqueueProgramMetadataCollection(seasonImageResponses);

            if (processedObjects != totalObjects)
            {
                logger.LogWarning("Failed to download and process {FailedCount} season image links.", seasons.Count - processedObjects);
            }
        }

        //if (seasonImageQueue.Count > 0)
        //{
        //    _ = Parallel.For(0, (seasonImageQueue.Count / SchedulesDirect.MaxImgQueries) + 1, new ParallelOptions { MaxDegreeOfParallelism = SchedulesDirect.MaxParallelDownloads }, i =>
        //    {
        //        DownloadImageResponses(seasonImageQueue, seriesImageResponses, i * SchedulesDirect.MaxImgQueries);
        //    });

        //    ProcessSeasonImageResponses();
        //    imageDownloadQueue.EnqueueProgramMetadataCollection(seasonImageResponses);

        //    //await DownloadImages(seasonImageResponses, cancellationToken);
        //    if (processedObjects != totalObjects)
        //    {
        //        logger.LogWarning($"Failed to download and process {seasons.Count - processedObjects} season image links.");
        //    }
        //}
        //UpdateSeasonIcons(schedulesDirectData.SeasonsToProcess);
        logger.LogInformation("Exiting GetAllSeasonImages(). SUCCESS.");
        seasonImageQueue = []; seasonImageResponses = []; seasons.Clear();
        epgCache.SaveCache();
        return true;
    }

    private void ProcessSeasonImageResponses()
    {

        string artworkSize = string.IsNullOrEmpty(sdsettings.ArtworkSize) ? "Md" : sdsettings.ArtworkSize;

        // process request response
        foreach (ProgramMetadata response in seasonImageResponses)
        {
            //IncrementProgress();
            if (response.Data == null)
            {
                continue;
            }

            MxfSeason? season = seasons.SingleOrDefault(arg => arg.ProtoTypicalProgram == response.ProgramId);
            if (season == null)
            {
                continue;
            }

            // get season images
            List<ProgramArtwork> artwork;

            if (season.extras.ContainsKey("artwork"))
            {
                List<ProgramArtwork> arts = season.extras["artwork"];
                arts = arts.Concat(SDHelpers.GetTieredImages(response.Data, ["season"], artworkSize)).ToList();
                season.extras["artwork"] = arts;
                artwork = season.extras["artwork"];
            }
            else
            {
                season.extras.Add("artwork", SDHelpers.GetTieredImages(response.Data, ["season"], artworkSize));
                artwork = season.extras["artwork"];
            }


            // create a season entry in cache
            string uid = $"{season.SeriesId}_{season.SeasonNumber}";
            if (!epgCache.JsonFiles.ContainsKey(uid))
            {
                epgCache.AddAsset(uid, null);
            }

            season.mxfGuideImage = epgCache.GetGuideImageAndUpdateCache(artwork, ImageType.Season, uid);
        }
    }

    public void ResetCache()
    {
        seasons.Clear();
        seasonImageQueue.Clear();
        seasonImageResponses.Clear();
        processedObjects = 0;
        totalObjects = 0;
    }

    public void ClearCache()
    {
        epgCache.ResetCache();
    }
}
