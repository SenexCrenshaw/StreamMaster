using System.Collections.Concurrent;
using System.Text.Json;

using StreamMaster.Domain.Cache;

namespace StreamMaster.SchedulesDirect.Images
{
    public class MovieImages(
        ILogger<MovieImages> logger,
        IHybridCache<MovieImages> hybridCache,
        IImageDownloadQueue imageDownloadQueue,
        IOptionsMonitor<SDSettings> sdSettings,
        ISchedulesDirectAPIService schedulesDirectAPI,
        ISchedulesDirectDataService schedulesDirectDataService
    ) : IMovieImages, IDisposable
    {
        private static readonly SemaphoreSlim classSemaphore = new(1, 1);
        private readonly SemaphoreSlim semaphore = new(SchedulesDirect.MaxParallelDownloads, SchedulesDirect.MaxParallelDownloads);

        private bool disposedValue;
        private int processedObjects = 0;

        public async Task<bool> ProcessArtAsync()
        {
            await classSemaphore.WaitAsync();
            try
            {
                List<string> movieImageQueue = [];
                ConcurrentBag<ProgramMetadata> movieImageResponses = [];

                int totalObjects = 0;

                ISchedulesDirectData schedulesDirectData = schedulesDirectDataService.SchedulesDirectData();
                List<MxfProgram> moviePrograms = schedulesDirectData.ProgramsToProcess.Values
                    .Where(p => p.IsMovie && !p.IsAdultOnly)
                    .ToList();

                totalObjects = moviePrograms.Count;

                logger.LogInformation("Processing {TotalObjects} movie posters.", totalObjects);

                foreach (MxfProgram program in moviePrograms)
                {
                    string? cachedJson = await hybridCache.GetAsync(program.ProgramId);
                    if (!string.IsNullOrEmpty(cachedJson))
                    {
                        List<ProgramArtwork> artworks = JsonSerializer.Deserialize<List<ProgramArtwork>>(cachedJson) ?? [];
                        program.ArtWorks = artworks;
                        imageDownloadQueue.EnqueueProgramArtworkCollection(artworks);
                    }
                    else if (!string.IsNullOrEmpty(program.ProgramId))
                    {
                        movieImageQueue.Add(program.ProgramId);
                    }
                }

                logger.LogDebug("Queued {MovieImageQueueCount} movie images for download.", movieImageQueue.Count);

                if (movieImageQueue.Count > 0)
                {
                    await DownloadAndProcessMovieImagesAsync(movieImageQueue, movieImageResponses).ConfigureAwait(false);
                }

                await ProcessMovieImageResponsesAsync(movieImageResponses);

                logger.LogInformation("Successfully processed {ProcessedObjects} of {TotalObjects} movies.", processedObjects, totalObjects);

                //await hybridCache.ClearAsync();
                return true;
            }
            finally
            {
                classSemaphore.Release();
            }
        }

        private async Task DownloadAndProcessMovieImagesAsync(
            List<string> movieImageQueue,
            ConcurrentBag<ProgramMetadata> movieImageResponses)
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
                        await schedulesDirectAPI.DownloadImageResponsesAsync(
                            movieImageQueue,
                            movieImageResponses,
                            startIndex
                        ).ConfigureAwait(false);
                        Interlocked.Add(ref processedCount, itemCount);
                        logger.LogInformation("Downloaded movie image information {ProcessedCount} of {TotalCount}", processedCount, movieImageQueue.Count);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Error downloading movie images at start index {StartIndex}", startIndex);
                    }
                    finally
                    {
                        _ = semaphore.Release();
                    }
                }));
            }

            await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        private async Task ProcessMovieImageResponsesAsync(ConcurrentBag<ProgramMetadata> movieImageResponses)
        {
            string artworkSize = string.IsNullOrEmpty(sdSettings.CurrentValue.ArtworkSize)
                ? BuildInfo.DefaultSDImageSize
                : sdSettings.CurrentValue.ArtworkSize;

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

                List<ProgramArtwork> artworks = SDHelpers.GetTieredImages(
                    response.Data,
                    artworkSize,
                    ["episode", "series"],
                    sdSettings.CurrentValue.MoviePosterAspect
                );

                mxfProgram.AddArtwork(artworks);

                string artworkJson = JsonSerializer.Serialize(artworks);
                await hybridCache.SetAsync(response.ProgramId, artworkJson);

                if (artworks.Count > 0)
                {
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
            if (!disposedValue)
            {
                if (disposing)
                {
                    semaphore.Dispose();
                }
                disposedValue = true;
            }
        }

        public List<string> GetExpiredKeys()
        {
            return [];// epgCache.GetExpiredKeys();
        }

        public void RemovedExpiredKeys(List<string>? keysToDelete = null)
        {
            //epgCache.RemovedExpiredKeys(keysToDelete);
        }
    }
}
