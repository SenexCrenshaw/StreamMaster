using System.Collections.Concurrent;
using System.Text.Json;

namespace StreamMaster.SchedulesDirect.Images;

public class SportsImages : ISportsImages, IDisposable
{
    private readonly ILogger<SportsImages> logger;
    private readonly IEPGCache<SportsImages> epgCache;
    private readonly IImageDownloadQueue imageDownloadQueue;
    private readonly ISchedulesDirectAPIService schedulesDirectAPI;
    private readonly SDSettings sdsettings;
    private readonly SemaphoreSlim semaphore;

    public List<MxfProgram> SportEvents { get; set; } = [];
    private readonly List<string> sportsImageQueue = [];
    private readonly ConcurrentBag<ProgramMetadata> sportsImageResponses = [];
    private int processedObjects;
    private int totalObjects;

    public SportsImages(
        ILogger<SportsImages> logger,
        IEPGCache<SportsImages> epgCache,
        IImageDownloadQueue imageDownloadQueue,
        IOptionsMonitor<SDSettings> intSettings,
        ISchedulesDirectAPIService schedulesDirectAPI)
    {
        this.logger = logger;
        this.epgCache = epgCache;
        this.imageDownloadQueue = imageDownloadQueue;
        this.schedulesDirectAPI = schedulesDirectAPI;
        sdsettings = intSettings.CurrentValue;
        semaphore = new SemaphoreSlim(SchedulesDirect.MaxParallelDownloads);
    }

    public async Task<bool> GetAllSportsImages()
    {
        // Reset counters
        // ResetCache();

        if (!sdsettings.SeasonEventImages)
        {
            return true;
        }

        logger.LogInformation("Entering GetAllSportsImages() for {totalObjects} sports events.", SportEvents.Count);

        foreach (MxfProgram sportEvent in SportEvents)
        {
            if (sportEvent.extras.TryGetValue("md5", out dynamic? md5))
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
        ResetCache();
        epgCache.SaveCache();
        return true;
    }

    private void ProcessCachedImages(MxfProgram sportEvent, EPGJsonCache cachedFile)
    {
        List<ProgramArtwork>? artwork = JsonSerializer.Deserialize<List<ProgramArtwork>>(cachedFile.Images);
        if (artwork != null)
        {
            // Add artwork to the program
            sportEvent.extras.AddOrUpdate("artwork", artwork);
            sportEvent.mxfGuideImage = epgCache.GetGuideImageAndUpdateCache(artwork, ImageType.Program);
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
                    Interlocked.Add(ref processedCount, itemCount);
                    logger.LogInformation("Downloaded sport event images {ProcessedCount} of {TotalCount}", processedCount, sportsImageQueue.Count);
                }
                finally
                {
                    semaphore.Release();
                }
            }));
        }

        await Task.WhenAll(tasks).ConfigureAwait(false);
        ProcessSportsImageResponses();
        imageDownloadQueue.EnqueueProgramMetadataCollection(sportsImageResponses);
    }

    private void ProcessSportsImageResponses()
    {
        string artworkSize = string.IsNullOrEmpty(sdsettings.ArtworkSize) ? "Md" : sdsettings.ArtworkSize;

        foreach (ProgramMetadata response in sportsImageResponses)
        {
            if (response.Data == null)
            {
                continue;
            }

            MxfProgram? program = SportEvents.FirstOrDefault(arg => arg.ProgramId == response.ProgramId);
            if (program == null)
            {
                continue;
            }

            List<ProgramArtwork> artwork = SDHelpers.GetTieredImages(response.Data, ["team event", "sport event"], artworkSize);

            program.extras.AddOrUpdate("artwork", artwork);
            program.mxfGuideImage = epgCache.GetGuideImageAndUpdateCache(artwork, ImageType.Program, program.extras["md5"]);
        }
    }

    public void ResetCache()
    {
        SportEvents.Clear();
        sportsImageQueue.Clear();
        sportsImageResponses.Clear();
        processedObjects = 0;
        totalObjects = 0;
    }

    public void ClearCache()
    {
        epgCache.ResetCache();
    }

    public void Dispose()
    {
        semaphore.Dispose();
        GC.SuppressFinalize(this);
    }
}