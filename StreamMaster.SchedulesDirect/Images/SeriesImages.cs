using System.Collections.Concurrent;
using System.Collections.Specialized;
using System.Text.Json;

namespace StreamMaster.SchedulesDirect.Images;
public class SeriesImages(ILogger<SeriesImages> logger, IEPGCache<SeriesImages> epgCache, IImageDownloadQueue imageDownloadQueue, IOptionsMonitor<SDSettings> intSettings, ISchedulesDirectAPIService schedulesDirectAPI, ISchedulesDirectDataService schedulesDirectDataService) : ISeriesImages
{
    private readonly SDSettings sdsettings = intSettings.CurrentValue;

    private List<string> seriesImageQueue = [];
    private ConcurrentBag<ProgramMetadata> seriesImageResponses = [];
    public NameValueCollection SportsSeries { get; set; } = [];

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
                refresh = (digits * sdsettings.SDStationIds.Count % DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)) + 1 == DateTime.Now.Day;
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
                string[]? s = SportsSeries.GetValues(series.SeriesId);
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

            for (int i = 0; i <= seriesImageQueue.Count / SchedulesDirect.MaxImgQueries; i++)
            {
                int startIndex = i * SchedulesDirect.MaxImgQueries;
                tasks.Add(Task.Run(async () =>
                {
                    await semaphore.WaitAsync();
                    try
                    {
                        int itemCount = Math.Min(seriesImageQueue.Count - startIndex, SchedulesDirect.MaxImgQueries);
                        await schedulesDirectAPI.DownloadImageResponsesAsync(seriesImageQueue, seriesImageResponses, startIndex).ConfigureAwait(false);
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


        //UpdateIcons(toProcess);

        logger.LogInformation("Exiting GetAllSeriesImages(). SUCCESS.");
        seriesImageQueue = []; SportsSeries = []; seriesImageResponses = [];
        epgCache.SaveCache();
        return true;
    }

    private void ProcessSeriesImageResponses()
    {

        string artworkSize = string.IsNullOrEmpty(sdsettings.ArtworkSize) ? "Md" : sdsettings.ArtworkSize;
        // process request response
        IEnumerable<ProgramMetadata> toProcess = seriesImageResponses.Where(a => !string.IsNullOrEmpty(a.ProgramId) && a.Data != null && a.Code == 0);
        logger.LogInformation("Processing {count} series image responses.", toProcess.Count());
        foreach (ProgramMetadata response in seriesImageResponses)
        {
            ++processedObjects;
            //IncrementProgress();
            string programId = response.ProgramId!;
            string uid = response.ProgramId!;

            ISchedulesDirectData schedulesDirectData = schedulesDirectDataService.SchedulesDirectData();
            MxfSeriesInfo? series = null;
            if (programId.StartsWith("SP"))
            {
                foreach (string? key in SportsSeries.AllKeys)
                {
                    if (key is null)
                    {
                        continue;
                    }
                    string? sport = SportsSeries.Get(key);

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
        seriesImageQueue.Clear();
        seriesImageResponses.Clear();
        SportsSeries.Clear();

        processedObjects = 0;
        totalObjects = 0;
    }

    public void ClearCache()
    {
        epgCache.ResetCache();
    }
}
