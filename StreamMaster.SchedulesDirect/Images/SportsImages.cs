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
            if (!string.IsNullOrEmpty(sportEvent.MD5))
            {
                if (epgCache.JsonFiles.TryGetValue(sportEvent.MD5, out EPGJsonCache? cachedFile))
                {
                    if (cachedFile != null && !string.IsNullOrEmpty(cachedFile.JsonEntry))
                    {
                        ProcessCachedImages(sportEvent, cachedFile);
                    }
                }
            }

            sportsImageQueue.Add(sportEvent.ProgramId);
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

    private static void ProcessCachedImages(MxfProgram sportEvent, EPGJsonCache cachedFile)
    {
        sportEvent.ArtWorks = string.IsNullOrEmpty(cachedFile.JsonEntry)
              ? []
              : JsonSerializer.Deserialize<List<ProgramArtwork>>(cachedFile.JsonEntry) ?? [];
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
        string artworkSize = string.IsNullOrEmpty(sdSettings.CurrentValue.ArtworkSize) ? BuildInfo.DefaultSDImageSize : sdSettings.CurrentValue.ArtworkSize;

        foreach (ProgramMetadata response in sportsImageResponses)
        {
            ++processedObjects;

            if (response.Data == null || response.Data.Count == 0 || response.Data[0].Response == "INVALID_PROGRAMID")
            {
                logger.LogWarning("No Sport Image artwork found for {ProgramId}", response.ProgramId);
                continue;
            }

            MxfProgram? mxfProgram = SportEvents.FirstOrDefault(arg => arg.ProgramId == response.ProgramId);
            if (mxfProgram == null)
            {
                continue;
            }

            if (!response.ProgramId.StartsWith("SP"))
            {
                continue;
            }

            List<ProgramArtwork> artworks = SDHelpers.GetTieredImages(response.Data, artworkSize, ["team event", "episode", "series", "sport"], sdSettings.CurrentValue.MoviePosterAspect);
            mxfProgram.AddArtwork(artworks);
            epgCache.UpdateProgramArtworkCache(artworks, ImageType.Movie, mxfProgram.MD5);

            if (artworks.Count > 0)
            {
                imageDownloadQueue.EnqueueProgramArtworkCollection(artworks);
            }
            else
            {
                logger.LogWarning("No artwork found for {ProgramId}", response.ProgramId);
            }
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

    public List<string> GetExpiredKeys()
    {
        return epgCache.GetExpiredKeys();
    }

    public void RemovedExpiredKeys(List<string>? keysToDelete = null)
    {
        epgCache.RemovedExpiredKeys(keysToDelete);
    }
}