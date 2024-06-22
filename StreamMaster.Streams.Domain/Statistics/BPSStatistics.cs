﻿using Reinforced.Typings.Attributes;

using StreamMaster.Domain.Extensions;

using System.Collections.Concurrent;

namespace StreamMaster.Streams.Domain.Statistics;

[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]

public class BPSStatistics : ClientStatistics
{
    private const int SlidingWindowSeconds = 10; // Window size in seconds for average calculation
    private readonly ConcurrentQueue<(DateTimeOffset Timestamp, long BytesRead, long BytesWritten)> _recentData = new();

    public bool IsSet { get; set; } = false;
    // Properties required for MessagePack serialization
    public double BitsPerSecond { get; set; }
    public string? StreamUrl { get; set; }
    public long BytesRead { get; set; }
    public long BytesWritten { get; set; }

    public override void UpdateValues()
    {
        if (IsSet)
        {
            return;
        }

        DateTimeOffset now = SMDT.UtcNow;

        // Remove outdated entries beyond the sliding window
        while (_recentData.TryPeek(out var oldest) && (now - oldest.Timestamp).TotalSeconds > SlidingWindowSeconds)
        {
            _recentData.TryDequeue(out _);
        }

        // Calculate the total bytes in the sliding window
        var totalBytesRead = _recentData.Sum(entry => entry.BytesRead);
        var totalBytesWritten = _recentData.Sum(entry => entry.BytesWritten);
        double elapsedTimeInSeconds = (now - _recentData.FirstOrDefault().Timestamp).TotalSeconds;

        // Calculate BitsPerSecond over the sliding window
        BitsPerSecond = elapsedTimeInSeconds > 0 ? (totalBytesRead + totalBytesWritten) * 8 / elapsedTimeInSeconds : 0;

        base.UpdateValues();
    }
    //private void UpdateValueStats()
    //{
    //    IsSet = true;
    //    DateTimeOffset now = SMDT.UtcNow;
    //    double elapsedTimeInSeconds = (now - StartTime).TotalSeconds;
    //    BitsPerSecond = elapsedTimeInSeconds > 0 ? (BytesRead + BytesWritten) * 8 / elapsedTimeInSeconds : 0;

    //    base.UpdateValues();
    //}

    public void UpdateStatistic(StreamStreamingStatistic stat)
    {
        //if (StartTime == DateTimeOffset.MinValue)
        //{
        //    StartTime = SMDT.UtcNow;
        //}
        // Update bytes and add to the recent data for accurate calculation
        BytesRead = stat.BytesRead;
        BytesWritten = stat.BytesWritten;
        BitsPerSecond = stat.BitsPerSecond;
        StartTime = stat.StartTime;

        //UpdateValueStats();
    }

    public BPSStatistics Copy()
    {
        return new BPSStatistics
        {
            BitsPerSecond = this.BitsPerSecond,
            StreamUrl = this.StreamUrl,
            BytesRead = this.BytesRead,
            BytesWritten = this.BytesWritten,
            StartTime = this.StartTime,
            IsSet = this.IsSet,
            Clients = this.Clients
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