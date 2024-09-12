using StreamMaster.SchedulesDirect.Domain.Enums;

using System.Collections.Concurrent;
using System.Text.Json;

namespace StreamMaster.SchedulesDirect;
public class Descriptions(ILogger<Descriptions> logger, ISchedulesDirectAPIService schedulesDirectAPI, IEPGCache<Descriptions> epgCache, ISchedulesDirectDataService schedulesDirectDataService) : IDescriptions
{
    private readonly int processedObjects;
    private readonly int totalObjects;
    private List<string> seriesDescriptionQueue = [];
    private ConcurrentDictionary<string, GenericDescription> seriesDescriptionResponses = [];

    public void ResetCache()
    {
        seriesDescriptionQueue = [];
        seriesDescriptionResponses = [];
    }

    public async Task<bool> BuildAllGenericSeriesInfoDescriptions()
    {
        ISchedulesDirectData schedulesDirectData = schedulesDirectDataService.SchedulesDirectData();
        // reset counters
        seriesDescriptionQueue = [];
        seriesDescriptionResponses = [];

        List<SeriesInfo> a = schedulesDirectData.SeriesInfosToProcess;
        ConcurrentDictionary<string, MxfProgram> b = schedulesDirectData.Programs;
        ConcurrentBag<MxfProvider> c = schedulesDirectData.Providers;
        List<SeriesInfo> toProcess = schedulesDirectData.SeriesInfosToProcess;

        logger.LogInformation($"Entering BuildAllGenericSeriesInfoDescriptions() for {toProcess.Count} series.");

        // fill mxf programs with cached values and queue the rest
        foreach (SeriesInfo series in toProcess)
        {
            // sports events will not have a generic description
            if (series.SeriesId.StartsWith("SP") || string.IsNullOrEmpty(series.ProtoTypicalProgram))
            {
                //IncrementProgress();
                continue;
            }

            // import the cached description if exists, otherwise queue it up
            string seriesId = $"SH{series.SeriesId}0000";
            if (epgCache.JsonFiles.ContainsKey(seriesId) && epgCache.JsonFiles[seriesId].JsonEntry != null)
            {
                try
                {
                    using StringReader reader = new(epgCache.GetAsset(seriesId));
                    GenericDescription? cached = JsonSerializer.Deserialize<GenericDescription>(reader.ReadToEnd());

                    if (cached?.Code == 0)
                    {
                        series.ShortDescription = cached.Description100;
                        series.Description = cached.Description1000;
                        if (!string.IsNullOrEmpty(cached.StartAirdate))
                        {
                            series.StartAirdate = cached.StartAirdate;
                        }
                    }

                    //IncrementProgress();
                }
                catch
                {
                    if (int.TryParse(series.SeriesId, out int dummy))
                    {
                        // must use EP to query generic series description
                        seriesDescriptionQueue.Add($"{series.ProtoTypicalProgram}");
                    }
                    else
                    {
                        //IncrementProgress();
                    }
                }
            }
            else if (!int.TryParse(series.SeriesId, out int dummy) || (series.ProtoTypicalProgram is not null && series.ProtoTypicalProgram.StartsWith("SH")))
            {
                //IncrementProgress();
            }
            else
            {
                // must use EP to query generic series description
                seriesDescriptionQueue.Add($"{series.ProtoTypicalProgram}");
            }
        }
        logger.LogInformation($"Found {processedObjects} cached/unavailable series descriptions.");

        // maximum 500 queries at a time
        //if (seriesDescriptionQueue.Count > 0)
        //{
        //    Parallel.For(0, (seriesDescriptionQueue.Count / SchedulesDirect.MaxDescriptionQueries) + 1, new ParallelOptions { MaxDegreeOfParallelism = SchedulesDirect.MaxParallelDownloads }, i =>
        //    {
        //        DownloadGenericSeriesDescriptions(i * SchedulesDirect.MaxDescriptionQueries);
        //    });

        //    ProcessSeriesDescriptionsResponses();
        //    if (processedObjects != totalObjects)
        //    {
        //        logger.LogWarning($"Failed to download and process {schedulesDirectData.SeriesInfosToProcess.Count - processedObjects} series descriptions.");
        //    }
        //}
        int processedCount = 0;
        if (seriesDescriptionQueue.Count > 0)
        {
            SemaphoreSlim semaphore = new(SchedulesDirect.MaxParallelDownloads, SchedulesDirect.MaxParallelDownloads);
            List<Task> tasks = [];

            for (int i = 0; i <= (seriesDescriptionQueue.Count / SchedulesDirect.MaxDescriptionQueries); i++)
            {
                int startIndex = i * SchedulesDirect.MaxDescriptionQueries;
                tasks.Add(Task.Run(async () =>
                {
                    await semaphore.WaitAsync();
                    try
                    {
                        int itemCount = Math.Min(seriesDescriptionQueue.Count - startIndex, SchedulesDirect.MaxDescriptionQueries);
                        await DownloadGenericSeriesDescriptionsAsync(startIndex).ConfigureAwait(false);
                        Interlocked.Add(ref processedCount, itemCount);
                        logger.LogInformation("Downloaded series descriptions {ProcessedCount} of {TotalCount}", processedCount, seriesDescriptionQueue.Count);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Error downloading series descriptions at {StartIndex}", startIndex);
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                }));
            }

            await Task.WhenAll(tasks).ConfigureAwait(false);

            // Continue with the rest of your processing
            ProcessSeriesDescriptionsResponses();

            if (processedObjects != totalObjects)
            {
                logger.LogWarning("Failed to download and process {FailedCount} series descriptions.", schedulesDirectData.SeriesInfosToProcess.Count - processedObjects);
            }
        }

        logger.LogInformation("Exiting BuildAllGenericSeriesInfoDescriptions(). SUCCESS.");
        seriesDescriptionQueue = []; seriesDescriptionResponses = [];
        epgCache.SaveCache();
        return true;
    }

    private async Task DownloadGenericSeriesDescriptionsAsync(int start = 0)
    {
        // Reject 0 requests
        if (seriesDescriptionQueue.Count - start < 1)
        {
            return;
        }

        // Build the array of series to request descriptions for
        string[] series = new string[Math.Min(seriesDescriptionQueue.Count - start, SchedulesDirect.MaxImgQueries)];
        for (int i = 0; i < series.Length; ++i)
        {
            series[i] = seriesDescriptionQueue[start + i];
        }

        // Request descriptions from Schedules Direct
        Dictionary<string, GenericDescription>? responses = await schedulesDirectAPI.GetApiResponse<Dictionary<string, GenericDescription>?>(APIMethod.POST, "metadata/description/", series).ConfigureAwait(false);

        if (responses != null)
        {
            // Process responses in parallel
            Parallel.ForEach(responses, (response) =>
            {
                seriesDescriptionResponses.TryAdd(response.Key, response.Value);
            });
        }
    }
    private void DownloadGenericSeriesDescriptions(int start = 0)
    {
        // reject 0 requests
        if (seriesDescriptionQueue.Count - start < 1)
        {
            return;
        }

        // build the array of series to request descriptions for
        string[] series = new string[Math.Min(seriesDescriptionQueue.Count - start, SchedulesDirect.MaxDescriptionQueries)];
        for (int i = 0; i < series.Length; ++i)
        {
            series[i] = seriesDescriptionQueue[start + i];
        }

        IEnumerable<string> test = series.Where(string.IsNullOrEmpty);
        //if (test.Any())
        //{
        //    int aaa = 1;
        //}
        // request descriptions from Schedules Direct
        //Dictionary<string, GenericDescription>? responses = GetGenericDescriptionsAsync(series, CancellationToken.None).Result;
        Dictionary<string, GenericDescription>? responses = schedulesDirectAPI.GetApiResponse<Dictionary<string, GenericDescription>?>(APIMethod.POST, "metadata/description/", series).Result;
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

    private void UpdateSeriesAirdate(string seriesId, string originalAirdate)
    {
        UpdateSeriesAirdate(seriesId, DateTime.Parse(originalAirdate));
    }
    private void UpdateSeriesAirdate(string seriesId, DateTime airdate)
    {
        ISchedulesDirectData schedulesDirectData = schedulesDirectDataService.SchedulesDirectData();
        // write the mxf entry
        schedulesDirectData.FindOrCreateSeriesInfo(seriesId.Substring(2, 8)).StartAirdate = airdate.ToString("yyyy-MM-dd");

        // update cache if needed
        try
        {
            using StringReader reader = new(epgCache.GetAsset(seriesId));
            JsonSerializerOptions options = new();

            GenericDescription? cached = JsonSerializer.Deserialize<GenericDescription>(reader.ReadToEnd(), options);

            if (!string.IsNullOrEmpty(cached.StartAirdate))
            {
                return;
            }

            cached.StartAirdate = airdate.Equals(DateTime.MinValue) ? "" : airdate.ToString("yyyy-MM-dd");

            using StringWriter writer = new();
            string jsonString = JsonSerializer.Serialize(cached, options);
            epgCache.UpdateAssetJsonEntry(seriesId, jsonString);
        }
        catch
        {
            // ignored
        }
    }

    private async Task<Dictionary<string, GenericDescription>?> GetGenericDescriptionsAsync(string[] request, CancellationToken cancellationToken)
    {
        DateTime dtStart = DateTime.Now;
        Dictionary<string, GenericDescription>? ret = await schedulesDirectAPI.GetApiResponse<Dictionary<string, GenericDescription>?>(APIMethod.POST, "metadata/description/", request, cancellationToken);
        if (ret != null)
        {
            logger.LogDebug($"Successfully retrieved {ret.Count}/{request.Length} generic program descriptions. ({DateTime.Now - dtStart:G})");
        }
        else
        {
            logger.LogError($"Did not receive a response from Schedules Direct for {request.Length} generic program descriptions. ({DateTime.Now - dtStart:G})");
        }

        return ret;
    }
    public void ClearCache()
    {
        epgCache.ResetCache();
    }
}

