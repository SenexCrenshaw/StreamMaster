using System.Collections.Concurrent;
using System.Text.Json;

namespace StreamMaster.SchedulesDirect;

public class Descriptions(ILogger<Descriptions> logger, ISchedulesDirectAPIService schedulesDirectAPI, IEPGCache<Descriptions> epgCache, ISchedulesDirectDataService schedulesDirectDataService)
    : IDescriptions
{
    //private readonly int processedObjects;
    //private readonly int totalObjects;
    private List<string> seriesDescriptionQueue = [];
    private ConcurrentDictionary<string, GenericDescription> seriesDescriptionResponses = [];
    private readonly SemaphoreSlim semaphore = new(SchedulesDirect.MaxParallelDownloads, SchedulesDirect.MaxParallelDownloads);

    private bool disposedValue = false;

    public async Task<bool> BuildAllGenericSeriesInfoDescriptions(CancellationToken cancellationToken)
    {
        ResetCache();

        ISchedulesDirectData schedulesDirectData = schedulesDirectDataService.SchedulesDirectData();
        List<SeriesInfo> toProcess = schedulesDirectData.SeriesInfosToProcess;
        logger.LogInformation("Entering BuildAllGenericSeriesInfoDescriptions() for {toProcess.Count} series.", toProcess.Count);

        foreach (SeriesInfo series in toProcess)
        {
            if (series.SeriesId.StartsWith("SP") || string.IsNullOrEmpty(series.ProtoTypicalProgram))
            {
                continue;
            }

            string seriesId = $"SH{series.SeriesId}0000";
            if (epgCache.JsonFiles.TryGetValue(seriesId, out EPGJsonCache? value) && value.JsonEntry != null)
            {
                TryLoadFromCache(seriesId, series);
            }
            else
            {
                seriesDescriptionQueue.Add(series.ProtoTypicalProgram);
            }
        }

        if (seriesDescriptionQueue.Count > 0)
        {
            await ProcessDescriptionsAsync(cancellationToken).ConfigureAwait(false);
        }

        logger.LogInformation("Exiting BuildAllGenericSeriesInfoDescriptions(). SUCCESS.");
        epgCache.SaveCache();
        return true;
    }

    private void TryLoadFromCache(string seriesId, SeriesInfo series)
    {
        try
        {
            string? cache = epgCache.GetAsset(seriesId);
            if (cache is null)
            {
                return;
            }

            GenericDescription? cached = JsonSerializer.Deserialize<GenericDescription>(cache);
            if (cached?.Code == 0)
            {
                series.ShortDescription = cached.Description100;
                series.Description = cached.Description1000;
                if (!string.IsNullOrEmpty(cached.StartAirdate))
                {
                    series.StartAirdate = cached.StartAirdate;
                }
            }
        }
        catch
        {
            seriesDescriptionQueue.Add($"{series.ProtoTypicalProgram}");
        }
    }

    private async Task ProcessDescriptionsAsync(CancellationToken cancellationToken)
    {
        List<Task> tasks = [];

        for (int i = 0; i <= (seriesDescriptionQueue.Count / SchedulesDirect.MaxDescriptionQueries); i++)
        {
            int startIndex = i * SchedulesDirect.MaxDescriptionQueries;
            tasks.Add(Task.Run(async () =>
            {
                await semaphore.WaitAsync(cancellationToken);
                try
                {
                    await DownloadGenericSeriesDescriptionsAsync(startIndex, cancellationToken).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error downloading series descriptions at {StartIndex}", startIndex);
                }
                finally
                {
                    semaphore.Release();
                }
            }, cancellationToken));
        }

        await Task.WhenAll(tasks).ConfigureAwait(false);
        ProcessSeriesDescriptionsResponses();
    }

    private async Task DownloadGenericSeriesDescriptionsAsync(int start, CancellationToken cancellationToken)
    {
        if (seriesDescriptionQueue.Count - start < 1)
        {
            return;
        }

        string[] series = seriesDescriptionQueue.Skip(start).Take(SchedulesDirect.MaxDescriptionQueries).ToArray();

        Dictionary<string, GenericDescription>? responses = await schedulesDirectAPI
            .GetApiResponse<Dictionary<string, GenericDescription>?>(APIMethod.POST, "metadata/description/", series, cancellationToken)
            .ConfigureAwait(false);

        if (responses != null)
        {
            Parallel.ForEach(responses, (response) => seriesDescriptionResponses.TryAdd(response.Key, response.Value));
        }
    }

    private void ProcessSeriesDescriptionsResponses()
    {
        ISchedulesDirectData schedulesDirectData = schedulesDirectDataService.SchedulesDirectData();
        // process request response
        foreach (KeyValuePair<string, GenericDescription> response in seriesDescriptionResponses)
        {
            //IncrementProgress();

            string seriesId = response.Key;
            GenericDescription description = response.Value;

            // determine which seriesInfo this belongs to
            SeriesInfo mxfSeriesInfo = schedulesDirectData.FindOrCreateSeriesInfo(seriesId.Substring(2, 8));

            // populate descriptions
            mxfSeriesInfo.ShortDescription = description.Description100;
            mxfSeriesInfo.Description = description.Description1000;

            // serialize JSON directly to a file
            using StringWriter writer = new();
            try
            {
                string jsonString = JsonSerializer.Serialize(description);
                epgCache.AddAsset(seriesId, jsonString);
            }
            catch
            {
                // ignored
            }
        }
    }

    //private void UpdateSeriesAirdate(string seriesId, string originalAirdate)
    //{
    //    UpdateSeriesAirdate(seriesId, DateTime.Parse(originalAirdate));
    //}

    //private void UpdateSeriesAirdate(string seriesId, DateTime airdate)
    //{
    //    ISchedulesDirectData schedulesDirectData = schedulesDirectDataService.SchedulesDirectData();
    //    // write the mxf entry
    //    schedulesDirectData.FindOrCreateSeriesInfo(seriesId.Substring(2, 8)).StartAirdate = airdate.ToString("yyyy-MM-dd");

    //    // update cache if needed
    //    try
    //    {
    //        string? cache = epgCache.GetAsset(seriesId);
    //        if (cache == null)
    //        {
    //            return;
    //        }
    //        using StringReader reader = new(cache);
    //        JsonSerializerOptions options = new();

    //        GenericDescription? cached = JsonSerializer.Deserialize<GenericDescription>(reader.ReadToEnd(), options);

    //        if (cached is null || !string.IsNullOrEmpty(cached.StartAirdate))
    //        {
    //            return;
    //        }

    //        cached.StartAirdate = airdate.Equals(DateTime.MinValue) ? "" : airdate.ToString("yyyy-MM-dd");

    //        using StringWriter writer = new();
    //        string jsonString = JsonSerializer.Serialize(cached, options);
    //        epgCache.UpdateAssetJsonEntry(seriesId, jsonString);
    //    }
    //    catch
    //    {
    //        // ignored
    //    }
    //}

    //private async Task<Dictionary<string, GenericDescription>?> GetGenericDescriptionsAsync(string[] request, CancellationToken cancellationToken)
    //{
    //    DateTime dtStart = DateTime.Now;
    //    Dictionary<string, GenericDescription>? ret = await schedulesDirectAPI.GetApiResponse<Dictionary<string, GenericDescription>?>(APIMethod.POST, "metadata/description/", request, cancellationToken);
    //    if (ret != null)
    //    {
    //        logger.LogDebug($"Successfully retrieved {ret.Count}/{request.Length} generic program descriptions. ({DateTime.Now - dtStart:G})");
    //    }
    //    else
    //    {
    //        logger.LogError($"Did not receive a response from Schedules Direct for {request.Length} generic program descriptions. ({DateTime.Now - dtStart:G})");
    //    }

    //    return ret;
    //}

    public void ClearCache()
    {
        seriesDescriptionQueue = [];
        seriesDescriptionResponses = [];
    }

    public void ResetCache()
    {
        epgCache.ResetCache();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // Dispose managed resources
                semaphore.Dispose();
            }
            // Free unmanaged resources (if any) here

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
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