using System.Text.Json;

using StreamMaster.Domain.Configuration;

namespace StreamMaster.Streams.Domain.Metrics;

public class StreamConnectionMetrics
{
    public StreamConnectionMetric StreamConnectionMetric { get; }

    private int startCount = 0;
    private readonly TimeSpan totalReconnectTime = TimeSpan.Zero;
    private readonly Lock lockObj = new();

    private static readonly SemaphoreSlim SaveSemaphore = new(1, 1);
    private static readonly System.Timers.Timer SaveDebounceTimer = new(1000) { AutoReset = false }; // 1-second debounce
    private static bool SavePending = false;
    private string Id { get; }
    private readonly string MetricsFilePath;

    public StreamConnectionMetrics(string Id, string StreamUrl)
    {
        this.Id = Id;
        MetricsFilePath = Path.Combine(BuildInfo.StreamHealthFolder, Id + ".json");
        StreamConnectionMetric = LoadMetrics(StreamUrl);

        SaveDebounceTimer.Elapsed += async (_, _) =>
        {
            if (!SavePending)
            {
                return;
            }

            await SaveSemaphore.WaitAsync();
            try
            {
                await SaveMetricsAsync();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error saving metrics: {ex}");
            }
            finally
            {
                SavePending = false;
                SaveSemaphore.Release();
            }
        };
    }

    private static void SignalMetricsChanged()
    {
        lock (SaveDebounceTimer)
        {
            SavePending = true;
            SaveDebounceTimer.Stop();
            SaveDebounceTimer.Start();
        }
    }
    public void RecordSuccessConnect(long connectionTime)
    {
        lock (lockObj)
        {
            startCount++;
            TimeSpan startTime = TimeSpan.FromMilliseconds(connectionTime);
            StreamConnectionMetric.AvgConnectionTime = TimeSpan.FromTicks(
               ((StreamConnectionMetric.AvgConnectionTime.Ticks * (startCount - 1)) + startTime.Ticks) / startCount
           );
            StreamConnectionMetric.LastSuccessConnect = DateTime.UtcNow;
        }
        SignalMetricsChanged();
    }

    public void RecordError()
    {
        lock (lockObj)
        {
            StreamConnectionMetric.LastErrorTime = DateTime.UtcNow;
        }
        SignalMetricsChanged();
    }

    public void IncrementRetryCount()
    {
        lock (lockObj)
        {
            StreamConnectionMetric.RetryCount++;
            StreamConnectionMetric.LastRetryTime = DateTime.UtcNow;
        }
        SignalMetricsChanged();
    }

    public void RecordConnectionAttempt(bool wasSuccessful)
    {
        lock (lockObj)
        {
            StreamConnectionMetric.TotalConnectionAttempts++;

        }
        SignalMetricsChanged();
    }

    private StreamConnectionMetric LoadMetrics(string StreamUrl)
    {
        try
        {
            if (!File.Exists(MetricsFilePath))
            {
                return new StreamConnectionMetric(StreamUrl);
            }

            using FileStream openStream = File.OpenRead(MetricsFilePath);
            StreamConnectionMetric? data = JsonSerializer.Deserialize<StreamConnectionMetric>(openStream);
            return data ?? new StreamConnectionMetric(StreamUrl);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error loading metrics for {StreamUrl}: {ex}");
            return new StreamConnectionMetric(StreamUrl);
        }
    }

    private async Task SaveMetricsAsync(CancellationToken cancellationToken = default)
    {
        await using FileStream createStream = File.Create(MetricsFilePath);
        await JsonSerializer.SerializeAsync(createStream, StreamConnectionMetric, BuildInfo.JsonIndentOptions, cancellationToken);
    }
}
