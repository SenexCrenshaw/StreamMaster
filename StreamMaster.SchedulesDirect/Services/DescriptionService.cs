using System.Collections.Concurrent;
using System.Text.Json;
using System.Threading.Channels;

using StreamMaster.Domain.Cache;

namespace StreamMaster.SchedulesDirect.Services;

public class DescriptionService(
    ILogger<DescriptionService> logger,
    ISchedulesDirectAPIService schedulesDirectAPI,
    IHybridCache<GenericDescription> hybridCache,
    ISchedulesDirectDataService schedulesDirectDataService) : IDescriptionService, IDisposable
{
    private readonly Channel<string> descriptionChannel = Channel.CreateUnbounded<string>();
    private readonly ConcurrentDictionary<string, GenericDescription> seriesDescriptionResponses = new();
    private readonly SemaphoreSlim semaphore = new(SchedulesDirect.MaxParallelDownloads, SchedulesDirect.MaxParallelDownloads);
    private bool disposedValue;

    public async Task<bool> BuildGenericSeriesInfoDescriptionsAsync(CancellationToken cancellationToken)
    {
        ISchedulesDirectData schedulesDirectData = schedulesDirectDataService.SchedulesDirectData();
        ICollection<SeriesInfo> toProcess = schedulesDirectData.SeriesInfosToProcess.Values;

        logger.LogInformation("Entering BuildGenericSeriesInfoDescriptionsAsync() for {toProcess.Count} series.", toProcess.Count);

        // Populate the channel with series descriptions to process
        await FillChannelWithDescriptionsAsync(toProcess);

        if (!descriptionChannel.Reader.CanCount || descriptionChannel.Reader.Count == 0)
        {
            logger.LogInformation("No descriptions to download. Exiting.");
            return true;
        }

        // Process descriptions in parallel
        List<Task> processingTasks = [];
        for (int i = 0; i < SchedulesDirect.MaxParallelDownloads; i++)
        {
            processingTasks.Add(Task.Run(() => FetchAndProcessDescriptionsAsync(cancellationToken), cancellationToken));
        }

        await Task.WhenAll(processingTasks).ConfigureAwait(false);

        ProcessSeriesDescriptionsResponses();
        logger.LogInformation("Exiting BuildGenericSeriesInfoDescriptionsAsync(). SUCCESS.");
        return true;
    }

    private async Task FillChannelWithDescriptionsAsync(ICollection<SeriesInfo> toProcess)
    {
        foreach (SeriesInfo series in toProcess)
        {
            if (series.SeriesId.StartsWith("SP") || string.IsNullOrEmpty(series.ProtoTypicalProgram))
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
                await descriptionChannel.Writer.WriteAsync(series.ProtoTypicalProgram).ConfigureAwait(false);
            }
        }
        descriptionChannel.Writer.Complete();
    }

    private async Task FetchAndProcessDescriptionsAsync(CancellationToken cancellationToken)
    {
        List<string> batch = [];
        while (await descriptionChannel.Reader.WaitToReadAsync(cancellationToken).ConfigureAwait(false))
        {
            while (descriptionChannel.Reader.TryRead(out string seriesId))
            {
                batch.Add(seriesId);

                if (batch.Count >= SchedulesDirect.MaxDescriptionQueries)
                {
                    await ProcessBatchAsync(batch, cancellationToken).ConfigureAwait(false);
                    batch.Clear();
                }
            }
        }

        if (batch.Count > 0)
        {
            await ProcessBatchAsync(batch, cancellationToken).ConfigureAwait(false);
        }
    }

    private async Task ProcessBatchAsync(List<string> batch, CancellationToken cancellationToken)
    {
        await semaphore.WaitAsync(cancellationToken);
        try
        {
            Dictionary<string, GenericDescription>? responses = await schedulesDirectAPI
                .GetApiResponse<Dictionary<string, GenericDescription>?>(APIMethod.POST, "metadata/description/", batch.ToArray(), cancellationToken)
                .ConfigureAwait(false);

            if (responses != null)
            {
                foreach (KeyValuePair<string, GenericDescription> response in responses)
                {
                    seriesDescriptionResponses.TryAdd(response.Key, response.Value);
                }
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

    private void ProcessSeriesDescriptionsResponses()
    {
        ISchedulesDirectData schedulesDirectData = schedulesDirectDataService.SchedulesDirectData();

        foreach (KeyValuePair<string, GenericDescription> response in seriesDescriptionResponses)
        {
            string seriesId = response.Key;
            GenericDescription description = response.Value;

            SeriesInfo seriesInfo = schedulesDirectData.FindOrCreateSeriesInfo(seriesId.Substring(2, 8));
            seriesInfo.ShortDescription = description.Description100;
            seriesInfo.Description = description.Description1000;

            try
            {
                string jsonString = JsonSerializer.Serialize(description);
                hybridCache.SetAsync(seriesId, jsonString).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                logger.LogWarning("Failed to cache description for series {SeriesId}: {Error}", seriesId, ex.Message);
            }
        }
    }

    public void ResetCache()
    {
        hybridCache.ClearAsync().ConfigureAwait(false);
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
                descriptionChannel.Writer.Complete();
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
