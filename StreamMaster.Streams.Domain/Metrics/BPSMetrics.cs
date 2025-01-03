using System.Collections.Concurrent;

using Reinforced.Typings.Attributes;

using StreamMaster.Domain.Extensions;

namespace StreamMaster.Streams.Domain.Metrics;

[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public class BPSMetrics
{
    private const int SlidingWindowSeconds = 10; // Window size in seconds for average calculation
    private readonly ConcurrentQueue<(DateTimeOffset Timestamp, long BytesRead, long BytesWritten)> _recentData = new();
    public bool IsSet { get; set; } = false;
    public double BitsPerSecond { get; set; }
    public long BytesRead { get; set; }
    public long BytesWritten { get; set; }
    public int Clients { get; set; }
    public void DecrementClient()
    {
        --Clients;
    }
    public void IncrementClient()
    {
        ++Clients;
    }

    public void UpdateValues()
    {
        if (IsSet)
        {
            return;
        }

        DateTimeOffset now = SMDT.UtcNow;

        // Remove outdated entries beyond the sliding window
        while (_recentData.TryPeek(out (DateTimeOffset Timestamp, long BytesRead, long BytesWritten) oldest) && (now - oldest.Timestamp).TotalSeconds > SlidingWindowSeconds)
        {
            _recentData.TryDequeue(out _);
        }

        // Calculate the total bytes in the sliding window
        long totalBytesRead = _recentData.Sum(entry => entry.BytesRead);
        long totalBytesWritten = _recentData.Sum(entry => entry.BytesWritten);
        double elapsedTimeInSeconds = (now - _recentData.FirstOrDefault().Timestamp).TotalSeconds;

        // Calculate BitsPerSecond over the sliding window
        BitsPerSecond = elapsedTimeInSeconds > 0 ? (totalBytesRead + totalBytesWritten) * 8 / elapsedTimeInSeconds : 0;

        TimeSpan elapsedTime = new(0);
        if (StartTime != DateTimeOffset.MinValue)
        {
            elapsedTime = SMDT.UtcNow - StartTime;
        }
        ElapsedTime = $"{elapsedTime.Days} {elapsedTime.Hours:00}:{elapsedTime.Minutes:00}:{elapsedTime.Seconds:00}";
    }
    public DateTimeOffset StartTime { get; set; }
    public string ElapsedTime { get; set; } = string.Empty;

    public BPSMetrics Copy()
    {
        return new BPSMetrics
        {
            BitsPerSecond = BitsPerSecond,
            BytesRead = BytesRead,
            BytesWritten = BytesWritten,
            IsSet = IsSet
        };
    }

    public void AddBytesRead(long bytesRead)
    {
        BytesRead += bytesRead;
        AddToRecentData(bytesRead, 0);
    }

    public void AddBytesWritten(long bytesWritten)
    {
        BytesWritten += bytesWritten;
        AddToRecentData(0, bytesWritten);
    }

    public void IncrementBytesRead()
    {
        AddBytesRead(1);
    }

    public void IncrementBytesWritten()
    {
        AddBytesWritten(1);
    }

    private void AddToRecentData(long bytesRead, long bytesWritten)
    {
        DateTimeOffset now = SMDT.UtcNow;
        _recentData.Enqueue((now, bytesRead, bytesWritten));
        UpdateValues();
    }
}
