using Microsoft.Extensions.Logging;

using StreamMaster.SchedulesDirectAPI.Domain.Enums;
using StreamMaster.SchedulesDirectAPI.Helpers;

using System.Collections.Concurrent;
using System.Text.Json;

namespace StreamMaster.SchedulesDirectAPI;
public partial class SchedulesDirect
{
    private List<MxfProgram> sportEvents = [];
    private List<string> sportsImageQueue = [];
    private ConcurrentBag<ProgramMetadata> sportsImageResponses = [];
    private bool GetAllSportsImages()
    {
        StreamMasterDomain.Common.Setting settings = memoryCache.GetSetting();
        // reset counters
        sportsImageQueue = [];
        sportsImageResponses = [];
        //IncrementNextStage(sportEvents.Count);
        if (!settings.SDSettings.SeasonEventImages)
        {
            return true;
        }

        // scan through each series in the mxf
        logger.LogInformation("Entering GetAllSportsImages() for {totalObjects} sports events.", sportEvents.Count);
        foreach (MxfProgram sportEvent in sportEvents)
        {
            string md5 = sportEvent.extras["md5"];
            if (epgCache.JsonFiles.ContainsKey(md5) && !string.IsNullOrEmpty(epgCache.JsonFiles[md5].Images))
            {
                //IncrementProgress();              
                using StringReader reader = new(epgCache.JsonFiles[md5].Images);
                List<ProgramArtwork>? artwork = JsonSerializer.Deserialize<List<ProgramArtwork>>(reader.ReadToEnd());
                if (sportEvent.extras.ContainsKey("artwork"))
                {
                    sportEvent.extras["artwork"] = artwork;
                }
                else
                {
                    sportEvent.extras.Add("artwork", artwork);
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
            _ = Parallel.For(0, (sportsImageQueue.Count / MaxImgQueries) + 1, new ParallelOptions { MaxDegreeOfParallelism = MaxParallelDownloads }, i =>
            {
                DownloadImageResponses(sportsImageQueue, sportsImageResponses, i * MaxImgQueries);
            });

            ProcessSportsImageResponses();
            imageDownloadQueue.EnqueueProgramMetadataCollection(sportsImageResponses);
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



    private void ProcessSportsImageResponses()
    {
        // process request response
        if (sportsImageResponses == null)
        {
            return;
        }

        StreamMasterDomain.Common.Setting setting = memoryCache.GetSetting();
        string artworkSize = string.IsNullOrEmpty(setting.SDSettings.ArtworkSize) ? "Md" : setting.SDSettings.ArtworkSize;

        foreach (ProgramMetadata response in sportsImageResponses)
        {
            //IncrementProgress();
            if (response.Data == null)
            {
                continue;
            }

            MxfProgram? mxfProgram = sportEvents.SingleOrDefault(arg => arg.ProgramId == response.ProgramId);
            if (mxfProgram == null)
            {
                continue;
            }

            // get sports event images      
            List<ProgramArtwork> artwork = SDHelpers.GetTieredImages(response.Data, ["team event", "sport event"], artworkSize);
            if (mxfProgram.extras.ContainsKey("artwork"))
            {
                mxfProgram.extras["artwork"] = artwork;
            }
            else
            {
                mxfProgram.extras.Add("artwork", artwork);
            }

            mxfProgram.mxfGuideImage = GetGuideImageAndUpdateCache(artwork, ImageType.Program, mxfProgram.extras["md5"]);
        }
    }
}

