using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMaster.SchedulesDirectAPI.Domain.Enums;
using StreamMaster.SchedulesDirectAPI.Domain.Models;
using StreamMaster.SchedulesDirectAPI.Helpers;

using StreamMasterDomain.Dto;
using StreamMasterDomain.Enums;
using StreamMasterDomain.Extensions;

using System.Collections.Concurrent;
using System.Text.Json;

namespace StreamMaster.SchedulesDirectAPI;
public partial class SchedulesDirect
{
    private  readonly List<MxfProgram> sportEvents = [];
    private List<string> sportsImageQueue = [];
    private ConcurrentBag<ProgramMetadata> sportsImageResponses = [];
    private async Task <bool> GetAllSportsImages(CancellationToken cancellationToken)
    {
        var settings = memoryCache.GetSetting();
        // reset counters
        sportsImageQueue = [];
        sportsImageResponses = [];
        //IncrementNextStage(sportEvents.Count);
        if (!settings.SDSettings.SeasonEventImages) return true;

        // scan through each series in the mxf
        logger.LogInformation($"Entering GetAllSportsImages() for {totalObjects} sports events.");
        foreach (var sportEvent in sportEvents)
        {
            string md5 = sportEvent.extras["md5"];
            if (epgCache.JsonFiles.ContainsKey(md5) && !string.IsNullOrEmpty(epgCache.JsonFiles[md5].Images))
            {
                //IncrementProgress();
                List<ProgramArtwork> artwork;
                using (var reader = new StringReader(epgCache.JsonFiles[md5].Images))
                {
                    sportEvent.extras.Add("artwork", artwork = JsonSerializer.Deserialize<List<ProgramArtwork>>(reader.ReadToEnd()));
                }

                sportEvent.mxfGuideImage = GetGuideImageAndUpdateCache(artwork, ImageType.Program);
            }
            else
            {
                sportsImageQueue.Add(sportEvent.ProgramId);
            }
        }
        logger.LogDebug($"Found {processedObjects} cached/unavailable sport event image links.");

        // maximum 500 queries at a time
        if (sportsImageQueue.Count > 0)
        {
            Parallel.For(0, (sportsImageQueue.Count / MaxImgQueries + 1), new ParallelOptions { MaxDegreeOfParallelism = MaxParallelDownloads }, i =>
            {               
                DownloadImageResponses(sportsImageQueue, sportsImageResponses,i * MaxImgQueries);
            });

            ProcessSportsImageResponses();
             imageDownloadService.EnqueueProgramMetadataCollection(sportsImageResponses);
            //await DownloadImages(sportsImageResponses, cancellationToken);
            if (processedObjects != totalObjects)
            {
                logger.LogWarning($"Failed to download and process {sportEvents.Count - processedObjects} sport event image links.");
            }
        }

        //UpdateIcons(sportEvents);

        logger.LogInformation("Exiting GetAllSportsImages(). SUCCESS.");  
        
        sportsImageQueue = []; sportsImageResponses = []; sportEvents.Clear();

        return true;
    }

    

    private  void ProcessSportsImageResponses()
    {
        // process request response
        if (sportsImageResponses == null) return;
        foreach (var response in sportsImageResponses)
        {
            //IncrementProgress();
            if (response.Data == null) continue;

            var mxfProgram = sportEvents.SingleOrDefault(arg => arg.ProgramId == response.ProgramId);
            if (mxfProgram == null) continue;

            // get sports event images
            List<ProgramArtwork> artwork;
            mxfProgram.extras.Add("artwork", artwork = SDHelpers.GetTieredImages(response.Data, new List<string> { "team event", "sport event" }));
            mxfProgram.mxfGuideImage = GetGuideImageAndUpdateCache(artwork, ImageType.Program, mxfProgram.extras["md5"]);
        }
    }
}

