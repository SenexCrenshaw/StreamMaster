using Microsoft.Extensions.Logging;
using StreamMaster.SchedulesDirectAPI.Domain.Enums;
using System.Collections.Concurrent;
using System.Text.Json;

namespace StreamMaster.SchedulesDirectAPI;
public partial class SchedulesDirect
{
    private  readonly List<MxfProgram> sportEvents = new List<MxfProgram>();

    private  bool GetAllSportsImages()
    {
        var settings = memoryCache.GetSetting();
        // reset counters
        imageQueue = new List<string>();
        imageResponses = new ConcurrentBag<ProgramMetadata>();
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
                imageQueue.Add(sportEvent.ProgramId);
            }
        }
        logger.LogDebug($"Found {processedObjects} cached/unavailable sport event image links.");

        // maximum 500 queries at a time
        if (imageQueue.Count > 0)
        {
            Parallel.For(0, (imageQueue.Count / MaxImgQueries + 1), new ParallelOptions { MaxDegreeOfParallelism = MaxParallelDownloads }, i =>
            {
                DownloadImageResponses(i * MaxImgQueries);
            });

            ProcessSportsImageResponses();
            if (processedObjects != totalObjects)
            {
                logger.LogWarning($"Failed to download and process {sportEvents.Count - processedObjects} sport event image links.");
            }
        }
        logger.LogInformation("Exiting GetAllSportsImages(). SUCCESS.");
        imageQueue = null; imageResponses = null; sportEvents.Clear();
        return true;
    }

    private  void ProcessSportsImageResponses()
    {
        // process request response
        if (imageResponses == null) return;
        foreach (var response in imageResponses)
        {
            //IncrementProgress();
            if (response.Data == null) continue;

            var mxfProgram = sportEvents.SingleOrDefault(arg => arg.ProgramId == response.ProgramId);
            if (mxfProgram == null) continue;

            // get sports event images
            List<ProgramArtwork> artwork;
            mxfProgram.extras.Add("artwork", artwork = GetTieredImages(response.Data, new List<string> { "team event", "sport event" }));
            mxfProgram.mxfGuideImage = GetGuideImageAndUpdateCache(artwork, ImageType.Program, mxfProgram.extras["md5"]);
        }
    }
}

