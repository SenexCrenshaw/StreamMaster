using System.Collections.Concurrent;
using System.Collections.Specialized;
using System.Text.Json;
namespace StreamMaster.SchedulesDirect.Images;

public class SeriesImages(
    ILogger<SeriesImages> logger,
    IEPGCache<SeriesImages> epgCache,
    IImageDownloadQueue imageDownloadQueue,
    IOptionsMonitor<SDSettings> sdSettings,
    ISchedulesDirectAPIService schedulesDirectAPI,
    ISchedulesDirectDataService schedulesDirectDataService) : ISeriesImages, IDisposable
{
    private List<string> seriesImageQueue = [];
    private ConcurrentBag<ProgramMetadata> seriesImageResponses = [];
    private readonly ConcurrentHashSet<string> seriesImageProcessed = [];
    public NameValueCollection SportsSeries { get; set; } = [];

    private int processedObjects;

    private readonly SemaphoreSlim semaphore = new(SchedulesDirect.MaxParallelDownloads);

    public async Task<bool> GetAllSeriesImages()
    {
        if (!sdSettings.CurrentValue.SeriesImages)
        {
            return true;
        }

        // Reset state
        seriesImageQueue = [];
        seriesImageResponses = [];
        processedObjects = 0;

        ISchedulesDirectData schedulesDirectData = schedulesDirectDataService.SchedulesDirectData();
        ICollection<SeriesInfo> toProcess = schedulesDirectData.SeriesInfosToProcess.Values;

        logger.LogInformation("Entering GetAllSeriesImages() for {totalObjects} series.", toProcess.Count);

        //List<SeriesInfo> a = toProcess.Where(a => a.Id.ContainsIgnoreCase("5318")).ToList();
        foreach (SeriesInfo series in toProcess)
        {
            if (string.IsNullOrEmpty(series.ProtoTypicalProgram) ||
                !schedulesDirectData.Programs.TryGetValue(series.ProtoTypicalProgram, out MxfProgram? _))
            {
                continue;
            }

            bool refresh = ShouldRefreshSeries(series.SeriesId, out string seriesId);

            if (
                !refresh &&
                epgCache.JsonFiles.TryGetValue(seriesId, out EPGJsonCache? value) && !string.IsNullOrEmpty(value.JsonEntry))
            {
                ProcessCachedImages(series, value);
                //seriesImageQueue.Add($"SH{series.SeriesId}0000");
                imageDownloadQueue.EnqueueProgramArtworkCollection(series.ArtWorks);
            }
            else
            {
                string[]? sport = SportsSeries.GetValues(series.SeriesId);
                if (sport != null)
                {
                    seriesImageQueue.AddRange(sport);
                }
                else
                {
                    seriesImageQueue.Add($"SH{series.SeriesId}0000");
                }
            }
        }

        logger.LogDebug("Found {processedObjects} cached/unavailable series image links.", processedObjects);

        if (seriesImageQueue.Count > 0)
        {
            await DownloadAndProcessImagesAsync().ConfigureAwait(false);
        }

        logger.LogInformation("Exiting Series Images SUCCESS.");

        epgCache.SaveCache();
        ClearCache();
        return true;
    }

    private bool ShouldRefreshSeries(string seriesId, out string finalSeriesId)
    {
        if (int.TryParse(seriesId, out int digits))
        {
            finalSeriesId = $"SH{seriesId}0000";
            if (!seriesImageProcessed.Contains(seriesId))
            {
                seriesImageProcessed.Add(seriesId);
                return true;
            }

            return (digits * sdSettings.CurrentValue.SDStationIds.Count % DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)) + 1 == DateTime.Now.Day;
        }

        finalSeriesId = seriesId;

        return false;
    }

    private static void ProcessCachedImages(SeriesInfo series, EPGJsonCache cachedFile)
    {
        series.ArtWorks = string.IsNullOrEmpty(cachedFile.JsonEntry)
             ? []
             : JsonSerializer.Deserialize<List<ProgramArtwork>>(cachedFile.JsonEntry) ?? [];
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
                    _ = Interlocked.Add(ref processedCount, itemCount);
                    logger.LogInformation("Downloaded series image information {ProcessedCount} of {TotalCount}", processedCount, seriesImageQueue.Count);
                }
                finally
                {
                    _ = semaphore.Release();
                }
            }));
        }

        await Task.WhenAll(tasks).ConfigureAwait(false);
        ProcessSeriesImageResponses();
    }

    private void ProcessSeriesImageResponses()
    {
        string artworkSize = string.IsNullOrEmpty(sdSettings.CurrentValue.ArtworkSize) ? BuildInfo.DefaultSDImageSize : sdSettings.CurrentValue.ArtworkSize;

        IEnumerable<ProgramMetadata> toProcess = seriesImageResponses.Where(a => !string.IsNullOrEmpty(a.ProgramId) && a.Data != null && a.Code == 0);

        logger.LogInformation("Processing {count} series image responses.", toProcess.Count());

        foreach (ProgramMetadata response in toProcess)
        {
            ++processedObjects;

            if (response.ProgramId.StartsWith("SP"))
            {
                continue;
            }

            if (response.Data == null || response.Data.Count == 0)
            {
                logger.LogWarning("No Series Image artwork found for {ProgramId}", response.ProgramId);
                continue;
            }

            ISchedulesDirectData schedulesDirectData = schedulesDirectDataService.SchedulesDirectData();
            SeriesInfo? series = response.ProgramId.StartsWith("SP") ? GetSportsSeries(response) : schedulesDirectData.FindOrCreateSeriesInfo(response.ProgramId.Substring(2, 8));
            if (series == null)
            {
                continue;
            }

            List<ProgramArtwork> artworks = SDHelpers.GetTieredImages(response.Data, artworkSize, ["series", "sport", "episode"], sdSettings.CurrentValue.SeriesPosterAspect);
            series.AddArtwork(artworks);
            epgCache.UpdateProgramArtworkCache(artworks, ImageType.Series, response.ProgramId);

            if (artworks.Count > 0)
            {
                imageDownloadQueue.EnqueueProgramArtworkCollection(artworks);
            }
            else
            {
                logger.LogWarning("No artwork found for {ProgramId}", response.ProgramId);
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

    public List<string> GetExpiredKeys()
    {
        return epgCache.GetExpiredKeys();
    }

    public void RemovedExpiredKeys(List<string>? keysToDelete = null)
    {
        epgCache.RemovedExpiredKeys(keysToDelete);
    }
}