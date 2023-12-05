using Microsoft.Extensions.Logging;

using StreamMaster.SchedulesDirectAPI.Domain.Enums;
using StreamMaster.SchedulesDirectAPI.Helpers;

using StreamMasterDomain.Services;

using System.Collections.Concurrent;
using System.Text.Json;

namespace StreamMaster.SchedulesDirectAPI;
public partial class SchedulesDirect
{
    private List<string> movieImageQueue = [];
    private ConcurrentBag<ProgramMetadata> movieImageResponses = [];
    private async Task<bool> GetAllMoviePosters(CancellationToken cancellationToken)
    {

        var moviePrograms = schedulesDirectData.ProgramsToProcess.Where(arg => arg.IsMovie).Where(arg => !arg.IsAdultOnly).ToList();

        // reset counters
        movieImageQueue = [];
        movieImageResponses =[];
        //IncrementNextStage(moviePrograms.Count);
        
        logger.LogInformation($"Entering GetAllMoviePosters() for {totalObjects} movies.");

        // query all movies from Schedules Direct
        foreach (var mxfProgram in moviePrograms)
        {
            if (epgCache.JsonFiles.ContainsKey(mxfProgram.extras["md5"]) && epgCache.JsonFiles[mxfProgram.extras["md5"]].Images != null)
            {
                //IncrementProgress();
                if (string.IsNullOrEmpty(epgCache.JsonFiles[mxfProgram.extras["md5"]].Images)) continue;

                List<ProgramArtwork>? artwork;
                using var reader = new StringReader(epgCache.JsonFiles[mxfProgram.extras["md5"]].Images);
                artwork = JsonSerializer.Deserialize<List<ProgramArtwork>>(reader.ReadToEnd());

                if (artwork != null)
                {
                    mxfProgram.extras["artwork"] = artwork;
                    mxfProgram.mxfGuideImage = GetGuideImageAndUpdateCache(artwork, ImageType.Movie);
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
            Parallel.For(0, (movieImageQueue.Count / MaxImgQueries + 1), new ParallelOptions { MaxDegreeOfParallelism = MaxParallelDownloads }, i =>
            {
                DownloadImageResponses(movieImageQueue,movieImageResponses, i * MaxImgQueries);
            });

            ProcessMovieImageResponses();
            imageDownloadService.EnqueueProgramMetadataCollection(movieImageResponses);
            //await DownloadImages(movieImageResponses, cancellationToken);
            if (processedObjects != totalObjects)
            {
                logger.LogWarning($"Failed to download and process {moviePrograms.Count - processedObjects} movie poster links.");
            }
        }
        logger.LogInformation("Exiting GetAllMoviePosters(). SUCCESS.");
        movieImageQueue = []; movieImageResponses = [];
        return true;
    }

    private  void ProcessMovieImageResponses()
    {
        // process request response
        foreach (var response in movieImageResponses)
        {
            //IncrementProgress();
            if (response.Data == null) continue;

            // determine which program this belongs to
            var mxfProgram = schedulesDirectData.FindOrCreateProgram(response.ProgramId);

            // first choice is return from Schedules Direct
            List<ProgramArtwork> artwork;
            artwork = SDHelpers.GetTieredImages(response.Data, ["episode"]).Where(arg => arg.Aspect.Equals("2x3")).ToList();

            //// second choice is from TMDb if allowed and available
            //if (artwork.Count == 0 || artwork[0].Category.Equals("Staple"))
            //{
            //    var tmdb = GetTmdbMoviePoster(mxfProgram.Title, mxfProgram.Year, mxfProgram.Language);
            //    if (tmdb.Count > 0) artwork = tmdb;
            //}

            // regardless if image is found or not, store the final result in xml file
            // this avoids hitting the tmdb server every update for every movie missing cover art
            mxfProgram.extras.Add("artwork", artwork);
            mxfProgram.mxfGuideImage = GetGuideImageAndUpdateCache(artwork, ImageType.Movie, mxfProgram.extras["md5"]);
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

}

