using System.Collections.Concurrent;
using System.Text.Json;

using StreamMaster.Domain.Cache;
using StreamMaster.SchedulesDirect.Domain;

namespace StreamMaster.SchedulesDirect.Services;

public class DescriptionService(
    ILogger<DescriptionService> logger,
    ISchedulesDirectAPIService schedulesDirectAPI,
    IOptionsMonitor<SDSettings> sdSettings,
    SMCacheManager<GenericDescription> hybridCache,
    IProgramRepository programRepository) : IDescriptionService, IDisposable
{
    private readonly ConcurrentDictionary<string, string> descriptionsToProcess = new();
    private readonly SemaphoreSlim semaphore = new(SDAPIConfig.MaxParallelDownloads, SDAPIConfig.MaxParallelDownloads);
    private bool disposedValue;

    public async Task<bool> BuildGenericSeriesInfoDescriptionsAsync(CancellationToken cancellationToken)
    {
        if (!sdSettings.CurrentValue.EpisodeAppendProgramDescription)
        {
            return true;
        }

        ICollection<SeriesInfo> toProcess = programRepository.SeriesInfos.Values;

        logger.LogInformation("Entering BuildGenericSeriesInfoDescriptionsAsync() for {toProcess.Count} series.", toProcess.Count);

        await FillChannelWithDescriptionsFromSeriesInfoAsync(cancellationToken);

        if (descriptionsToProcess.IsEmpty)
        {
            logger.LogInformation("No descriptions to download. Exiting.");
            return true;
        }

        // Process descriptions in parallel
        List<Task> processingTasks = [];

        int threads = Math.Clamp(descriptionsToProcess.Values.Count, 1, SDAPIConfig.MaxParallelDownloads);

        for (int i = 0; i < threads; i++)
        {
            processingTasks.Add(Task.Run(() => FetchAndProcessDescriptionsAsync(cancellationToken), cancellationToken));
        }

        await Task.WhenAll(processingTasks).ConfigureAwait(false);

        logger.LogInformation("Exiting BuildGenericSeriesInfoDescriptionsAsync(). SUCCESS.");
        return true;
    }

    private async Task FillChannelWithDescriptionsFromSeriesInfoAsync(CancellationToken cancellationToken)
    {
        ICollection<SeriesInfo> toProcess = programRepository.SeriesInfos.Values;

        foreach (SeriesInfo series in toProcess)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            if (series.SeriesId.StartsWith("SP") || string.IsNullOrEmpty(series.ProgramId))
            {
                continue;
            }

            string seriesId = $"SH{series.SeriesId}0000";

            GenericDescription? cached = await hybridCache.GetAsync<GenericDescription>(seriesId);
            if (cached is not null)
            {
                if (cached.Code == 0)
                {
                    series.ShortDescription = cached.Description100;
                    series.Description = cached.Description1000;
                    if (!string.IsNullOrEmpty(cached.StartAirdate))
                    {
                        series.StartAirdate = cached.StartAirdate;
                    }
                }
            }
            else
            {
                descriptionsToProcess.TryAdd(series.ProgramId, series.ProgramId);
            }
        }
    }

    private async Task FetchAndProcessDescriptionsAsync(CancellationToken cancellationToken)
    {
        List<string> batch = [];

        foreach (string seriesId in descriptionsToProcess.Values)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (seriesId is null)
            {
                continue;
            }

            batch.Add(seriesId);

            if (batch.Count >= SDAPIConfig.MaxDescriptionQueries)
            {
                await ProcessBatchAsync([.. batch], cancellationToken).ConfigureAwait(false);
                batch.Clear();
            }
        }

        if (batch.Count > 0)
        {
            await ProcessBatchAsync([.. batch], cancellationToken).ConfigureAwait(false);
        }
    }

    private async Task ProcessBatchAsync(string[] batch, CancellationToken cancellationToken)
    {
        await semaphore.WaitAsync(cancellationToken);
        try
        {
            Dictionary<string, GenericDescription>? responses = await schedulesDirectAPI.GetDescriptionsAsync(batch, cancellationToken).ConfigureAwait(false);

            if (responses != null)
            {
                await ProcessSeriesDescriptionsResponsesAsync(responses, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            logger.LogError("Error fetching series descriptions for batch: {Error}", ex.Message);
        }
        finally
        {
            semaphore.Release();
        }
    }

    private async Task ProcessSeriesDescriptionsResponsesAsync(Dictionary<string, GenericDescription> seriesDescriptionResponses, CancellationToken cancellationToken)
    {
        Dictionary<string, string> bulkItems = [];

        foreach (KeyValuePair<string, GenericDescription> response in seriesDescriptionResponses)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }
            string seriesId = response.Key;
            GenericDescription description = response.Value;

            SeriesInfo? seriesInfo = programRepository.FindSeriesInfo(seriesId.Substring(2, 8));
            if (seriesInfo == null)
            {
                continue;
            }
            seriesInfo.ShortDescription = description.Description100;
            seriesInfo.Description = description.Description1000;
            bulkItems[seriesId] = JsonSerializer.Serialize(description);
        }

        if (bulkItems.Count > 0)
        {
            await hybridCache.SetBulkAsync(bulkItems).ConfigureAwait(false);
        }
    }

    public static void ResetCache()
    {
        //hybridCache.ClearAsync().ConfigureAwait(false);
    }

    public List<string> GetExpiredKeys()
    {
        return hybridCache.GetExpiredKeysAsync().Result;
    }

    public void RemovedExpiredKeys(List<string>? keysToDelete = null)
    {
        // RemovedExpiredKeys logic is redundant with hybridCache
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                semaphore.Dispose();
                descriptionsToProcess.Clear();
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}