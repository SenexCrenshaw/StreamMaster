using StreamMaster.Streams.Domain.Statistics;

using System.Collections.Concurrent;

namespace StreamMaster.Streams.Statistics;

public sealed class StreamStreamingStatisticsManager(ILogger<StreamStreamingStatisticsManager> logger) : IStreamStreamingStatisticsManager
{
    private readonly ConcurrentDictionary<string, StreamStreamingStatistic> _streamStreamingStatistics = new();
    private readonly ILogger<StreamStreamingStatisticsManager> _logger = logger;

    public StreamStreamingStatistic RegisterStream(SMStreamDto smStream)
    {
        if (!_streamStreamingStatistics.ContainsKey(smStream.Id))
        {
            var c = new StreamStreamingStatistic();
            c.SetStream(smStream);
            _streamStreamingStatistics.TryAdd(smStream.Id, c);

        }
        return _streamStreamingStatistics[smStream.Id];
    }

    public bool UnRegisterStream(string SMStreamId)
    {
        return _streamStreamingStatistics.TryRemove(SMStreamId, out _);
    }

    public void AddBytesRead(string SMStreamId, int bytesRead)
    {
        if (_streamStreamingStatistics.TryGetValue(SMStreamId, out StreamStreamingStatistic? clientStats))
        {
            clientStats.AddBytesRead(bytesRead);
        }

    }
    public List<StreamStreamingStatistic> GetStreamingStatistics()
    {
        return [.. _streamStreamingStatistics.Values];
    }

    public List<string> GetAllClientIds()
    {
        return [.. _streamStreamingStatistics.Keys];
    }

    public void IncrementBytesRead(string SMStreamId)
    {
        if (_streamStreamingStatistics.TryGetValue(SMStreamId, out StreamStreamingStatistic? clientStats))
        {
            clientStats.IncrementBytesRead();
        }
        else
        {
            _logger.LogWarning("Client {clientId} not found when trying to increment read.", SMStreamId);
        }
    }
}