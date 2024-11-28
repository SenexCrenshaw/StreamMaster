using System.Collections.Concurrent;
using System.Text.Json;
namespace StreamMaster.SchedulesDirect.Images;

public class MovieImages(ILogger<MovieImages> logger, IEPGCache<MovieImages> epgCache, IImageDownloadQueue imageDownloadQueue, IOptionsMonitor<SDSettings> sdSettings, ISchedulesDirectAPIService schedulesDirectAPI, ISchedulesDirectDataService schedulesDirectDataService)
    : IMovieImages, IDisposable
{
    private readonly List<string> movieImageQueue = [];
    private readonly ConcurrentBag<ProgramMetadata> movieImageResponses = [];
    private readonly SemaphoreSlim semaphore = new(SchedulesDirect.MaxParallelDownloads, SchedulesDirect.MaxParallelDownloads);
    private int processedObjects;
    private int totalObjects;
    private bool disposedValue;

    public async Task<bool> GetAllMoviePosters()
    {
        ISchedulesDirectData schedulesDirectData = schedulesDirectDataService.SchedulesDirectData();
        List<MxfProgram> moviePrograms = schedulesDirectData.ProgramsToProcess.Values
            .Where(p => p.IsMovie && !p.IsAdultOnly)
            .ToList();

        // Reset counters
        movieImageQueue.Clear();
        movieImageResponses.Clear();
        processedObjects = 0;
        totalObjects = moviePrograms.Count;

        logger.LogInformation("Entering GetAllMoviePosters() for {totalObjects} movies.", totalObjects);

        // Check cache and queue missing movie posters
        foreach (MxfProgram mxfProgram in moviePrograms)
        {
            if (!string.IsNullOrEmpty(mxfProgram.ProgramId))
            {
                if (epgCache.JsonFiles.TryGetValue(mxfProgram.ProgramId, out EPGJsonCache? cachedFile))
                {
                    if (cachedFile != null && !string.IsNullOrEmpty(cachedFile.JsonEntry))
                    {
                        cachedFile.SetCurrent();
                        List<ProgramArtwork>? artworks = JsonSerializer.Deserialize<List<ProgramArtwork>>(cachedFile.JsonEntry);
                        mxfProgram.ArtWorks = artworks ?? ([]);
                        imageDownloadQueue.EnqueueProgramArtworkCollection(mxfProgram.ArtWorks);
                    }
                }
            }

            if (mxfProgram.ArtWorks.Count == 0)
            {
                movieImageQueue.Add(mxfProgram.ProgramId);
            }
        }

        logger.LogDebug("Found {processedObjects} cached/unavailable movie poster links.", processedObjects);

        if (movieImageQueue.Count > 0)
        {
            await ProcessMovieImageQueueAsync();
        }

        logger.LogInformation("Exiting Movie Posters SUCCESS.");

        epgCache.SaveCache();
        ClearCache();
        return true;
    }

    private async Task ProcessMovieImageQueueAsync()
    {
        List<Task> tasks = [];
        int processedCount = 0;

        for (int i = 0; i <= movieImageQueue.Count / SchedulesDirect.MaxImgQueries; i++)
        {
            int startIndex = i * SchedulesDirect.MaxImgQueries;
            tasks.Add(Task.Run(async () =>
            {
                await semaphore.WaitAsync();
                try
                {
                    int itemCount = Math.Min(movieImageQueue.Count - startIndex, SchedulesDirect.MaxImgQueries);
                    await schedulesDirectAPI.DownloadImageResponsesAsync(movieImageQueue, movieImageResponses, startIndex).ConfigureAwait(false);
                    int localProcessedCount = Interlocked.Add(ref processedCount, itemCount);
                    logger.LogInformation("Downloaded movie image information {LocalProcessedCount} of {TotalCount}", localProcessedCount, movieImageQueue.Count);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error downloading movie images at {StartIndex}", startIndex);
                }
                finally
                {
                    _ = semaphore.Release();
                }
            }));
        }

        await Task.WhenAll(tasks).ConfigureAwait(false);

        ProcessMovieImageResponses();
    }

    private void ProcessMovieImageResponses()
    {
        string artworkSize = string.IsNullOrEmpty(sdSettings.CurrentValue.ArtworkSize) ? BuildInfo.DefaultSDImageSize : sdSettings.CurrentValue.ArtworkSize;

        foreach (ProgramMetadata response in movieImageResponses)
        {
            ++processedObjects;

            if (response.Data == null || response.Data.Count == 0)
            {
                logger.LogWarning("No Movie Image artwork found for {ProgramId}", response.ProgramId);
                continue;
            }

            ISchedulesDirectData schedulesDirectData = schedulesDirectDataService.SchedulesDirectData();
            MxfProgram mxfProgram = schedulesDirectData.FindOrCreateProgram(response.ProgramId);

            List<ProgramArtwork> artworks = SDHelpers.GetTieredImages(response.Data, artworkSize, ["episode", "series"], sdSettings.CurrentValue.MoviePosterAspect);
            mxfProgram.AddArtwork(artworks);
            epgCache.UpdateProgramArtworkCache(artworks, ImageType.Movie, response.ProgramId);

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
        movieImageQueue.Clear();
        movieImageResponses.Clear();
        processedObjects = 0;
        totalObjects = 0;
    }

    public void ResetCache()
    {
        epgCache.ResetCache();
    }

    // IDisposable implementation to properly clean up resources
    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // Dispose managed resources here
                semaphore?.Dispose();
            }

            // Free unmanaged resources here, if there are any

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Dispose of resources when the object is no longer needed
        Dispose(disposing: true);
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