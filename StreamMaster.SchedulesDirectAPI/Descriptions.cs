using Microsoft.Extensions.Logging;

using System.Collections.Concurrent;
using System.Text.Json;

namespace StreamMaster.SchedulesDirectAPI;
public partial class SchedulesDirect
{
    private List<string> seriesDescriptionQueue;
    private ConcurrentDictionary<string, GenericDescription> seriesDescriptionResponses;

    private bool BuildAllGenericSeriesInfoDescriptions()
    {
        // reset counters
        seriesDescriptionQueue = [];
        seriesDescriptionResponses = [];
        ////IncrementNextStage(mxf.SeriesInfosToProcess.Count);
        logger.LogInformation($"Entering BuildAllGenericSeriesInfoDescriptions() for {totalObjects} series.");

        List<MxfSeriesInfo> a = schedulesDirectData.SeriesInfosToProcess;
        List<MxfProgram> b = schedulesDirectData.Programs;
        List<MxfProvider> c = schedulesDirectData.Providers;
        // fill mxf programs with cached values and queue the rest
        foreach (MxfSeriesInfo series in schedulesDirectData.SeriesInfosToProcess)
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

                    if (cached is not null && cached.Code == 0)
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
        logger.LogDebug($"Found {processedObjects} cached/unavailable series descriptions.");

        // maximum 500 queries at a time
        if (seriesDescriptionQueue.Count > 0)
        {
            Parallel.For(0, (seriesDescriptionQueue.Count / MaxImgQueries) + 1, new ParallelOptions { MaxDegreeOfParallelism = MaxParallelDownloads }, i =>
            {
                DownloadGenericSeriesDescriptions(i * MaxImgQueries);
            });

            ProcessSeriesDescriptionsResponses();
            if (processedObjects != totalObjects)
            {
                logger.LogWarning($"Failed to download and process {schedulesDirectData.SeriesInfosToProcess.Count - processedObjects} series descriptions.");
            }
        }
        logger.LogInformation("Exiting BuildAllGenericSeriesInfoDescriptions(). SUCCESS.");
        seriesDescriptionQueue = []; seriesDescriptionResponses = [];
        return true;
    }

    private void DownloadGenericSeriesDescriptions(int start = 0)
    {
        // reject 0 requests
        if (seriesDescriptionQueue.Count - start < 1)
        {
            return;
        }

        // build the array of series to request descriptions for
        string[] series = new string[Math.Min(seriesDescriptionQueue.Count - start, MaxImgQueries)];
        for (int i = 0; i < series.Length; ++i)
        {
            series[i] = seriesDescriptionQueue[start + i];
        }

        IEnumerable<string> test = series.Where(string.IsNullOrEmpty);
        if (test.Any())
        {
            int aaa = 1;
        }
        // request descriptions from Schedules Direct
        Dictionary<string, GenericDescription>? responses = GetGenericDescriptionsAsync(series, CancellationToken.None).Result;
        if (responses != null)
        {
            Parallel.ForEach(responses, (response) =>
            {
                seriesDescriptionResponses.TryAdd(response.Key, response.Value);
            });
        }
    }

    private void ProcessSeriesDescriptionsResponses()
    {
        // process request response
        foreach (KeyValuePair<string, GenericDescription> response in seriesDescriptionResponses)
        {
            //IncrementProgress();

            string seriesId = response.Key;
            GenericDescription description = response.Value;

            // determine which seriesInfo this belongs to
            MxfSeriesInfo mxfSeriesInfo = schedulesDirectData.FindOrCreateSeriesInfo(seriesId.Substring(2, 8));

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

    //private  bool BuildAllExtendedSeriesDataForUiPlus()
    //{
    //    // reset counters
    //    //IncrementNextStage(mxf.With.SeriesInfos.Count);
    //    seriesDescriptionQueue = new List<string>();
    //    return true;
    //    //if (!config.ModernMediaUiPlusSupport) return true;
    //    //logger.LogInformation($"Entering BuildAllExtendedSeriesDataForUiPlus() for {totalObjects} series.");

    //    //// read in cached ui+ extended information
    //    //var oldPrograms = new Dictionary<string, ModernMediaUiPlusPrograms>();
    //    //if (File.Exists(Helper.Epg123MmuiplusJsonPath))
    //    //{
    //    //    using (var sr = new StreamReader(Helper.Epg123MmuiplusJsonPath))
    //    //    {
    //    //        oldPrograms = JsonConvert.DeserializeObject<Dictionary<string, ModernMediaUiPlusPrograms>>(sr.ReadToEnd());
    //    //    }
    //    //}

    //    //// fill mxf programs with cached values and queue the rest
    //    //foreach (var series in mxf.With.SeriesInfos)
    //    //{
    //    //    // sports events will not have a generic description
    //    //    if (!series.SeriesId.StartsWith("SH"))
    //    //    {
    //    //        //IncrementProgress();
    //    //        continue;
    //    //    }

    //    //    var seriesId = $"SH{series.SeriesId}0000";

    //    //    // generic series information already in support file array
    //    //    if (ModernMediaUiPlus.Programs.ContainsKey(seriesId))
    //    //    {
    //    //        //IncrementProgress();
    //    //        if (ModernMediaUiPlus.Programs.TryGetValue(seriesId, out var program) && program.OriginalAirDate != null)
    //    //        {
    //    //            UpdateSeriesAirdate(seriesId, program.OriginalAirDate);
    //    //        }
    //    //        continue;
    //    //    }

    //    //    // extended information in current json file
    //    //    if (oldPrograms.ContainsKey(seriesId))
    //    //    {
    //    //        //IncrementProgress();
    //    //        if (oldPrograms.TryGetValue(seriesId, out var program) && !string.IsNullOrEmpty(program.OriginalAirDate))
    //    //        {
    //    //            ModernMediaUiPlus.Programs.Add(seriesId, program);
    //    //            UpdateSeriesAirdate(seriesId, program.OriginalAirDate);
    //    //        }
    //    //        continue;
    //    //    }

    //    //    // add to queue
    //    //    seriesDescriptionQueue.Add(seriesId);
    //    //}
    //    //logger.LogDebug($"Found {processedObjects} cached extended series descriptions.");

    //    //// maximum 5000 queries at a time
    //    //if (seriesDescriptionQueue.Count > 0)
    //    //{
    //    //    for (var i = 0; i < seriesDescriptionQueue.Count; i += MaxQueries)
    //    //    {
    //    //        if (GetExtendedSeriesDataForUiPlus(i)) continue;
    //    //        logger.LogWarning($"Failed to download and process {mxf.With.SeriesInfos.Count - processedObjects} extended series descriptions.");
    //    //        return true;
    //    //    }
    //    //}
    //    //logger.LogInformation("Exiting BuildAllExtendedSeriesDataForUiPlus(). SUCCESS.");
    //    //seriesDescriptionQueue = null; seriesDescriptionResponses = null;
    //    //return true;
    //}
    //private  async bool GetExtendedSeriesDataForUiPlus(int start = 0)
    //{
    //    // build the array of programs to request for
    //    var programs = new string[Math.Min(seriesDescriptionQueue.Count - start, MaxQueries)];
    //    for (var i = 0; i < programs.Length; ++i)
    //    {
    //        programs[i] = seriesDescriptionQueue[start + i];
    //    }

    //    // request programs from Schedules Direct
    //    var responses = await .GetPrograms(programs);
    //    if (responses == null) return false;

    //    // process request response
    //    var idx = 0;
    //    foreach (var response in responses)
    //    {
    //        if (response == null)
    //        {
    //            logger.LogWarning($"Did not receive data for program {programs[idx++]}.");
    //            continue;
    //        }
    //        ++idx; //IncrementProgress();

    //        //// add the series extended data to the file if not already present from program builds
    //        //if (!ModernMediaUiPlus.Programs.ContainsKey(response.ProgramId))
    //        //{
    //        //    AddModernMediaUiPlusProgram(response);
    //        //}

    //        // add the series start air date if available
    //        UpdateSeriesAirdate(response.ProgramId, response.OriginalAirDate);
    //    }
    //    return true;
    //}
    private void UpdateSeriesAirdate(string seriesId, string originalAirdate)
    {
        UpdateSeriesAirdate(seriesId, DateTime.Parse(originalAirdate));
    }
    private void UpdateSeriesAirdate(string seriesId, DateTime airdate)
    {
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
}


