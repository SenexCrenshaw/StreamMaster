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
        List<MxfProgram> moviePrograms = schedulesDirectData.ProgramsToProcess
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
            if (mxfProgram.Extras.TryGetValue("md5", out dynamic? md5))
            {
                if (epgCache.JsonFiles.TryGetValue(md5, out EPGJsonCache? cachedFile))
                {
                    if (cachedFile != null && !string.IsNullOrEmpty(cachedFile.Images))
                    {
                        List<ProgramArtwork>? artwork = JsonSerializer.Deserialize<List<ProgramArtwork>>(cachedFile.Images);
                        if (artwork != null)
                        {
                            mxfProgram.Extras["artwork"] = artwork;
                            MxfGuideImage? guideImage = epgCache.GetGuideImageAndUpdateCache(artwork, ImageType.Movie);
                            if (guideImage is not null)
                            {
                                mxfProgram.mxfGuideImage = guideImage;
                            }
                        }

                    }
                }

            }

            movieImageQueue.Add(mxfProgram.ProgramId);

        }

        logger.LogDebug("Found {processedObjects} cached/unavailable movie poster links.", processedObjects);

        if (movieImageQueue.Count > 0)
        {
            await ProcessMovieImageQueueAsync();
        }

        logger.LogInformation("Exiting GetAllMoviePosters(). SUCCESS.");
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
        string artworkSize = string.IsNullOrEmpty(sdSettings.CurrentValue.ArtworkSize) ? "Md" : sdSettings.CurrentValue.ArtworkSize;

        foreach (ProgramMetadata response in movieImageResponses)
        {
            if (response.Data == null)
            {
                continue;
            }

            ISchedulesDirectData schedulesDirectData = schedulesDirectDataService.SchedulesDirectData();
            MxfProgram mxfProgram = schedulesDirectData.FindOrCreateProgram(response.ProgramId);

            //List<string> test = response.Data.Select(a => a.Tier).Distinct().ToList();
            //List<ProgramArtwork> staples = response.Data.Where(a => a.Category == "Staple").ToList();
            //List<ProgramArtwork> staplesMd = response.Data.Where(a => a.Category == "Staple" && a.Size == "Md").ToList();

            //List<ProgramArtwork> staplesMd23 = response.Data.Where(a => a.Category == "Staple" && a.Size == "Md" && a.Aspect == "2x3").ToList();

            //List<ProgramArtwork> testartwork = SDHelpers.GetTieredImages(response.Data, ["episode", "series"], artworkSize);


            if (!mxfProgram.Extras.TryGetValue("artwork", out dynamic? value))
            {
                List<ProgramArtwork> artworks = SDHelpers.GetTieredImages(response.Data, ["episode", "series"], artworkSize, sdSettings.CurrentValue.MoviePosterAspect).ToList();

                mxfProgram.Extras["artwork"] = artworks;
            }
            //else
            //{
            //    value.Add(artworks);
            //}

            if (mxfProgram.Extras["artwork"].Count > 0)
            {
                mxfProgram.mxfGuideImage = epgCache.GetGuideImageAndUpdateCache(mxfProgram.Extras["artwork"], ImageType.Movie, mxfProgram.Extras["md5"]);
                imageDownloadQueue.EnqueueProgramArtworkCollection(mxfProgram.Extras["artwork"]);
            }
            else
            {

                if (response.Data.Count > 0)
                {
                    logger.LogError("No 2x3 artwork found for {ProgramId}", response.ProgramId);
                }
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
}