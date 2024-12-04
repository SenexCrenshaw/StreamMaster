using System.Collections.Concurrent;
using System.Text.Json;

using StreamMaster.Domain.Cache;
using StreamMaster.SchedulesDirect.Domain;
using StreamMaster.SchedulesDirect.Services;

namespace StreamMaster.SchedulesDirect.Images
{
    public class MovieImages(
        ILogger<MovieImages> logger,
        SMCacheManager<MovieImages> hybridCache,
        IImageDownloadQueue imageDownloadQueue,
        IOptionsMonitor<SDSettings> sdSettings,
        ISchedulesDirectAPIService schedulesDirectAPI,
        IProgramRepository programRepository
    ) : IMovieImages, IDisposable
    {
        private static readonly SemaphoreSlim classSemaphore = new(1, 1);
        private readonly SemaphoreSlim semaphore = new(SDAPIConfig.MaxParallelDownloads, SDAPIConfig.MaxParallelDownloads);
        private bool disposedValue;
        private int processedObjects = 0;

        public async Task<bool> ProcessArtAsync(CancellationToken cancellationToken)
        {
            if (!sdSettings.CurrentValue.MovieImages)
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
                List<string> movieImageQueue = [];
                ConcurrentBag<ProgramMetadata> movieImageResponses = [];

                int totalObjects = 0;


                List<MxfProgram> moviePrograms = programRepository.Programs.Values.Where(p => p.IsMovie && !p.IsAdultOnly).ToList();
                MxfProgram? a = moviePrograms.FirstOrDefault(a => a.ProgramId.Contains("EP0457"));
                MxfProgram? a2 = moviePrograms.FirstOrDefault(a => a.ProgramId.Contains("1843"));
                if (a is not null)
                {
                    int aaa = 1;
                }
                if (a2 is not null)
                {
                    int aaa = 1;
                }
                totalObjects = moviePrograms.Count;

                logger.LogInformation("Processing {TotalObjects} movie posters.", totalObjects);

                foreach (MxfProgram program in moviePrograms)
                {
                    if (program.ProgramId == "MV02175103")
                    {
                        int aaa = 1;
                    }
                    cancellationToken.ThrowIfCancellationRequested();
                    List<ProgramArtwork>? artWorks = await hybridCache.GetAsync<List<ProgramArtwork>>(program.ProgramId);
                    if (artWorks is not null)
                    {
                        //program.AddArtworks(artWorks);
                        programRepository.SetProgramLogos(program, artWorks);
                        imageDownloadQueue.EnqueueProgramArtworkCollection(artWorks);
                    }
                    else if (!string.IsNullOrEmpty(program.ProgramId))
                    {
                        movieImageQueue.Add(program.ProgramId);
                    }
                }

                logger.LogDebug("Queued {MovieImageQueueCount} movie images for download.", movieImageQueue.Count);

                if (movieImageQueue.Count > 0)
                {
                    await DownloadAndProcessMovieImagesAsync(movieImageQueue, movieImageResponses, cancellationToken).ConfigureAwait(false);
                }

                await ProcessMovieImageResponsesAsync(movieImageResponses, cancellationToken);

                logger.LogInformation("Successfully processed {ProcessedObjects} of {TotalObjects} movies.", processedObjects, totalObjects);

                //await hybridCache.ClearAsync();
                return true;
            }
            finally
            {
                classSemaphore.Release();
            }
        }

        private async Task DownloadAndProcessMovieImagesAsync(List<string> movieImageQueue, ConcurrentBag<ProgramMetadata> movieImageResponses, CancellationToken cancellationToken)
        {
            List<Task> tasks = [];
            int processedCount = 0;

            for (int i = 0; i <= movieImageQueue.Count / SDAPIConfig.MaxImgQueries; i++)
            {
                int startIndex = i * SDAPIConfig.MaxImgQueries;
                tasks.Add(Task.Run(async () =>
                {
                    await semaphore.WaitAsync();
                    try
                    {
                        int itemCount = Math.Min(movieImageQueue.Count - startIndex, SDAPIConfig.MaxImgQueries);
                        await schedulesDirectAPI.DownloadImageResponsesAsync(
                            movieImageQueue,
                            movieImageResponses,
                            startIndex, cancellationToken: cancellationToken
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
                }, cancellationToken));
            }

            await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        private async Task ProcessMovieImageResponsesAsync(ConcurrentBag<ProgramMetadata> movieImageResponses, CancellationToken cancellationToken)
        {
            string artworkSize = string.IsNullOrEmpty(sdSettings.CurrentValue.ArtworkSize)
                ? BuildInfo.DefaultSDImageSize
                : sdSettings.CurrentValue.ArtworkSize;

            foreach (ProgramMetadata response in movieImageResponses)
            {
                cancellationToken.ThrowIfCancellationRequested();
                ++processedObjects;

                if (response.Data == null || response.Data.Count == 0 || response.Data[0].Code != 0)
                {
                    logger.LogWarning("No Movie Image artwork found for {ProgramId}", response.ProgramId);
                    continue;
                }

                MxfProgram? mxfProgram = programRepository.FindProgram(response.ProgramId);

                if (mxfProgram == null)
                {
                    logger.LogWarning("Program {ProgramId} not found in the data store.", response.ProgramId);
                    continue;
                }

                List<ProgramArtwork> artworks = SDHelpers.GetTieredImages(
                    response.Data,
                    artworkSize,
                    ["episode", "series"],
                    sdSettings.CurrentValue.MoviePosterAspect
                );

                programRepository.SetProgramLogos(mxfProgram,artworks);
                //mxfProgram.AddArtworks(artworks);

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
