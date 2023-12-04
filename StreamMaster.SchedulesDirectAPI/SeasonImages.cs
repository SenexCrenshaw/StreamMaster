using Microsoft.Extensions.Logging;
using StreamMaster.SchedulesDirectAPI.Domain.Enums;
using System.Collections.Concurrent;
using System.Text.Json;

namespace StreamMaster.SchedulesDirectAPI;
public partial class SchedulesDirect
{
    private  readonly List<MxfSeason> seasons = [];

    private  bool GetAllSeasonImages()
    {
        var settings = memoryCache.GetSetting();

        // reset counters
        imageQueue = new List<string>();
        imageResponses = new ConcurrentBag<ProgramMetadata>();
        //IncrementNextStage(mxf.SeasonsToProcess.Count);
        if (!settings.SDSettings.SeasonEventImages) return true;


        // scan through each series in the mxf
        logger.LogInformation($"Entering GetAllSeasonImages() for {totalObjects} seasons.");
        foreach (var season in schedulesDirectData.SeasonsToProcess)
        {
            var uid = $"{season.SeriesId}_{season.SeasonNumber}";
            if (epgCache.JsonFiles.ContainsKey(uid) && !string.IsNullOrEmpty(epgCache.JsonFiles[uid].Images))
            {
                epgCache.JsonFiles[uid].Current = true;
                //IncrementProgress();
                if (string.IsNullOrEmpty(epgCache.JsonFiles[uid].Images)) continue;

                List<ProgramArtwork> artwork;
                using (var reader = new StringReader(epgCache.JsonFiles[uid].Images))
                {
                    season.extras.Add("artwork", artwork = JsonSerializer.Deserialize<List<ProgramArtwork>>(reader.ReadToEnd()));
                }
            
                season.mxfGuideImage = GetGuideImageAndUpdateCache(artwork, ImageType.Season);
            }
            else if (!string.IsNullOrEmpty(season.ProtoTypicalProgram))
            {
                seasons.Add(season);
                imageQueue.Add(season.ProtoTypicalProgram);
            }
            else
            {
                //IncrementProgress();
            }
        }
        logger.LogDebug($"Found {processedObjects} cached/unavailable season image links.");

        // maximum 500 queries at a time
        if (imageQueue.Count > 0)
        {
            Parallel.For(0, (imageQueue.Count / MaxImgQueries + 1), new ParallelOptions { MaxDegreeOfParallelism = MaxParallelDownloads }, i =>
            {
                DownloadImageResponses(i * MaxImgQueries);
            });

            ProcessSeasonImageResponses();
            if (processedObjects != totalObjects)
            {
                logger.LogWarning($"Failed to download and process {seasons.Count - processedObjects} season image links.");
            }
        }
        logger.LogInformation("Exiting GetAllSeasonImages(). SUCCESS.");
        imageQueue = null; imageResponses = null; seasons.Clear();
        return true;
    }

    private  void ProcessSeasonImageResponses()
    {
        // process request response
        foreach (var response in imageResponses)
        {
            //IncrementProgress();
            if (response.Data == null) continue;

            var season = seasons.SingleOrDefault(arg => arg.ProtoTypicalProgram == response.ProgramId);
            if (season == null) continue;

            // get season images
            List<ProgramArtwork> artwork;
            season.extras.Add("artwork", artwork = GetTieredImages(response.Data, new List<string> { "season" }));

            // create a season entry in cache
            var uid = $"{season.SeriesId}_{season.SeasonNumber}";
            if (!epgCache.JsonFiles.ContainsKey(uid))
            {
                epgCache.AddAsset(uid, null);
            }

            season.mxfGuideImage = GetGuideImageAndUpdateCache(artwork, ImageType.Season, uid);
        }
    }
}
