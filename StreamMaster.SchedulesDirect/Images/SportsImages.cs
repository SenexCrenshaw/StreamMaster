using System.Collections.Concurrent;
using System.Text.Json;
namespace StreamMaster.SchedulesDirect.Images;

public class SportsImages(
    ILogger<SportsImages> logger,
    IEPGCache<SportsImages> epgCache,
    IImageDownloadQueue imageDownloadQueue,
    IOptionsMonitor<SDSettings> sdSettings,
    ISchedulesDirectAPIService schedulesDirectAPI) : ISportsImages, IDisposable
{
    private readonly SemaphoreSlim semaphore = new(SchedulesDirect.MaxParallelDownloads);

    public List<MxfProgram> SportEvents { get; set; } = [];
    private readonly List<string> sportsImageQueue = [];
    private readonly ConcurrentBag<ProgramMetadata> sportsImageResponses = [];
    private int processedObjects;

    public async Task<bool> GetAllSportsImages()
    {
        if (!sdSettings.CurrentValue.SportsImages)
        {
            return true;
        }

        logger.LogInformation("Entering GetAllSportsImages() for {totalObjects} sports events.", SportEvents.Count);

        foreach (MxfProgram sportEvent in SportEvents)
        {
            if (sportEvent.Extras.TryGetValue("md5", out dynamic? md5))
            {
                if (epgCache.JsonFiles.TryGetValue(md5, out EPGJsonCache? cachedFile))
                {
                    if (cachedFile != null && !string.IsNullOrEmpty(cachedFile.Images))
                    {
                        ProcessCachedImages(sportEvent, cachedFile);
                    }
                }
            }
            else
            {
                sportsImageQueue.Add(sportEvent.ProgramId);
            }
        }

        logger.LogDebug("Found {processedObjects} cached/unavailable sport event image links.", processedObjects);

        if (sportsImageQueue.Count > 0)
        {
            await DownloadAndProcessImagesAsync().ConfigureAwait(false);
        }

        logger.LogInformation("Exiting GetAllSportsImages(). SUCCESS.");
        //ResetCache();
        epgCache.SaveCache();
        ClearCache();
        return true;
    }

    private void ProcessCachedImages(MxfProgram sportEvent, EPGJsonCache cachedFile)
    {
        if (string.IsNullOrEmpty(cachedFile.Images))
        {
            return;
        }

        List<ProgramArtwork>? artwork = JsonSerializer.Deserialize<List<ProgramArtwork>>(cachedFile.Images);
        if (artwork != null)
        {
            // Add artwork to the program
            sportEvent.Extras.AddOrUpdate("artwork", artwork);
            MxfGuideImage? guideImages = epgCache.GetGuideImageAndUpdateCache(artwork, ImageType.Program);
            if (guideImages is not null)
            {
                sportEvent.mxfGuideImage = guideImages;
            }
        }
    }

    private async Task DownloadAndProcessImagesAsync()
    {
        List<Task> tasks = [];
        int processedCount = 0;

        for (int i = 0; i <= sportsImageQueue.Count / SchedulesDirect.MaxImgQueries; i++)
        {
            int startIndex = i * SchedulesDirect.MaxImgQueries;
            tasks.Add(Task.Run(async () =>
            {
                await semaphore.WaitAsync();
                try
                {
                    int itemCount = Math.Min(sportsImageQueue.Count - startIndex, SchedulesDirect.MaxImgQueries);
                    await schedulesDirectAPI.DownloadImageResponsesAsync(sportsImageQueue, sportsImageResponses, startIndex).ConfigureAwait(false);
                    _ = Interlocked.Add(ref processedCount, itemCount);
                    logger.LogInformation("Downloaded sport event images {ProcessedCount} of {TotalCount}", processedCount, sportsImageQueue.Count);
                }
                finally
                {
                    _ = semaphore.Release();
                }
            }));
        }

        await Task.WhenAll(tasks).ConfigureAwait(false);
        ProcessSportsImageResponses();
        //imageDownloadQueue.EnqueueProgramMetadataCollection(sportsImageResponses);
    }

    private void ProcessSportsImageResponses()
    {
        string artworkSize = string.IsNullOrEmpty(sdSettings.CurrentValue.ArtworkSize) ? "Md" : sdSettings.CurrentValue.ArtworkSize;

        foreach (ProgramMetadata response in sportsImageResponses)
        {
            if (response.Data == null)
            {
                continue;
            }

            MxfProgram? mxfProgram = SportEvents.FirstOrDefault(arg => arg.ProgramId == response.ProgramId);
            if (mxfProgram == null)
            {
                continue;
            }
            //List<ProgramArtwork> artworks = SDHelpers.GetTieredImages(response.Data, ["team event", "sport event"], artworkSize, sdSettings.CurrentValue.SeriesPosterAspect);

            //if (!mxfProgram.Extras.ContainsKey("artwork"))
            //{
            //    mxfProgram.Extras["artwork"] = new List<ProgramArtwork>();
            //}

            //if (!mxfProgram.Extras.TryGetValue("artwork", out dynamic? value))
            //{
            //    mxfProgram.Extras["artwork"] = artworks;
            //}
            //else
            //{
            //    value.Add(artworks);
            //}

            if (!mxfProgram.Extras.TryGetValue("artwork", out dynamic? value))
            {
                List<ProgramArtwork> artworks = SDHelpers.GetTieredImages(response.Data, ["team event", "sport event"], artworkSize, sdSettings.CurrentValue.SeriesPosterAspect);

                mxfProgram.Extras["artwork"] = artworks;
            }

            if (mxfProgram.Extras["artwork"].Count > 0)
            {
                mxfProgram.mxfGuideImage = epgCache.GetGuideImageAndUpdateCache(mxfProgram.Extras["artwork"], ImageType.Program, mxfProgram.Extras["md5"]);
                imageDownloadQueue.EnqueueProgramArtworkCollection(mxfProgram.Extras["artwork"]);
            }

            //program.Extras.AddOrUpdate("artwork", artwork);

        }
    }

    public void ClearCache()
    {
        SportEvents.Clear();
        sportsImageQueue.Clear();
        sportsImageResponses.Clear();
        processedObjects = 0;
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