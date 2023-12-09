using Microsoft.Extensions.Logging;

using StreamMaster.SchedulesDirectAPI.Domain.Enums;
using StreamMaster.SchedulesDirectAPI.Helpers;

using System.Collections.Concurrent;
using System.Collections.Specialized;
using System.Text.Json;

namespace StreamMaster.SchedulesDirectAPI;
public partial class SchedulesDirect
{

    private NameValueCollection sportsSeries = [];

    private List<string> seriesImageQueue = [];
    private ConcurrentBag<ProgramMetadata> seriesImageResponses = [];

    private async Task<bool> GetAllSeriesImages(CancellationToken cancellationToken)
    {
        // reset counters
        seriesImageQueue = [];
        seriesImageResponses = [];
        //IncrementNextStage(schedulesDirectData.SeriesInfosToProcess.Count);

        logger.LogInformation($"Entering GetAllSeriesImages() for {totalObjects} series.");
        var refreshing = 0;

        var setting = memoryCache.GetSetting();

        // scan through each series in the mxf
        foreach (var series in schedulesDirectData.SeriesInfosToProcess)
        {
            string seriesId;

            // if image for series already exists in archive file, use it
            // cycle images for a refresh based on day of month and seriesid
            var refresh = false;
            if (int.TryParse(series.SeriesId, out var digits))
            {
                refresh = (digits * setting.SDSettings.SDStationIds.Count % DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)) + 1 == DateTime.Now.Day;
                seriesId = $"SH{series.SeriesId}0000";
            }
            else
            {
                seriesId = series.SeriesId;
            }

            if (!refresh && epgCache.JsonFiles.ContainsKey(seriesId) && !string.IsNullOrEmpty(epgCache.JsonFiles[seriesId].Images))
            {
                //IncrementProgress();
                if (epgCache.JsonFiles[seriesId].Images == string.Empty)
                {
                    continue;
                }

                List<ProgramArtwork> artwork;
                using (var reader = new StringReader(epgCache.JsonFiles[seriesId].Images))
                {
                    artwork = JsonSerializer.Deserialize<List<ProgramArtwork>>(reader.ReadToEnd());
                }

                // Add artwork to series.extras
                if (artwork != null)
                {
                    series.extras.Add("artwork", artwork);
                }

                series.mxfGuideImage = GetGuideImageAndUpdateCache(artwork, ImageType.Series);
            }
            else if (int.TryParse(series.SeriesId, out var dummy))
            {
                // only increment the refresh count if something exists already
                if (refresh && epgCache.JsonFiles.ContainsKey(seriesId) && epgCache.JsonFiles[seriesId].Images != null)
                {
                    ++refreshing;
                }
                seriesImageQueue.Add($"SH{series.SeriesId}0000");
            }
            else
            {
                seriesImageQueue.AddRange(sportsSeries.GetValues(series.SeriesId));
            }
        }
        logger.LogDebug($"Found {processedObjects} cached/unavailable series image links.");
        if (refreshing > 0)
        {
            logger.LogDebug($"Refreshing {refreshing} series image links.");
        }

        // maximum 500 queries at a time
        if (seriesImageQueue.Count > 0)
        {
            _ = Parallel.For(0, (seriesImageQueue.Count / MaxImgQueries) + 1, new ParallelOptions { MaxDegreeOfParallelism = MaxParallelDownloads }, i =>
            {
                DownloadImageResponses(seriesImageQueue, seriesImageResponses, i * MaxImgQueries);
            });

            ProcessSeriesImageResponses();
            imageDownloadQueue.EnqueueProgramMetadataCollection(seriesImageResponses);
            //await DownloadImages(seriesImageResponses, cancellationToken);
            if (processedObjects != totalObjects)
            {
                logger.LogWarning($"Failed to download and process {schedulesDirectData.SeriesInfosToProcess.Count - processedObjects} series image links.");
            }
        }

        //UpdateIcons(schedulesDirectData.SeriesInfosToProcess);

        logger.LogInformation("Exiting GetAllSeriesImages(). SUCCESS.");
        seriesImageQueue = []; sportsSeries = []; seriesImageResponses = [];
        return true;
    }

    private void ProcessSeriesImageResponses()
    {
        // process request response
        foreach (var response in seriesImageResponses)
        {
            //IncrementProgress();
            var uid = response.ProgramId;

            if (response.Data == null || response.Code != 0)
            {
                continue;
            }

            MxfSeriesInfo series = null;
            if (response.ProgramId.StartsWith("SP"))
            {
                foreach (var key in sportsSeries.AllKeys)
                {
                    if (!sportsSeries.Get(key).Contains(response.ProgramId))
                    {
                        continue;
                    }

                    series = schedulesDirectData.FindOrCreateSeriesInfo(key);
                    uid = key;
                }
            }
            else
            {
                series = schedulesDirectData.FindOrCreateSeriesInfo(response.ProgramId.Substring(2, 8));
            }
            if (series == null || !string.IsNullOrEmpty(series.GuideImage) || series.extras.ContainsKey("artwork"))
            {
                continue;
            }

            // get series images
            var artwork = SDHelpers.GetTieredImages(response.Data, ["series", "sport", "episode"]);
            if (response.ProgramId.StartsWith("SP") && artwork.Count <= 0)
            {
                continue;
            }

            series.extras.Add("artwork", artwork);
            series.mxfGuideImage = GetGuideImageAndUpdateCache(artwork, ImageType.Series, uid);
        }
    }



    private MxfGuideImage GetGuideImageAndUpdateCache(List<ProgramArtwork> artwork, ImageType type, string cacheKey = null)
    {
        if (artwork.Count == 0)
        {
            if (cacheKey != null)
            {
                epgCache.UpdateAssetImages(cacheKey, string.Empty);
            }

            return null;
        }
        if (cacheKey != null)
        {
            using var writer = new StringWriter();
            var artworkJson = JsonSerializer.Serialize(artwork);
            epgCache.UpdateAssetImages(cacheKey, artworkJson);
        }

        var setting = memoryCache.GetSetting();
        ProgramArtwork image = null;
        if (type == ImageType.Movie)
        {
            image = artwork.FirstOrDefault();
        }
        else
        {
            var aspect = setting.SDSettings.SeriesPosterArt ? "2x3" : setting.SDSettings.SeriesWsArt ? "16x9" : setting.SDSettings.SeriesPosterAspect;
            image = artwork.SingleOrDefault(arg => arg.Aspect.ToLower().Equals(aspect));
        }

        if (image == null && type == ImageType.Series)
        {
            image = artwork.SingleOrDefault(arg => arg.Aspect.ToLower().Equals("4x3"));
        }
        return image != null ? schedulesDirectData.FindOrCreateGuideImage(image.Uri) : null;
    }
}
