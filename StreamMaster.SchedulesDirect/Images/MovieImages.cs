using Microsoft.Extensions.Caching.Memory;

using StreamMaster.Domain.Common;
using StreamMaster.SchedulesDirect.Domain.Enums;
using StreamMaster.SchedulesDirect.Helpers;

using System.Collections.Concurrent;
using System.Text.Json;

namespace StreamMaster.SchedulesDirect.Images;
public class MovieImages(ILogger<MovieImages> logger, IEPGCache<MovieImages> epgCache, IImageDownloadQueue imageDownloadQueue, IMemoryCache memoryCache, ISchedulesDirectAPIService schedulesDirectAPI, ISchedulesDirectDataService schedulesDirectDataService) : IMovieImages
{
    private List<string> movieImageQueue = [];
    private ConcurrentBag<ProgramMetadata> movieImageResponses = [];
    private int processedObjects;
    private int totalObjects;
    public async Task<bool> GetAllMoviePosters()
    {
        ISchedulesDirectData schedulesDirectData = schedulesDirectDataService.SchedulesDirectData();

        List<MxfProgram> moviePrograms = schedulesDirectData.ProgramsToProcess.Where(arg => arg.IsMovie).Where(arg => !arg.IsAdultOnly).ToList();

        // reset counters
        movieImageQueue = [];
        movieImageResponses = [];
        //IncrementNextStage(moviePrograms.Count);

        logger.LogInformation("Entering GetAllMoviePosters() for {totalObjects} movies.", moviePrograms.Count);

        // query all movies from Schedules Direct
        foreach (MxfProgram? mxfProgram in moviePrograms)
        {
            if (mxfProgram.extras.ContainsKey("md5") && epgCache.JsonFiles.ContainsKey(mxfProgram.extras["md5"]) && epgCache.JsonFiles[mxfProgram.extras["md5"]].Images != null)
            {
                //IncrementProgress();
                if (string.IsNullOrEmpty(epgCache.JsonFiles[mxfProgram.extras["md5"]].Images))
                {
                    continue;
                }

                List<ProgramArtwork>? artwork;
                using StringReader reader = new(epgCache.JsonFiles[mxfProgram.extras["md5"]].Images);
                artwork = JsonSerializer.Deserialize<List<ProgramArtwork>>(reader.ReadToEnd());

                if (artwork != null)
                {
                    mxfProgram.extras.AddOrUpdate("artwork", artwork);
                    mxfProgram.mxfGuideImage = epgCache.GetGuideImageAndUpdateCache(artwork, ImageType.Movie);
                }

            }
            else
            {
                movieImageQueue.Add(mxfProgram.ProgramId);
            }
        }
        logger.LogDebug($"Found {processedObjects} cached/unavailable movie poster links.");

        // maximum 500 queries at a time
        if (movieImageQueue.Count > 0)
        {
            SemaphoreSlim semaphore = new(SchedulesDirect.MaxParallelDownloads, SchedulesDirect.MaxParallelDownloads);
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
                        semaphore.Release();
                    }
                }));
            }

            await Task.WhenAll(tasks).ConfigureAwait(false);

            // Continue with the rest of your processing
            ProcessMovieImageResponses();
            imageDownloadQueue.EnqueueProgramMetadataCollection(movieImageResponses);

            if (processedObjects != totalObjects)
            {
                logger.LogWarning("Failed to download and process {FailedCount} movie poster links.", moviePrograms.Count - processedObjects);
            }
        }


        logger.LogInformation("Exiting GetAllMoviePosters(). SUCCESS.");
        movieImageQueue = []; movieImageResponses = [];
        epgCache.SaveCache();
        return true;
    }

    private void ProcessMovieImageResponses()
    {
        Setting setting = memoryCache.GetSetting();
        string artworkSize = string.IsNullOrEmpty(setting.SDSettings.ArtworkSize) ? "Md" : setting.SDSettings.ArtworkSize;
        // process request response
        foreach (ProgramMetadata response in movieImageResponses)
        {
            //IncrementProgress();
            if (response.Data == null)
            {
                continue;
            }
            ISchedulesDirectData schedulesDirectData = schedulesDirectDataService.SchedulesDirectData();

            // determine which program this belongs to
            MxfProgram mxfProgram = schedulesDirectData.FindOrCreateProgram(response.ProgramId);

            // first choice is return from Schedules Direct
            List<ProgramArtwork> artwork;
            artwork = SDHelpers.GetTieredImages(response.Data, ["episode"], artworkSize).Where(arg => arg.Aspect.Equals("2x3")).ToList();
            if (artwork.Any())
            {
                mxfProgram.extras.AddOrUpdate("artwork", artwork);

                mxfProgram.mxfGuideImage = epgCache.GetGuideImageAndUpdateCache(artwork, ImageType.Movie, mxfProgram.extras["md5"]);
            }
            //// second choice is from TMDb if allowed and available
            //if (artwork.Count == 0 || artwork[0].Category.Equals("Staple"))
            //{
            //    var tmdb = GetTmdbMoviePoster(mxfProgram.Title, mxfProgram.Year, mxfProgram.Language);
            //    if (tmdb.Count > 0) artwork = tmdb;
            //}

            // regardless if image is found or not, store the final result in xml file
            // this avoids hitting the tmdb server every update for every movie missing cover art
            //mxfProgram.extras.Add("artwork", artwork);
            //mxfProgram.mxfGuideImage = epgCache.GetGuideImageAndUpdateCache(artwork, ImageType.Movie, mxfProgram.extras["md5"]);
        }
    }



    //private  List<ProgramArtwork> GetTmdbMoviePoster(string title, int year, string language)
    //{
    //    var poster = tmdbApi.FindPosterArtwork(title, year, language);
    //    if (poster == null) return new List<ProgramArtwork>();
    //    return new List<ProgramArtwork>
    //        {
    //            new ProgramArtwork
    //            {
    //                Aspect = "2x3",
    //                Category = "Box Art",
    //                Height = (int)(tmdbApi.PosterWidth * 1.5),
    //                Size = config.ArtworkSize,
    //                Uri = poster,
    //                Width = tmdbApi.PosterWidth
    //            }
    //        };
    //}

    public void ResetCache()
    {

        movieImageQueue.Clear();
        movieImageResponses.Clear();

        processedObjects = 0;
        totalObjects = 0;
    }

    public void ClearCache()
    {
        epgCache.ResetCache();
    }

}

