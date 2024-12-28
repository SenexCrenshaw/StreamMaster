using System.Text.Json;

using StreamMaster.Domain.Configuration;

namespace StreamMaster.Streams.Domain.Metrics;

public static class StreamConnectionMetricSerializer
{
    public static StreamConnectionMetricData? Load(string filePath, string? streamUrl = null)
    {
        if (!File.Exists(filePath))
        {
            return streamUrl is null ? null : new StreamConnectionMetricData(streamUrl);
        }

        try
        {
            using FileStream openStream = File.OpenRead(filePath);
            return JsonSerializer.Deserialize<StreamConnectionMetricData>(openStream) ??
                   (streamUrl is null ? null : new StreamConnectionMetricData(streamUrl));
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error loading metrics from {filePath}: {ex}");
            return streamUrl is null ? null : new StreamConnectionMetricData(streamUrl);
        }
    }

    public static async Task SaveAsync(string filePath, StreamConnectionMetricData metricData, CancellationToken cancellationToken = default)
    {
        try
        {
            await using FileStream createStream = File.Create(filePath);
            await JsonSerializer.SerializeAsync(createStream, metricData, BuildInfo.JsonIndentOptions, cancellationToken);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error saving metrics to {filePath}: {ex}");
        }
    }
}
