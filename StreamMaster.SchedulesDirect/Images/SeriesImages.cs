using System.Collections.Concurrent;
using System.Collections.Specialized;
using System.Text.Json;

namespace StreamMaster.SchedulesDirect.Images;

public class SeriesImages(
    ILogger<SeriesImages> logger,
    IEPGCache<SeriesImages> epgCache,
    IImageDownloadQueue imageDownloadQueue,
    IOptionsMonitor<SDSettings> intSettings,
    ISchedulesDirectAPIService schedulesDirectAPI,
    ISchedulesDirectDataService schedulesDirectDataService) : ISeriesImages, IDisposable
{
    private List<string> seriesImageQueue = [];
    private ConcurrentBag<ProgramMetadata> seriesImageResponses = [];
    public NameValueCollection SportsSeries { get; set; } = [];

    private int processedObjects;

    private readonly SemaphoreSlim semaphore = new(SchedulesDirect.MaxParallelDownloads);

    public async Task<bool> GetAllSeriesImages()
    {
        // Reset state
        seriesImageQueue = [];
        seriesImageResponses = [];
        processedObjects = 0;

        ISchedulesDirectData schedulesDirectData = schedulesDirectDataService.SchedulesDirectData();
        List<SeriesInfo> toProcess = schedulesDirectData.SeriesInfosToProcess;

        logger.LogInformation("Entering GetAllSeriesImages() for {totalObjects} series.", toProcess.Count);
        int refreshing = 0;

        foreach (SeriesInfo series in toProcess)
        {
            if (string.IsNullOrEmpty(series.ProtoTypicalProgram) ||
                !schedulesDirectData.Programs.TryGetValue(series.ProtoTypicalProgram, out MxfProgram? program))
            {
                continue;
            }

            bool refresh = ShouldRefreshSeries(series.SeriesId, out string seriesId);

            if (!refresh && epgCache.JsonFiles.TryGetValue(seriesId, out EPGJsonCache? value) && !string.IsNullOrEmpty(value.Images))
            {
                ProcessCachedImages(series, value);
            }
            else if (int.TryParse(series.SeriesId, out _))
            {
                if (refresh && epgCache.JsonFiles.TryGetValue(seriesId, out EPGJsonCache? cache) && cache.Images != null)
                {
                    ++refreshing;
                }
                seriesImageQueue.Add($"SH{series.SeriesId}0000");
            }
            else
            {
                string[]? sport = SportsSeries.GetValues(series.SeriesId);
                if (sport != null)
                {
                    seriesImageQueue.AddRange(sport);
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
            await DownloadAndProcessImagesAsync().ConfigureAwait(false);
        }

        logger.LogInformation("Exiting GetAllSeriesImages(). SUCCESS.");
        ResetCache();
        epgCache.SaveCache();
        return true;
    }

    private bool ShouldRefreshSeries(string seriesId, out string finalSeriesId)
    {
        bool refresh = false;
        if (int.TryParse(seriesId, out int digits))
        {
            refresh = (digits * intSettings.CurrentValue.SDStationIds.Count % DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)) + 1 == DateTime.Now.Day;
            finalSeriesId = $"SH{seriesId}0000";
        }
        else
        {
            finalSeriesId = seriesId;
        }
        return refresh;
    }

    private void ProcessCachedImages(SeriesInfo series, EPGJsonCache value)
    {
        if (string.IsNullOrEmpty(value.Images))
        {
            return;
        }

        List<ProgramArtwork>? artwork = JsonSerializer.Deserialize<List<ProgramArtwork>>(value.Images);
        if (artwork != null)
        {
            series.Extras.AddOrUpdate("artwork", artwork);
            MxfGuideImage? guideImage = epgCache.GetGuideImageAndUpdateCache(artwork, ImageType.Series);
            if (guideImage != null)
            {
                series.MxfGuideImage = guideImage;
            }
        }
    }

    private async Task DownloadAndProcessImagesAsync()
    {
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
                finally
                {
                    semaphore.Release();
                }
            }));
        }

        await Task.WhenAll(tasks).ConfigureAwait(false);
        ProcessSeriesImageResponses();
        imageDownloadQueue.EnqueueProgramMetadataCollection(seriesImageResponses);
    }

    private void ProcessSeriesImageResponses()
    {
        string artworkSize = string.IsNullOrEmpty(intSettings.CurrentValue.ArtworkSize) ? "Md" : intSettings.CurrentValue.ArtworkSize;
        IEnumerable<ProgramMetadata> toProcess = seriesImageResponses.Where(a => !string.IsNullOrEmpty(a.ProgramId) && a.Data != null && a.Code == 0);

        logger.LogInformation("Processing {count} series image responses.", toProcess.Count());

        foreach (ProgramMetadata response in toProcess)
        {
            ++processedObjects;
            ISchedulesDirectData schedulesDirectData = schedulesDirectDataService.SchedulesDirectData();
            SeriesInfo? series = response.ProgramId.StartsWith("SP") ? GetSportsSeries(response) : schedulesDirectData.FindOrCreateSeriesInfo(response.ProgramId.Substring(2, 8));

            if (series == null || !string.IsNullOrEmpty(series.GuideImage) || series.Extras.ContainsKey("artwork"))
            {
                continue;
            }

            List<ProgramArtwork> artwork = SDHelpers.GetTieredImages(response.Data, ["series", "sport", "episode"], artworkSize);
            if (response.ProgramId.StartsWith("SP") && artwork.Count == 0)
            {
                continue;
            }

            series.Extras.Add("artwork", artwork);
            MxfGuideImage? res = epgCache.GetGuideImageAndUpdateCache(artwork, ImageType.Series, response.ProgramId);
            if (res != null)
            {
                series.MxfGuideImage = res;
            }
        }
    }

    private SeriesInfo? GetSportsSeries(ProgramMetadata response)
    {
        ISchedulesDirectData schedulesDirectData = schedulesDirectDataService.SchedulesDirectData();
        foreach (string? key in SportsSeries.AllKeys)
        {
            if (key is null)
            {
                continue;
            }

            string? sport = SportsSeries.Get(key);
            if (sport?.Contains(response.ProgramId) == true)
            {
                return schedulesDirectData.FindOrCreateSeriesInfo(key);
            }
        }
        return null;
    }

    public void ClearCache()
    {
        seriesImageQueue.Clear();
        seriesImageResponses.Clear();
        SportsSeries.Clear();

        processedObjects = 0;
    }

    public void ResetCache()
    {
        epgCache.ResetCache();
    }

    public void Dispose()
    {
        semaphore.Dispose();
        GC.SuppressFinalize(this);
    }
}