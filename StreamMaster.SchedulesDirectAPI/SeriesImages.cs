using Microsoft.Extensions.Logging;

using StreamMaster.SchedulesDirectAPI.Domain.Enums;

using System.Collections.Concurrent;
using System.Collections.Specialized;
using System.Text.Json;

namespace StreamMaster.SchedulesDirectAPI;
public partial class SchedulesDirect
{
    
    private  NameValueCollection sportsSeries = [];

    private List<string> seriesImageQueue = [];
    private ConcurrentBag<ProgramMetadata> seriesImageResponses = [];

    private  async Task<bool> GetAllSeriesImages(CancellationToken cancellationToken)
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
                refresh = digits * setting.SDSettings.SDStationIds.Count % DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month) + 1 == DateTime.Now.Day;
                seriesId = $"SH{series.SeriesId}0000";
            }
            else
            {
                seriesId = series.SeriesId;
            }

            if (!refresh && epgCache.JsonFiles.ContainsKey(seriesId) && !string.IsNullOrEmpty(epgCache.JsonFiles[seriesId].Images))
            {
                //IncrementProgress();
                if (epgCache.JsonFiles[seriesId].Images == string.Empty) continue;

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
            Parallel.For(0, (seriesImageQueue.Count / MaxImgQueries + 1), new ParallelOptions { MaxDegreeOfParallelism = MaxParallelDownloads }, i =>
            {
                DownloadImageResponses( seriesImageQueue, seriesImageResponses, i * MaxImgQueries);
            });

            ProcessSeriesImageResponses();
             await DownloadImages(seriesImageResponses, cancellationToken);
            if (processedObjects != totalObjects)
            {
                logger.LogWarning($"Failed to download and process {schedulesDirectData.SeriesInfosToProcess.Count - processedObjects} series image links.");
            }
        }
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

            if (response.Data == null || response.Code != 0) continue;
            MxfSeriesInfo series = null;
            if (response.ProgramId.StartsWith("SP"))
            {
                foreach (var key in sportsSeries.AllKeys)
                {
                    if (!sportsSeries.Get(key).Contains(response.ProgramId)) continue;
                    series = schedulesDirectData.FindOrCreateSeriesInfo(key);
                    uid = key;
                }
            }
            else
            {
                series = schedulesDirectData.FindOrCreateSeriesInfo(response.ProgramId.Substring(2, 8));
            }
            if (series == null || !string.IsNullOrEmpty(series.GuideImage) || series.extras.ContainsKey("artwork")) continue;

            // get series images
            var artwork = GetTieredImages(response.Data, new List<string> { "series", "sport", "episode" });
            if (response.ProgramId.StartsWith("SP") && artwork.Count <= 0) continue;
            series.extras.Add("artwork", artwork);
            series.mxfGuideImage = GetGuideImageAndUpdateCache(artwork, ImageType.Series, uid);
        }
    }

    private  List<ProgramArtwork> GetTieredImages(List<ProgramArtwork> sdImages, List<string> tiers)
    {
        var setting = memoryCache.GetSetting();
        var ret = new List<ProgramArtwork>();
        var images = sdImages.Where(arg =>
            !string.IsNullOrEmpty(arg.Category) && !string.IsNullOrEmpty(arg.Aspect) && !string.IsNullOrEmpty(arg.Uri) &&
            (string.IsNullOrEmpty(arg.Tier) || tiers.Contains(arg.Tier.ToLower())) &&
            !string.IsNullOrEmpty(arg.Size) && arg.Size.Equals(setting.SDSettings.ArtworkSize));

        // get the aspect ratios available and fix the URI
        var aspects = new HashSet<string>();
        foreach (var image in images)
        {
            aspects.Add(image.Aspect);
            //if (!image.Uri.ToLower().StartsWith("http"))
            //{
            //    image.Uri = $"{api.BaseArtworkAddress}image/{image.Uri.ToLower()}";
            //}
        }

        // determine which image to return with each aspect
        foreach (var aspect in aspects)
        {
            var imgAspects = images.Where(arg => arg.Aspect.Equals(aspect));

            var links = new ProgramArtwork[11];
            foreach (var image in imgAspects)
            {
                switch (image.Category.ToLower())
                {
                    case "box art":     // DVD box art, for movies only
                        if (links[0] == null) links[0] = image;
                        break;
                    case "vod art":
                        if (links[1] == null) links[1] = image;
                        break;
                    case "poster art":  // theatrical movie poster, standard sizes
                        if (links[2] == null) links[2] = image;
                        break;
                    case "banner":      // source-provided image, usually shows cast ensemble with source-provided text
                        if (links[3] == null) links[3] = image;
                        break;
                    case "banner-l1":   // same as Banner
                        if (links[4] == null) links[4] = image;
                        break;
                    case "banner-l2":   // source-provided image with plain text
                        if (links[5] == null) links[5] = image;
                        break;
                    case "banner-lo":   // banner with Logo Only
                        if (links[6] == null) links[6] = image;
                        break;
                    case "logo":        // official logo for program, sports organization, sports conference, or TV station
                        if (links[7] == null) links[7] = image;
                        break;
                    case "banner-l3":   // stock photo image with plain text
                        if (links[8] == null) links[8] = image;
                        break;
                    case "iconic":      // representative series/season/episode image, no text
                        if (tiers.Contains("series") && links[9] == null) links[9] = image;
                        break;
                    case "staple":      // the staple image is intended to cover programs which do not have a unique banner image
                        if (links[10] == null) links[10] = image;
                        break;
                    case "banner-l1t":
                    case "banner-lot":  // banner with Logo Only + Text indicating season number
                        break;
                }
            }

            foreach (var link in links)
            {
                if (link == null) continue;
                ret.Add(link);
                break;
            }
        }

        if (ret.Count > 1)
        {
            ret = ret.OrderBy(arg => arg.Width).ToList();
        }
        return ret;
    }

    private  MxfGuideImage GetGuideImageAndUpdateCache(List<ProgramArtwork> artwork, ImageType type, string cacheKey = null)
    {
        if (artwork.Count == 0)
        {
            if (cacheKey != null) epgCache.UpdateAssetImages(cacheKey, string.Empty);
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
        return image != null ? schedulesDirectData.FindOrCreateGuideImage( image.Uri) : null;
    }
}
