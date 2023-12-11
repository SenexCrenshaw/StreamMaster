using Microsoft.Extensions.Logging;

using StreamMaster.SchedulesDirectAPI.Domain.Enums;
using StreamMaster.SchedulesDirectAPI.Helpers;

using System.Collections.Concurrent;
using System.Text.Json;

namespace StreamMaster.SchedulesDirectAPI;
public partial class SchedulesDirect
{
    private List<MxfSeason> seasons = [];
    private List<string> seasonImageQueue = [];
    private ConcurrentBag<ProgramMetadata> seasonImageResponses = [];
    private async Task<bool> GetAllSeasonImages(CancellationToken cancellationToken)
    {
        StreamMasterDomain.Common.Setting settings = memoryCache.GetSetting();

        // reset counters
        seasonImageQueue = [];
        seasonImageResponses = [];
        //IncrementNextStage(mxf.SeasonsToProcess.Count);
        if (!settings.SDSettings.SeasonEventImages)
        {
            return true;
        }


        // scan through each series in the mxf
        logger.LogInformation("Entering GetAllSeasonImages() for {totalObjects} seasons.", schedulesDirectData.SeasonsToProcess.Count);
        foreach (MxfSeason season in schedulesDirectData.SeasonsToProcess)
        {
            string uid = $"{season.SeriesId}_{season.SeasonNumber}";
            if (epgCache.JsonFiles.ContainsKey(uid) && !string.IsNullOrEmpty(epgCache.JsonFiles[uid].Images))
            {
                epgCache.JsonFiles[uid].Current = true;
                //IncrementProgress();
                if (string.IsNullOrEmpty(epgCache.JsonFiles[uid].Images))
                {
                    continue;
                }

                List<ProgramArtwork> artwork;
                using (StringReader reader = new(epgCache.JsonFiles[uid].Images))
                {
                    season.extras.Add("artwork", artwork = JsonSerializer.Deserialize<List<ProgramArtwork>>(reader.ReadToEnd()));
                }

                season.mxfGuideImage = GetGuideImageAndUpdateCache(artwork, ImageType.Season);
            }
            else if (!string.IsNullOrEmpty(season.ProtoTypicalProgram))
            {
                seasons.Add(season);
                seasonImageQueue.Add(season.ProtoTypicalProgram);
            }
            else
            {
                //IncrementProgress();
            }
        }
        logger.LogDebug($"Found {processedObjects} cached/unavailable season image links.");

        // maximum 500 queries at a time
        if (seasonImageQueue.Count > 0)
        {
            _ = Parallel.For(0, (seasonImageQueue.Count / MaxImgQueries) + 1, new ParallelOptions { MaxDegreeOfParallelism = MaxParallelDownloads }, i =>
            {
                DownloadImageResponses(seasonImageQueue, seriesImageResponses, i * MaxImgQueries);
            });

            ProcessSeasonImageResponses();
            imageDownloadQueue.EnqueueProgramMetadataCollection(seasonImageResponses);

            //await DownloadImages(seasonImageResponses, cancellationToken);
            if (processedObjects != totalObjects)
            {
                logger.LogWarning($"Failed to download and process {seasons.Count - processedObjects} season image links.");
            }
        }
        //UpdateSeasonIcons(schedulesDirectData.SeasonsToProcess);
        logger.LogInformation("Exiting GetAllSeasonImages(). SUCCESS.");
        seasonImageQueue = []; seasonImageResponses = []; seasons.Clear();
        return true;
    }

    private void ProcessSeasonImageResponses()
    {
        // process request response
        foreach (ProgramMetadata response in seasonImageResponses)
        {
            //IncrementProgress();
            if (response.Data == null)
            {
                continue;
            }

            MxfSeason? season = seasons.SingleOrDefault(arg => arg.ProtoTypicalProgram == response.ProgramId);
            if (season == null)
            {
                continue;
            }

            // get season images
            List<ProgramArtwork> artwork;
            season.extras.Add("artwork", artwork = SDHelpers.GetTieredImages(response.Data, ["season"]));

            // create a season entry in cache
            string uid = $"{season.SeriesId}_{season.SeasonNumber}";
            if (!epgCache.JsonFiles.ContainsKey(uid))
            {
                epgCache.AddAsset(uid, null);
            }

            season.mxfGuideImage = GetGuideImageAndUpdateCache(artwork, ImageType.Season, uid);
        }
    }
}
