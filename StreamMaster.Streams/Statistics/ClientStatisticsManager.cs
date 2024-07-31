using StreamMaster.Domain.Extensions;
using StreamMaster.Streams.Domain.Statistics;

using System.Collections.Concurrent;

namespace StreamMaster.Streams.Statistics;

public sealed class ClientStatisticsManager(ILogger<ClientStatisticsManager> logger) : IClientStatisticsManager
{
    private readonly ConcurrentDictionary<string, ClientStreamingStatistics> _clientStatistics = new();
    private readonly ILogger<ClientStatisticsManager> _logger = logger;

    public void RegisterClient(IClientConfiguration streamerConfiguration)
    {
        if (!_clientStatistics.ContainsKey(streamerConfiguration.UniqueRequestId))
        {
            ClientStreamingStatistics c = new();
            c.SetStreamerConfiguration(streamerConfiguration);
            c.StartTime = SMDT.UtcNow;
            _clientStatistics.TryAdd(streamerConfiguration.UniqueRequestId, c);

        }
        return;
    }

    public bool UnRegisterClient(string UniqueRequestId)
    {
        return _clientStatistics.TryRemove(UniqueRequestId, out _);
    }

    public void AddBytesRead(string UniqueRequestId, int bytesRead)
    {
        if (_clientStatistics.TryGetValue(UniqueRequestId, out ClientStreamingStatistics? clientStats))
        {
            clientStats.AddBytesRead(bytesRead);
        }

    }
    public List<ClientStreamingStatistics> GetAllClientStatistics()
    {
        return [.. _clientStatistics.Values];
    }

    public List<string> GetAllUniqueRequestIds()
    {
        return [.. _clientStatistics.Keys];
    }

    public void IncrementBytesRead(string UniqueRequestId)
    {
        if (_clientStatistics.TryGetValue(UniqueRequestId, out ClientStreamingStatistics? clientStats))
        {
            clientStats.IncrementBytesRead();
        }
        else
        {
            _logger.LogWarning("Client {UniqueRequestId} not found when trying to increment read.", UniqueRequestId);
        }
    }
}