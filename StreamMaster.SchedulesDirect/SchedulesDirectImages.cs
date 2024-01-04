using Microsoft.Extensions.Caching.Memory;

using StreamMaster.Domain.Common;
using StreamMaster.SchedulesDirect.Domain.Enums;
using StreamMaster.SchedulesDirect.Helpers;

using System.Collections.Concurrent;
using System.Collections.Specialized;
using System.Text.Json;

namespace StreamMaster.SchedulesDirect;
public partial class SchedulesDirectImages(ILogger<SchedulesDirectImages> logger, IImageDownloadQueue imageDownloadQueue, IMemoryCache memoryCache, IIconService iconService, ISchedulesDirectAPIService schedulesDirectAPI, ISettingsService settingsService, IEPGCache<SchedulesDirectImages> epgCache, ISchedulesDirectDataService schedulesDirectDataService) : ISchedulesDirectImages
{
    public NameValueCollection sportsSeries { get; set; } = [];

    private List<string> seriesImageQueue = [];
    private ConcurrentBag<ProgramMetadata> seriesImageResponses = [];
    private int processedObjects;
    private int totalObjects;


    public async Task<bool> GetAllSeriesImages()
    {
        //epgCache.LoadCache();
        // reset counters
        seriesImageQueue = [];
        seriesImageResponses = [];

        ISchedulesDirectData schedulesDirectData = schedulesDirectDataService.SchedulesDirectData();
        List<MxfSeriesInfo> toProcess = schedulesDirectData.SeriesInfosToProcess;

        logger.LogInformation("Entering GetAllSeriesImages() for {totalObjects} series.", toProcess.Count);
        int refreshing = 0;

        Setting setting = memoryCache.GetSetting();

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

                MxfGuideImage? res = epgCache.GetGuideImageAndUpdateCache(artwork, ImageType.Series);
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

        if (seriesImageQueue.Count > 0)
        {
            SemaphoreSlim semaphore = new(SchedulesDirect.MaxParallelDownloads, SchedulesDirect.MaxParallelDownloads);
            List<Task> tasks = [];
            int processedCount = 0;

            for (int i = 0; i <= (seriesImageQueue.Count / SchedulesDirect.MaxImgQueries); i++)
            {
                int startIndex = i * SchedulesDirect.MaxImgQueries;
                tasks.Add(Task.Run(async () =>
                {
                    await semaphore.WaitAsync();
                    try
                    {
                        int itemCount = Math.Min(seriesImageQueue.Count - startIndex, SchedulesDirect.MaxImgQueries);
                        await DownloadImageResponsesAsync(seriesImageQueue, seriesImageResponses, startIndex).ConfigureAwait(false);
                        Interlocked.Add(ref processedCount, itemCount);
                        logger.LogInformation("Downloaded series image information {ProcessedCount} of {TotalCount}", processedCount, seriesImageQueue.Count);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Error downloading series images at {StartIndex}", startIndex);
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                }));
            }

            await Task.WhenAll(tasks).ConfigureAwait(false);

            // Continue with the rest of your processing
            ProcessSeriesImageResponses();
            imageDownloadQueue.EnqueueProgramMetadataCollection(seriesImageResponses);

            if (processedObjects != totalObjects)
            {
                logger.LogWarning("Failed to download and process {FailedCount} series image links.", toProcess.Count - processedObjects);
            }
        }


        // maximum 500 queries at a time
        //if (seriesImageQueue.Count > 0)
        //{
        //    _ = Parallel.For(0, (seriesImageQueue.Count / SchedulesDirect.MaxImgQueries) + 1, new ParallelOptions { MaxDegreeOfParallelism = SchedulesDirect.MaxParallelDownloads }, i =>
        //    {
        //        DownloadImageResponses(seriesImageQueue, seriesImageResponses, i * SchedulesDirect.MaxImgQueries);
        //    });

        //    ProcessSeriesImageResponses();
        //    imageDownloadQueue.EnqueueProgramMetadataCollection(seriesImageResponses);
        //    //await DownloadImages(seriesImageResponses, cancellationToken);
        //    if (processedObjects != totalObjects)
        //    {
        //        logger.LogWarning("Failed to download and process {count} series image links.", toProcess.Count - processedObjects);
        //    }
        //}

        //UpdateIcons(toProcess);

        logger.LogInformation("Exiting GetAllSeriesImages(). SUCCESS.");
        seriesImageQueue = []; sportsSeries = []; seriesImageResponses = [];
        epgCache.SaveCache();
        return true;
    }

    public async Task<List<ProgramMetadata>?> GetArtworkAsync(string[] request)
    {
        DateTime dtStart = DateTime.Now;
        List<ProgramMetadata>? ret = await schedulesDirectAPI.GetApiResponse<List<ProgramMetadata>>(APIMethod.POST, "metadata/programs/", request);
        if (ret != null)
        {
            logger.LogDebug($"Successfully retrieved artwork info for {ret.Count}/{request.Length} programs. ({DateTime.Now - dtStart:G})");
        }
        else
        {
            logger.LogDebug($"Did not receive a response from Schedules Direct for artwork info of {request.Length} programs. ({DateTime.Now - dtStart:G})");
        }

        return ret;
    }

    private async Task DownloadImageResponsesAsync(List<string> imageQueue, ConcurrentBag<ProgramMetadata> programMetadata, int start = 0)
    {
        // Reject 0 requests
        if (imageQueue.Count - start < 1)
        {
            return;
        }

        // Build the array of series to request images for
        string[] series = new string[Math.Min(imageQueue.Count - start, SchedulesDirect.MaxImgQueries)];
        for (int i = 0; i < series.Length; ++i)
        {
            series[i] = imageQueue[start + i];
        }

        // Request images from Schedules Direct
        List<ProgramMetadata>? responses = await GetArtworkAsync(series).ConfigureAwait(false);
        if (responses != null)
        {
            foreach (ProgramMetadata response in responses)
            {

                programMetadata.Add(response);
            }
        }
        else
        {
            logger.LogInformation("Did not receive a response from Schedules Direct for artwork info of {count} programs, first entry {entry}.", series.Length, series.Any() ? series[0] : "");
        }
    }

    //private void DownloadImageResponses(List<string> imageQueue, ConcurrentBag<ProgramMetadata> programMetadata, int start = 0)
    //{
    //    // reject 0 requests
    //    if (imageQueue.Count - start < 1)
    //    {
    //        return;
    //    }

    //    // build the array of series to request images for
    //    string[] series = new string[Math.Min(imageQueue.Count - start, SchedulesDirect.MaxImgQueries)];
    //    for (int i = 0; i < series.Length; ++i)
    //    {
    //        series[i] = imageQueue[start + i];
    //    }

    //    // request images from Schedules Direct
    //    List<ProgramMetadata>? responses = GetArtworkAsync(series).Result;
    //    if (responses != null)
    //    {
    //        _ = Parallel.ForEach(responses, programMetadata.Add);
    //    }
    //    else
    //    {
    //        logger.LogInformation("Did not receive a response from Schedules Direct for artwork info of {count} programs, first entry {entry}.", series.Length, series.Any() ? series[0] : "");
    //    }
    //}

    private void ProcessSeriesImageResponses()
    {
        Setting setting = memoryCache.GetSetting();
        string artworkSize = string.IsNullOrEmpty(setting.SDSettings.ArtworkSize) ? "Md" : setting.SDSettings.ArtworkSize;
        // process request response
        IEnumerable<ProgramMetadata> toProcess = seriesImageResponses.Where(a => !string.IsNullOrEmpty(a.ProgramId) && a.Data != null && a.Code == 0);
        logger.LogInformation("Processing {count} series image responses.", toProcess.Count());
        foreach (ProgramMetadata response in seriesImageResponses)
        {

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

            MxfGuideImage? res = epgCache.GetGuideImageAndUpdateCache(artwork, ImageType.Series, uid);
            if (res != null)
            {
                series.mxfGuideImage = res;
            }
        }
    }

    public void ResetCache()
    {
        sportsImageQueue = [];
        sportsImageResponses = [];
        sportsSeries = [];

        seasons = [];
        seasonImageQueue = [];
        seasonImageResponses = [];

        movieImageQueue = [];
        movieImageResponses = [];

        processedObjects = 0;
        totalObjects = 0;
    }

    public void ClearCache()
    {
        epgCache.ResetCache();
    }
}
