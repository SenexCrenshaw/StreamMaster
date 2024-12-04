using System.Collections.Concurrent;
using System.Text.Json;

using StreamMaster.Domain.Cache;
using StreamMaster.SchedulesDirect.Domain;

namespace StreamMaster.SchedulesDirect.Images;

public class SeasonImages(
    ILogger<SeasonImages> logger,
    SMCacheManager<SeasonImages> hybridCache,
    IImageDownloadQueue imageDownloadQueue,
    IOptionsMonitor<SDSettings> sdSettings,
    ISchedulesDirectAPIService schedulesDirectAPI,
    IProgramRepository programRepository,
    ISchedulesDirectDataService schedulesDirectDataService) : ISeasonImages, IDisposable
{
    private static readonly SemaphoreSlim classSemaphore = new(1, 1);
    private readonly SemaphoreSlim semaphore = new(SDAPIConfig.MaxParallelDownloads);

    public async Task<bool> ProcessArtAsync(CancellationToken cancellationToken)
    {
        if (!sdSettings.CurrentValue.SeasonImages)
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


            ICollection<Season> toProcess = programRepository.Seasons.Values;

            int totalObjects = toProcess.Count;
            logger.LogInformation("Entering GetAllSeasonImages() for {totalObjects} seasons.", totalObjects);

            List<string> seasonImageQueue = [];
            foreach (Season season in toProcess)
            {
                string key = $"{season.SeriesId}_{season.SeasonNumber}";
                List<ProgramArtwork>? artWorks = await hybridCache.GetAsync<List<ProgramArtwork>>(key);

                if (artWorks is not null)
                {
                    season.AddArtworks(artWorks);
                    imageDownloadQueue.EnqueueProgramArtworkCollection(artWorks);
                    //if (season.ProgramId.Equals("EP019254150003"))
                    //{
                    //    int aa = 1;
                    //}
                    //if (!string.IsNullOrEmpty(season.ProgramId))
                    //{
                    //    //MxfProgram mxfProgram = schedulesDirectData.FindOrCreateProgram(season.ProgramId);

                    //    MxfProgram? mxfProgram = schedulesDirectData.FindProgram(season.ProgramId);

                    //    if (mxfProgram == null)
                    //    {
                    //        logger.LogWarning("Program {ProgramId} not found in the data store.", season.ProgramId);
                    //        continue;
                    //    }
                    //    mxfProgram.AddArtworks(artWorks);
                    //}
                }
                else if (!string.IsNullOrEmpty(season.ProgramId))
                {
                    seasonImageQueue.Add(season.ProgramId);
                }
            }

            logger.LogDebug("Found {processedObjects} cached/unavailable season image links.", seasonImageQueue.Count);

            if (seasonImageQueue.Count > 0)
            {
                ConcurrentBag<ProgramMetadata> seasonImageResponses = [];
                await DownloadAndProcessImagesAsync(seasonImageQueue, seasonImageResponses, cancellationToken).ConfigureAwait(false);
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



    private async Task DownloadAndProcessImagesAsync(List<string> seasonImageQueue, ConcurrentBag<ProgramMetadata> seasonImageResponses, CancellationToken cancellationToken)
    {
        List<Task> tasks = [];
        int processedCount = 0;

        for (int i = 0; i <= seasonImageQueue.Count / SDAPIConfig.MaxImgQueries; i++)
        {
            int startIndex = i * SDAPIConfig.MaxImgQueries;
            tasks.Add(Task.Run(async () =>
            {
                await semaphore.WaitAsync();
                try
                {
                    int itemCount = Math.Min(seasonImageQueue.Count - startIndex, SDAPIConfig.MaxImgQueries);
                    await schedulesDirectAPI.DownloadImageResponsesAsync(seasonImageQueue, seasonImageResponses, startIndex, cancellationToken).ConfigureAwait(false);
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
        ISchedulesDirectData schedulesDirectData = schedulesDirectDataService.SchedulesDirectData;

        foreach (ProgramMetadata response in seasonImageResponses)
        {
            if (response.ProgramId.Equals("EP019254150003"))
            {
                int aa = 1;

            }

            if (response.Data == null || response.Data.Count == 0 || response.Data[0].Code != 0)
            {
                logger.LogWarning("No Season Image artwork found for {ProgramId}", response.ProgramId);
                continue;
            }

            //MxfProgram? mfxProgram = schedulesDirectData.FindProgram(response.ProgramId);
            //if (mfxProgram is null)
            //{
            //    continue;
            //}

            Season? season = programRepository.Seasons.Values.FirstOrDefault(arg => arg.ProgramId == response.ProgramId);
            if (season == null)
            {
                continue;
            }

            List<ProgramArtwork> artworks = SDHelpers.GetTieredImages(response.Data, artworkSize, ["season"], sdSettings.CurrentValue.SeriesPosterAspect);
            season.AddArtworks(artworks);

            if (artworks.Count > 0)
            {
                //mfxProgram.AddArtworks(artworks);

                string key = $"{season.SeriesId}_{season.SeasonNumber}";

                string artworkJson = JsonSerializer.Serialize(artworks);
                await hybridCache.SetAsync(key, artworkJson);

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
