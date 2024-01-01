using Microsoft.Extensions.Logging;

using StreamMaster.Domain.Common;
using StreamMaster.Domain.Extensions;
using StreamMaster.SchedulesDirect.Domain.Enums;
using StreamMaster.SchedulesDirect.Helpers;

using System.Collections.Concurrent;
using System.Collections.Specialized;
using System.Text.Json;

namespace StreamMaster.SchedulesDirect;
public partial class SchedulesDirect
{

    private NameValueCollection sportsSeries = [];

    private List<string> seriesImageQueue = [];
    private ConcurrentBag<ProgramMetadata> seriesImageResponses = [];

    private bool GetAllSeriesImages()
    {
        // reset counters
        seriesImageQueue = [];
        seriesImageResponses = [];
        //IncrementNextStage(toProcess.Count);
        ISchedulesDirectData schedulesDirectData = schedulesDirectDataService.SchedulesDirectData();
        List<MxfSeriesInfo> toProcess = schedulesDirectData.SeriesInfosToProcess;

        logger.LogInformation("Entering GetAllSeriesImages() for {totalObjects} series.", toProcess.Count);
        int refreshing = 0;

        Setting setting = memoryCache.GetSetting();
        //List<string> test = toProcess.Select(a => a.ProtoTypicalProgram).Distinct().ToList();
        //List<string> programs = schedulesDirectData.Programs.Select(a => a.ProgramId).Distinct().ToList();
        // scan through each series in the mxf
        foreach (MxfSeriesInfo series in toProcess)
        {
            string seriesId;

            //MxfProgram? prog = schedulesDirectData.Programs.FirstOrDefault(a => a.ProgramId == series.ProtoTypicalProgram);
            if (string.IsNullOrEmpty(series.ProtoTypicalProgram) || !schedulesDirectData.Programs.TryGetValue(series.ProtoTypicalProgram, out MxfProgram? program))
            {
                continue;
            }

            // if image for series already exists in archive file, use it
            // cycle images for a refresh based on day of month and seriesid
            bool refresh = false;
            if (int.TryParse(series.SeriesId, out int digits))
            {
                refresh = (digits * setting.SDSettings.SDStationIds.Count % DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)) + 1 == DateTime.Now.Day;
                seriesId = $"SH{series.SeriesId}0000";
            }
            else
            {
                seriesId = series.SeriesId;
            }

            if (!refresh && epgCache.JsonFiles.TryGetValue(seriesId, out EPGJsonCache? value) && !string.IsNullOrEmpty(value.Images))
            {
                //IncrementProgress();
                if (value.Images == string.Empty)
                {
                    continue;
                }

                List<ProgramArtwork>? artwork;
                using (StringReader reader = new(value.Images))
                {
                    artwork = JsonSerializer.Deserialize<List<ProgramArtwork>>(reader.ReadToEnd());
                }

                // Add artwork to series.extras
                if (artwork != null)
                {
                    series.extras.AddOrUpdate("artwork", artwork);
                }

                MxfGuideImage? res = GetGuideImageAndUpdateCache(artwork, ImageType.Series);
                if (res != null)
                {
                    series.mxfGuideImage = res;
                }

            }
            else if (int.TryParse(series.SeriesId, out int dummy))
            {
                // only increment the refresh count if something exists already
                if (refresh && epgCache.JsonFiles.TryGetValue(seriesId, out EPGJsonCache? cache) && cache.Images != null)
                {
                    ++refreshing;
                }
                seriesImageQueue.Add($"SH{series.SeriesId}0000");
            }
            else
            {
                string[]? s = sportsSeries.GetValues(series.SeriesId);
                if (s != null)
                {
                    seriesImageQueue.AddRange(s);
                }
            }
        }
        logger.LogDebug("Found {processedObjects} cached/unavailable series image links.", processedObjects);
        if (refreshing > 0)
        {
            logger.LogDebug("Refreshing {refreshing} series image links.", refreshing);
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
                logger.LogWarning("Failed to download and process {count} series image links.", toProcess.Count - processedObjects);
            }
        }

        //UpdateIcons(toProcess);

        logger.LogInformation("Exiting GetAllSeriesImages(). SUCCESS.");
        seriesImageQueue = []; sportsSeries = []; seriesImageResponses = [];
        return true;
    }

    private void ProcessSeriesImageResponses()
    {
        Setting setting = memoryCache.GetSetting();
        string artworkSize = string.IsNullOrEmpty(setting.SDSettings.ArtworkSize) ? "Md" : setting.SDSettings.ArtworkSize;
        // process request response
        foreach (ProgramMetadata response in seriesImageResponses.Where(a => !string.IsNullOrEmpty(a.ProgramId) && a.ProgramId.Length > 8 && a.Data != null && a.Code != 0))
        {

            //if (response.ProgramId == null || response.ProgramId.Length < 8)
            //{
            //    logger.LogError("Prog id is null or too short {ProgramId}", response.ProgramId);
            //}

            //IncrementProgress();
            string programId = response.ProgramId!;
            string uid = response.ProgramId!;

            ISchedulesDirectData schedulesDirectData = schedulesDirectDataService.SchedulesDirectData();
            MxfSeriesInfo? series = null;
            if (programId.StartsWith("SP"))
            {
                foreach (string? key in sportsSeries.AllKeys)
                {
                    if (key is null)
                    {
                        continue;
                    }
                    string? sport = sportsSeries.Get(key);

                    if (sport is not null && !sport.Contains(response.ProgramId))
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
            List<ProgramArtwork> artwork = SDHelpers.GetTieredImages(response.Data, ["series", "sport", "episode"], artworkSize);
            if (response.ProgramId.StartsWith("SP") && artwork.Count <= 0)
            {
                continue;
            }

            series.extras.Add("artwork", artwork);

            MxfGuideImage? res = GetGuideImageAndUpdateCache(artwork, ImageType.Series, uid);
            if (res != null)
            {
                series.mxfGuideImage = res;
            }
        }
    }

    private MxfGuideImage? GetGuideImageAndUpdateCache(List<ProgramArtwork>? artwork, ImageType type, string? cacheKey = null)
    {
        if (artwork is null || artwork.Count == 0)
        {
            if (cacheKey != null)
            {
                epgCache.UpdateAssetImages(cacheKey, string.Empty);
            }

            return null;
        }
        if (cacheKey != null)
        {
            using StringWriter writer = new();
            string artworkJson = JsonSerializer.Serialize(artwork);
            epgCache.UpdateAssetImages(cacheKey, artworkJson);
        }

        Setting setting = memoryCache.GetSetting();
        ProgramArtwork? image = null;
        if (type == ImageType.Movie)
        {
            image = artwork.FirstOrDefault();
        }
        else
        {
            string aspect = setting.SDSettings.SeriesPosterArt ? "2x3" : setting.SDSettings.SeriesWsArt ? "16x9" : setting.SDSettings.SeriesPosterAspect;
            image = artwork.SingleOrDefault(arg => arg.Aspect.ToLower().Equals(aspect));
        }

        if (image == null && type == ImageType.Series)
        {
            image = artwork.SingleOrDefault(arg => arg.Aspect.ToLower().Equals("4x3"));
        }
        ISchedulesDirectData schedulesDirectData = schedulesDirectDataService.SchedulesDirectData();
        return image != null ? schedulesDirectData.FindOrCreateGuideImage(image.Uri) : null;
    }
}
