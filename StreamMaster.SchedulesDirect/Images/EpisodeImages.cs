using System.Collections.Concurrent;
using System.Text.Json;

using StreamMaster.Domain.Cache;
using StreamMaster.SchedulesDirect.Domain;

namespace StreamMaster.SchedulesDirect.Images;

public class EpisodeImages(
    ILogger<EpisodeImages> logger,
    HybridCacheManager<EpisodeImages> EpisodeCache,
    IImageDownloadQueue imageDownloadQueue,
    IOptionsMonitor<SDSettings> sdSettings,
    ISchedulesDirectAPIService schedulesDirectAPI,
    ISchedulesDirectDataService schedulesDirectDataService) : IEpisodeImages, IDisposable
{
    private static readonly SemaphoreSlim classSemaphore = new(1, 1);
    private readonly SemaphoreSlim semaphore = new(SDAPIConfig.MaxParallelDownloads);

    public async Task<bool> ProcessArtAsync(CancellationToken cancellationToken)
    {
        if (!sdSettings.CurrentValue.EpisodeImages)
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

            ISchedulesDirectData schedulesDirectData = schedulesDirectDataService.SchedulesDirectData;

            List<MxfProgram> episodePrograms = schedulesDirectData.Programs.Values
                .Where(p => !p.IsMovie && p.ProgramId.StartsWith("EP"))
                .ToList();

            MxfProgram? a = schedulesDirectData.Programs.Values.FirstOrDefault(a => a.ProgramId == "EP039440321320");
            if (a != null)
            {
                int aa = 1;
            }

            int totalObjects = episodePrograms.Count;

            logger.LogInformation("Entering GetAllEpisodeImages() for {totalObjects} series.", totalObjects);

            List<string> seriesImageQueue = [];
            foreach (MxfProgram episode in episodePrograms)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (episode.ProgramId is "EP039440321320")
                {
                    int aa = 1;
                }

                if (episode.ProgramId is "EP019254150004")
                {
                    int aa2 = 1;
                }

                //if (episode.IsSeries)
                //{
                //    SeriesInfo? series = schedulesDirectData.FindSeriesInfo(episode.ProgramId.Substring(2, 8));
                //    continue;
                //}
                if (string.IsNullOrEmpty(episode.ProgramId) ||
                    !schedulesDirectData.Programs.TryGetValue(episode.ProgramId, out MxfProgram? mfxProgram))
                {
                    continue;
                }

                List<ProgramArtwork>? artWorks = await EpisodeCache.GetAsync<List<ProgramArtwork>>(episode.ProgramId);
                if (artWorks is not null)
                {
                    mfxProgram.AddArtworks(artWorks);
                    imageDownloadQueue.EnqueueProgramArtworkCollection(artWorks);
                }
                else
                {

                    seriesImageQueue.Add(episode.ProgramId);
                }
            }

            logger.LogDebug("Found {seriesImageQueueCount} cached/unavailable series image links.", seriesImageQueue.Count);

            if (seriesImageQueue.Count > 0)
            {
                ConcurrentBag<ProgramMetadata> seriesImageResponses = [];
                await DownloadAndProcessImagesAsync(seriesImageQueue, seriesImageResponses, cancellationToken: cancellationToken).ConfigureAwait(false);
                await ProcessSeriesImageResponsesAsync(seriesImageResponses, cancellationToken);
            }

            logger.LogInformation("Exiting Series Images SUCCESS.");

            return true;
        }
        finally
        {
            classSemaphore.Release();
        }
    }

    private async Task DownloadAndProcessImagesAsync(List<string> seriesImageQueue, ConcurrentBag<ProgramMetadata> seriesImageResponses, CancellationToken cancellationToken)
    {
        List<Task> tasks = [];
        int processedCount = 0;

        for (int i = 0; i <= seriesImageQueue.Count / SDAPIConfig.MaxImgQueries; i++)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }
            int startIndex = i * SDAPIConfig.MaxImgQueries;
            tasks.Add(Task.Run(async () =>
            {
                await semaphore.WaitAsync();
                try
                {
                    int itemCount = Math.Min(seriesImageQueue.Count - startIndex, SDAPIConfig.MaxImgQueries);
                    await schedulesDirectAPI.DownloadImageResponsesAsync(seriesImageQueue, seriesImageResponses, startIndex, cancellationToken).ConfigureAwait(false);
                    Interlocked.Add(ref processedCount, itemCount);
                    logger.LogInformation("Downloaded series images {ProcessedCount} of {TotalCount}", processedCount, seriesImageQueue.Count);
                }
                finally
                {
                    semaphore.Release();
                }
            }, cancellationToken));
        }

        await Task.WhenAll(tasks).ConfigureAwait(false);
    }

    private async Task ProcessSeriesImageResponsesAsync(ConcurrentBag<ProgramMetadata> seriesImageResponses, CancellationToken cancellationToken)
    {
        string artworkSize = string.IsNullOrEmpty(sdSettings.CurrentValue.ArtworkSize) ? BuildInfo.DefaultSDImageSize : sdSettings.CurrentValue.ArtworkSize;
        ISchedulesDirectData schedulesDirectData = schedulesDirectDataService.SchedulesDirectData;

        foreach (ProgramMetadata response in seriesImageResponses)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (response.Data == null || response.Data.Count == 0 || response.Data[0].Code != 0)
            {
                logger.LogWarning("No Series Image artwork found for {ProgramId}", response.ProgramId);
                continue;
            }

            MxfProgram? mfxProgram = schedulesDirectData.FindProgram(response.ProgramId);
            if (mfxProgram is null)
            {
                continue;
            }

            List<ProgramArtwork> artworks = SDHelpers.GetTieredImages(response.Data, artworkSize, ["series", "sport", "episode", "season"], sdSettings.CurrentValue.SeriesPosterAspect);

            if (artworks.Count > 0)
            {
                mfxProgram.AddArtworks(artworks);

                string artworkJson = JsonSerializer.Serialize(artworks);
                await EpisodeCache.SetAsync(response.ProgramId, artworkJson);
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
        return EpisodeCache.GetExpiredKeysAsync().Result;
    }

    public void RemovedExpiredKeys(List<string>? keysToDelete = null)
    {
    }
}
